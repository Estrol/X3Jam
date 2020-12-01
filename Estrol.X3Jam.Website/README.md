# Estrol HTTPServer library
Because `System.Net.HttpServer` is requiring some kind `whitelist` that only be used when using Administrator mode \
This module trying to achived HTTP server without Administrator's permissions

Written for X3-JAM Website module.

## How?
Main gateway on HTTP Server was in `HTTPServer.cs` \
which Implement `HTTPServer` class. 
```csharp
public HTTPServer(IPAddress ip, int port);
```

To get client request and parse it you need listen HTTPServer#Data event
```csharp
HTTPServer.Data(object o, WebConnection wb);
```

`WebConnection` object data
```csharp
public class WebConnection {
    public TcpClient tc;
    public NetworkStream ns;
    public byte[] data = new byte[2000];
    public Uri url;

    public WebConnection(TcpClient tc, NetworkStream ns);
    public void Send(object data);
}
```
