namespace Estrol.X3Jam.Server.CData {
    public enum ClientPacket : ushort {
        Disconnect = 0xfff0,
        Login = 0x03ef,
        Connect = 0x03f1,
        PlanetConnect = 0x03f3,
        PlanetLogin = 0x03e8,
        Channel = 0x03ea,
        OJNList = 0x0fbe,
        EnterCH = 0x03ec,
        GetRoom = 0x07d2,
        GetChar = 0x07d0,
        Timest = 0x13A4,
        ClientList = 0x07e8,
        TCPPing = 0x1771,
        LeaveCH = 0x07e5,
        CreateRoom = 0x07d4,
        LeaveRoom = 0x0bbd,
        RoomBGChange = 0x0fa2,
        SetSongID = 0x0fa0,
        RoomInit = 0x0fa4,
        ClientMSG = 0x07dc,
        ClientMSG2 = 0x0bc3,
        JoinRoom = 0x0bba
    }
}
