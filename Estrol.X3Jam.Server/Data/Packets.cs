namespace Estrol.X3Jam.Server.Data {
    public enum Packets : ushort {
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
        Payload = 0x07e8,
        TCPPing = 0x1771,
        LeaveCH = 0x07e5,
        CreateRoom = 0x07d4,
        SetSongID = 0x0fa0,
    }
}
