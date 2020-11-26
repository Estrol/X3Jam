using System;
using System.IO;
using System.Text;
using System.Windows;
using Estrol.X3Jam.Server.Data;
using Estrol.X3Jam.Server.Utils;
using Estrol.X3Jam.Server.Handlers;
using System.Linq;

namespace Estrol.X3Jam.Server {
    public class ServerMain {
        public Server Server;
        public OJNList OJNList;
        public RoomManager RoomMG;
        public ChannelManager ChannelMG;
        public Database Database;

        public ServerMain() {}

        public void Intialize() {
            Server = new Server(this, 15010, 15000);
            RoomMG = new RoomManager();
            Database = new Database(this);
            ChannelMG = new ChannelManager();
            LoadOJNList();

            Server.OnServerMessage += this.TCPEvent;
            Server.Start();
        }

        public void LoadOJNList() {
            try {
                this.OJNList = OJNListDecoder.Decode(AppDomain.CurrentDomain.BaseDirectory + @"\Image\OJNList.dat");
                Console.WriteLine("[Server] Loaded OJNList with songs count {0}", this.OJNList.Count);
            } catch (Exception) {
                Console.WriteLine("[Server] Failed to load OJNList");
            }
        }

        public void TCPEvent(object o, Connection c) {
            PacketManager PM = new PacketManager(c.Buffer);

            switch (PM.opcode) {
                case Packets.Connect: new Connect(c, PM); break;
                case Packets.PlanetConnect: new PlanetConnect(c, PM); break;

                case Packets.Login: new Login(c, PM, this); break;
                case Packets.PlanetLogin: new PlanetLogin(c, PM, this); break;

                case Packets.Channel: new Channel(c, PM); break;
                case Packets.EnterCH: {
                    int ChannelID = PM.data[4] + 1;
                    if (ChannelID > 4) {
                        Console.WriteLine("[Server] [{0}] Attempting to enter channel: {1}, which doesn't exists in server!",
                            c.UserInfo.GetUsername(),
                            ChannelID);

                        break;
                    }

                    Console.WriteLine("[Server] [{0}] Entering channel: {1}",
                        c.UserInfo.GetUsername(),
                        ChannelID);

                    c.UserInfo.SetChannel(ChannelID);
                    var CH = ChannelMG.GetChannelByID(ChannelID);
                    CH.AddUser(c.UserInfo);

                    c.Send(new byte[] {
                        0x10, 0x00, 0xed, 0x03, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 // 0x01, 0x00, 0x00, 0x00 -> Player Ranking in leaderboard
                    });
                    break;
                }

                case Packets.LeaveCH: {
                    var CH = ChannelMG.GetChannelByID(c.UserInfo.GetChannel());
                    CH.RemoveUser(c.UserInfo);

                    c.Send(new byte[] {
                        0x08, 00, 0xe6, 0x07, 0x00, 0x00, 0x00, 0x00
                    });
                    break;
                }

                case Packets.SetSongID: {
                    ushort SongID = BitConverter.ToUInt16(PM.data, 0);
                    Room room = RoomMG.GetRoomById(c.UserInfo.GetRoom());

                    room.SetSongID(SongID);
                    Console.WriteLine("[Server] [Channel: {0}, Room: {1}] Set SongID: {3}",
                        c.UserInfo.GetChannel(),
                        c.UserInfo.GetRoom(),
                        SongID);

                    ushort p_len = (ushort)(PM.data.Length + 2);
                    byte[] len = BitConverter.GetBytes(p_len);
                    byte[] data = len.Concat(PM.data).ToArray();
                    data[2] = 0xa1;



                    c.Send(data);
                    break;
                }

                case Packets.CreateRoom: {
                    string RoomName = DataUtils.GetString(PM.data);

                    Console.WriteLine("[Server] [{0}] Create a Room with name: \"{1}\", in channel: {2}",
                        c.UserInfo.GetUsername(),
                        RoomName,
                        c.UserInfo.GetChannel());

                    int roomID = RoomMG.GetNearestEmptyRoomID();
                    Room room = new Room(roomID, RoomName, c.UserInfo, 0x0);

                    c.UserInfo.SetRoom(roomID);
                    RoomMG.AddRoom(room);

                    c.Send(new byte[] {
                        0x0d, 0x00, 0xd6, 0x07, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00
                    });
                    break;
                }

                case Packets.GetRoom: new GetRoom(c, PM, this); break;

                case Packets.GetChar: new PlayerCharacter(c, PM); break;

                case Packets.Timest: {
                    c.Send(new byte[] {
                        0x08, 0x00, 0xa5, 0x13, 0xc6, 
                        0xf5, 0x02, 0x00
                    });
                    break;
                }

                case Packets.Payload: {
                    c.Send(new byte[] {
                        0x04, 0x00, 0xe9, 0x07
                    });
                    break;
                }

                case Packets.TCPPing: {
                    c.Send(new byte[] {
                        0x04, 0x00, 0x71, 0x17
                    });
                    break;
                }

                case Packets.OJNList: {
                    var ms = new MemoryStream();
                    var bw = new BinaryWriter(ms);

                    var headers = OJNList.GetHeaders();
                    short packetLength = (short)(6 + (headers.Length * 12) + 12);
                    bw.Write(packetLength);
                    bw.Write(new byte[] { 0xBF, 0x0F }); // Header?
                    bw.Write((short)OJNList.Count);

                    foreach (OJN ojn in headers) {
                        bw.Write((short)ojn.Id);
                        bw.Write((short)ojn.NoteCountEx);
                        bw.Write((short)ojn.NoteCountNx);
                        bw.Write((short)ojn.NoteCountHx);
                        bw.Write(new byte[4]);
                    }
                    
                    Console.WriteLine("[Server] [{0}] Get MusicList Info!", c.UserInfo.GetUsername());
                    bw.Write(new byte[12]);
                    c.Send(ms.ToArray());
                    break;
                }

                case Packets.Disconnect: {
                    Console.WriteLine("[Server] [{0}] Disconnected!", c.UserInfo != null ? c.UserInfo.GetUsername() : "null");
                    c.Socket.Disconnect(true);
                    return;
                }

                default: {
                    byte[] opcode = new byte[2];
                    Buffer.BlockCopy(PM.data, 0, opcode, 0, 2);

                    string message = string.Format("Unknown Packet: {0}", ByteArrayToString(opcode));
                    byte[] _data;
                    string msg4 = "<Empty>";
                    if (PM.data.Length > 2) {
                        _data = new byte[PM.data.Length - 2];
                        Buffer.BlockCopy(PM.data, 2, _data, 0, PM.data.Length - 2);

                        msg4 = ByteArrayToString(_data);
                    }


                    Console.WriteLine("[Handlers] {0}\n"
                        + "[Handlers]  - Stack Trace:\n"
                        + "[Handlers]  - Opcode: {1}\n"
                        + "[Handlers]  - Data: {2}"
                        , message
                        , PM._opcode.ToString("X4")
                        , msg4);
                    break;
                }
            }

            // Listen for another data again
            c.Read();
        }

        public void RoomSendMessage(int RoomID, string Message) {
            Room room = RoomMG.GetRoomById(RoomID);
            User[] users = room.GetUsers();

             
        }

        public static string ByteArrayToString(byte[] ba) {
            return BitConverter.ToString(ba).Replace("-", " ");
        }
    }
}
