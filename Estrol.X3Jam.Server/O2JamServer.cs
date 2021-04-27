﻿using System;
using System.Linq;
using System.Text;

using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Server.CNetwork;
using Estrol.X3Jam.Server.CHandler;
using Estrol.X3Jam.Server.CManager;
using Estrol.X3Jam.Server.CUtility;

namespace Estrol.X3Jam.Server {
    public class O2JamServer {
        public Configuration Config;
        public DataNetwork Database;
        public ChanManager ChannelManager;
        public TCPServer Server;

        public string[] StaticOpcode = {
            "Connect", "Login",
            "PlanetConnect", "PlanetLogin",
            "Disconnect"
        };

        public O2JamServer() {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public void Intialize() {
            Config = new();
            ChannelManager = new(this);
            Database = new();
            Database.Intialize();

            short port = short.Parse(Config.Get("GamePort"));

            Server = new(port);
            Server.OnServerMessage += TCPMessage;
            Server.Start();
        }

        public void TCPMessage(object o, Client client) {
            CMessageManager cMessage = new(client, client.Buffer);
            if (cMessage.IsFailed) {
                var Handler = new CDisconnect(client);
                Handler.Handle();
                return;
            }

            for (int i = 0; i < cMessage.packets.Count; i++) {
                CMessage message = cMessage.packets[i];
                client.Message = message;
                client.Main = this;

                string name = Enum.GetName(typeof(ClientPacket), message.opcode);
                if (name != null && !StaticOpcode.Contains(name)) {
                    if (client.UserInfo == null) {
                        client.m_socket.Close();

                        return;
                    }
                }

                CBase handler;
                switch (message.opcode) {
                    case ClientPacket.Connect:
                        handler = new CConnect(client);
                        handler.Handle();
                        break;

                    case ClientPacket.PlanetConnect:
                        handler = new CPlanetConnect(client);
                        handler.Handle();
                        break;

                    case ClientPacket.Login:
                        handler = new CLogin(client);
                        handler.Handle();
                        break;

                    case ClientPacket.PlanetLogin:
                        handler = new CPlanetLogin(client);
                        handler.Handle();
                        break;

                    case ClientPacket.Channel:
                        handler = new CChannel(client);
                        handler.Handle();
                        break;

                    case ClientPacket.EnterCH:
                        handler = new CEnterCH(client);
                        handler.Handle();
                        break;

                    case ClientPacket.LeaveCH:
                        handler = new CLeaveCH(client);
                        handler.Handle();
                        break;

                    case ClientPacket.SetSongID:
                        handler = new CRoomMusicID(client);
                        handler.Handle();
                        break;

                    case ClientPacket.CreateRoom:
                        handler = new CCreateRoom(client);
                        handler.Handle();
                        break;

                    case ClientPacket.LeaveRoom:
                        handler = new CLeaveRoom(client);
                        handler.Handle();
                        break;

                    case ClientPacket.RoomPlrColorChange:
                        handler = new CPlayerColorChange(client);
                        handler.Handle();
                        break;

                    case ClientPacket.JoinRoom:
                        handler = new CRoomJoin(client);
                        handler.Handle();
                        break;

                    case ClientPacket.GetRoom:
                        handler = new CRoomList(client);
                        handler.Handle();
                        break;

                    case ClientPacket.GetChar:
                        handler = new CCharacter(client);
                        handler.Handle();
                        break;

                    case ClientPacket.Timest:
                        handler = new CTimest(client);
                        handler.Handle();
                        break;

                    case ClientPacket.ClientList:
                        handler = new CMusicList(client);
                        handler.Handle();
                        break;

                    case ClientPacket.RoomRingChange:
                        handler = new CRoomRingChange(client);
                        handler.Handle();
                        break;

                    case ClientPacket.TCPPing:
                        handler = new CTcpPing(client);
                        handler.Handle();
                        break;

                    case ClientPacket.ListChat:
                        handler = new CLobbyMessage(client);
                        handler.Handle();
                        break;

                    case ClientPacket.RoomChat:
                        handler = new CRoomMessage(client);
                        handler.Handle();
                        break;

                    case ClientPacket.OJNList:
                        handler = new COJNList(client);
                        handler.Handle();
                        break;

                    case ClientPacket.Disconnect:
                        handler = new CDisconnect(client);
                        handler.Handle();
                        break;

                    case ClientPacket.RoomBGChange:
                        handler = new CRoomBGChange(client);
                        handler.Handle();
                        break;

                    case ClientPacket.GameStart:
                        handler = new CGameStart(client);
                        handler.Handle();
                        break;

                    case ClientPacket.GamePing:
                        handler = new CGamePing(client);
                        handler.Handle();
                        break;

                    case ClientPacket.ScoreSubmit:
                        handler = new CRoomFinish(client);
                        handler.Handle();
                        break;

                    case ClientPacket.GameInit:
                        handler = new CGameReady(client);
                        handler.Handle();
                        break;

                    case ClientPacket.GameQuit:
                        handler = new CInGameExit(client);
                        handler.Handle();
                        break;

                    case ClientPacket.GameReady:
                        handler = new CRoomUserReady(client);
                        handler.Handle();
                        break;

                    case ClientPacket.Tutorial1:
                    case ClientPacket.Tutorial2:
                        // Ignored
                        break;

                    default: {
                        byte[] opcode = new byte[2];
                        Buffer.BlockCopy(message.data, 0, opcode, 0, 2);

                        string msg = null;
                        if (message.data.Length > 2) {
                            byte[] mData = new byte[message.data.Length - 2];
                            Buffer.BlockCopy(message.data, 2, mData, 0, message.data.Length - 2);

                            msg = ToHexString(mData);
                        }

                        Log.Write("Unhandled opcode");
                        Log.Write("Code: {0}", message._opcode.ToString("X4"));
                        Log.Write("Data: {0}", msg ?? "Empty");

                        break;
                    }
                }

                if (message.opcode == ClientPacket.Disconnect) return;
            }
            
            client.Read();
        }

        public static string ToHexString(byte[] bData) {
            return BitConverter.ToString(bData).Replace("-", " ");
        }
    }
}
