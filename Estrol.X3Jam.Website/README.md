# Estrol HTTPServer library
Because `System.Net.HttpServer` is requiring some kind `whitelist` that only be used when using Administrator mode \
This module trying to achived HTTP server without Administrator's permissions

Written for X3-JAM Website module.

## How?
Main gateway on HTTP Server was in `HTTPSocketServer.cs` \
which Implement `HTTPSocketServer` class. 
```csharp
public HTTPSocketServer(int port);
```

To get client request and parse it you need listen HTTPSocketServer#OnServerData event
```csharp
HTTPSocketServer.Data(object o, HTTPClient wb);
```

`HTTPClient` object data
```csharp
public class HTTPClient {
    public const int MAX_BUFFER_SIZE = 10000;

    // Internal
    public HTTPSocketServer Main { set; get; }
    public Socket ClientSocket { set; get; }
    public byte[] ResponseData { set; get; }
    public int DataLength { set; get; }

    // Public
    public HTTPHeader Headers { set; get; }
    public Uri URL { set; get; }
    public byte[] BodyArray { set; get; }
    public string BodyString => Encoding.ASCII.GetString(BodyArray);

    // Method
    public void Send(string data, int statusCode = 200, string contentType = "text/plain");

```
