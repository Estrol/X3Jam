using System;
using System.IO;
using System.Text;
using System.Windows;
using Estrol.X3Jam.Server.Data;
using Estrol.X3Jam.Server.Utils;
using Estrol.X3Jam.Server.Handlers;
using System.Linq;
using System.Collections.Generic;

namespace Estrol.X3Jam.Server {
    public class ServerMain {
        public Server Server;
        public OJNList OJNList;
        public RoomManager RoomMG;
        public ChannelManager ChannelMG;
        public Database Database;
        public Config Config;

        public ServerMain() {}

        public void Intialize() {
            Server = new Server(15010);
            RoomMG = new RoomManager();
            Config = new Config();
            Database = new Database(this);
            ChannelMG = new ChannelManager(this);

            Server.OnServerMessage += this.TCPEvent;
            Server.Start();
        }

        public void TCPEvent(object o, Connection c) {
            PacketManager PM = new PacketManager(c.Buffer);

            switch (PM.opcode) {
                case Packets.Connect: new Connect(c, PM); break;
                case Packets.PlanetConnect: new PlanetConnect(c, PM); break;

                case Packets.Login: new Login(c, PM, this); break;
                case Packets.PlanetLogin: new PlanetLogin(c, PM, this); break;

                case Packets.Channel: new Channel(c, PM, this); break;
                case Packets.EnterCH: {
                    int ChannelID = PM.data[4] + 1;
                    if (ChannelID > ChannelMG.ChannelCount) {
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

                    c.UserInfo.SetChannel(null);

                    c.Send(new byte[] {
                        0x08, 00, 0xe6, 0x07, 0x00, 0x00, 0x00, 0x00
                    });
                    break;
                }

                case Packets.SetSongID: {
                    ushort SongID = BitConverter.ToUInt16(PM.data, 2);
                    Room room = RoomMG.GetRoomById(c.UserInfo.GetRoom());

                    room.SetSongID(SongID);
                    Console.WriteLine("[Server] [Channel: {0}, Room: {1}] Set SongID: {2}",
                        c.UserInfo.GetChannel(),
                        c.UserInfo.GetRoom(),
                        SongID);

                    RoomSendMSG(string.Format("Setting Room {0}'s SongID to {1}", room.RoomID, SongID), room.RoomID);
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

                    RoomSendMSG(string.Format("Room created successfully with ID: {0}", room.RoomID), room.RoomID);
                    break;
                }

                case Packets.LeaveRoom: {
                    Room room = RoomMG.GetRoomById(c.UserInfo.GetRoom());
                    room.RemoveUser(c.UserInfo);
                    c.UserInfo.SetRoom(null);

                    c.Send(new byte[] {
                        0x08, 0x00, 0xbe, 0x0b, 0x00, 0x00, 0x00, 0x00
                    });
                    break;
                }

                case Packets.RoomInit: {
                    c.Send(new byte[] {
                        0x06, 0x00, 0xa5, 0x0f, 0x00, 0x00
                    });
                    c.Send(new byte[] {
                        0x08, 0x00, 0xa3, 0x0f, 0x09, 0x00, 0x00, 0x80, 
                        0x09, 0x00, 0xb8, 0x0f, 0x01, 0x00, 0x00, 0x00,
                        0x00
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

                case Packets.ClientMSG: {
                    string message = DataUtils.GetString(PM.data);
                    ChannelItem ch = ChannelMG.GetChannelByID(c.UserInfo.GetChannel());
                    User usr = c.UserInfo;

                    if (message.StartsWith("!") && message.Length > 1) {
                        char[] seperator = { ' ' };
                        string[] _args = message.Replace("!", "").Split(seperator, StringSplitOptions.RemoveEmptyEntries);

                        string command = _args[0];
                        string[] args = new string[_args.Length - 1];
                        Array.Copy(_args, 1, args, 0, _args.Length - 1);

                        switch (command) {
                            case "whois": {
                                ListSendMSG("Not implemented yet!", ch);
                                break;
                            }

                            case "send": {
                                ListSendMSG(string.Join(" ", args), ch);
                                break;
                            }

                            case "count": {
                                ListSendMSG(string.Format("Current users in CH {0}: {1}", ch.m_ChannelID, ch.GetUsers().Length), ch);
                                break;
                            }

                            default: {
                                ListSendMSG(string.Format("Unknown command: {0}", command), ch);
                                break;
                            }
                        }
                    } else {
                        ListSendMSG(message, ch, usr);
                    }
                    break;
                }

                case Packets.ClientMSG2: {
                    string message = DataUtils.GetString(PM.data);
                    User usr = c.UserInfo;

                    RoomSendMSG(message, usr.GetRoom(), usr);
                    break;
                }

                case Packets.OJNList: {
                    var ms = new MemoryStream();
                    var bw = new BinaryWriter(ms);

                    ChannelItem ch = ChannelMG.GetChannelByID(c.UserInfo.GetChannel());
                    var headers = ch.GetMusicList();

                    if (headers.Length > 500) {
                        var split_headers = headers.Split(2).ToArray();

                        for (int i = 0; i < split_headers.Length; i++) {
                            var hHeader = split_headers[i].ToArray();
                            short length = (short)(6 + (hHeader.Length * 12) + 12);

                            bw.Write(length);
                            bw.Write(new byte[] { 0xBF, 0x0F }); // Header?
                            bw.Write((short)hHeader.Length);

                            foreach (OJN ojn in hHeader) {
                                bw.Write(new byte[] {

                                });

                                bw.Write((short)ojn.Id);
                                bw.Write((short)ojn.NoteCountEx);
                                bw.Write((short)ojn.NoteCountNx);
                                bw.Write((short)ojn.NoteCountHx);
                                bw.Write(new byte[4]);
                            }

                            bw.Write(new byte[12]);
                        }
                    } else {
                        short packetLength = (short)(6 + (headers.Length * 12) + 12);
                        bw.Write(packetLength);
                        bw.Write(new byte[] { 0xBF, 0x0F }); // Header?
                        bw.Write((short)ch.GetListCount());

                        foreach (OJN ojn in headers) {
                            bw.Write((short)ojn.Id);
                            bw.Write((short)ojn.NoteCountEx);
                            bw.Write((short)ojn.NoteCountNx);
                            bw.Write((short)ojn.NoteCountHx);
                            bw.Write(new byte[4]);
                        }

                        bw.Write(new byte[12]);
                    }

                    Console.WriteLine("[Server] [{0}] Get MusicList Info!", c.UserInfo.GetUsername());
                    c.Send(ms.ToArray());
                    break;
                }

                case Packets.Disconnect: {
                    string usrStr = "null";

                    if (c.UserInfo != null) {
                        var CH = ChannelMG.GetChannelByID(c.UserInfo.GetChannel());
                        if (CH != null) {
                            CH.RemoveUser(c.UserInfo);
                        }
                        usrStr = c.UserInfo.GetUsername();
                    }

                    Console.WriteLine("[Server] [{0}] Disconnected!", usrStr);
                    c.m_socket.Disconnect(true);
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

        public void ListSendMSG(string Message, ChannelItem ch, User usr = null) {
            User[] users = ch.GetUsers();

            using (var stream = new MemoryStream())
            using (var bw = new BinaryWriter(stream)) {
                bw.Write((short)0); // Initial len header
                bw.Write(new byte[] { 0xdd, 0x07 });

                char[] str;
                if (usr != null) {
                    str = usr.GetNickname().ToCharArray();
                } else {
                    str = "System".ToCharArray();
                }

                bw.Write(Encoding.UTF8.GetBytes(str));
                bw.Write((byte)0x00);

                char[] msg = Message.ToCharArray();
                bw.Write(Encoding.UTF8.GetBytes(msg));
                bw.Write((byte)0x00);

                bw.Seek(0, SeekOrigin.Begin);
                bw.Write((short)stream.Length);

                byte[] data = stream.ToArray();

                for (int i = 0; i < users.Length; i++) {
                    User user = users[i];

                    user.SendMessage(data);
                }
            }
        }

        public void RoomSendMSG(string Message, int RoomID, User usr = null) {
            Room room = RoomMG.GetRoomById(RoomID);
            User[] users = room.GetUsers();

            using (var stream = new MemoryStream())
            using (var bw = new BinaryWriter(stream)) {
                bw.Write((short)0); // Initial len header
                bw.Write(new byte[] { 0xdd, 0x07 });

                char[] str;
                if (usr != null) {
                    str = usr.GetNickname().ToCharArray();
                } else {
                    str = "System".ToCharArray();
                }

                bw.Write(Encoding.UTF8.GetBytes(str));
                bw.Write((byte)0x00);

                char[] msg = Message.ToCharArray();
                bw.Write(Encoding.UTF8.GetBytes(msg));
                bw.Write((byte)0x00);

                bw.Seek(0, SeekOrigin.Begin);
                bw.Write((short)stream.Length);

                byte[] data = stream.ToArray();

                for (int i = 0; i < users.Length; i++) {
                    User user = users[i];

                    user.SendMessage(data);
                }
            }
        }

        public static string ByteArrayToString(byte[] ba) {
            return BitConverter.ToString(ba).Replace("-", " ");
        }
    }
}
