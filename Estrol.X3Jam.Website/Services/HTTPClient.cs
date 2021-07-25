using System;
using System.Text;
using System.Net.Sockets;

namespace Estrol.X3Jam.Website.Services {
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

        // Methods
        public void Send(string data, int statusCode = 200, string contentType = "text/plain") {
            string ResponseText = $"{Headers.HTTPVersion} {HTTPStatus.GetResponseStatus(statusCode)}\r\n";
            ResponseText += $"Server: Estrol's dotnet HTTPSocketServer for X3-JAM\r\n";
            ResponseText += $"Access-Control-Allow-Origin: *\r\n";
            ResponseText += "Access-Control-Allow-Methods: GET,HEAD,POST\r\n";
            ResponseText += "Access-Control-Allow-Headers: Content-Type, Accept, Authorization\r\n";
            ResponseText += "Access-Control-Max-Age: 68400\r\n";
            ResponseText += "Vary: Access-Control-Request-Headers\r\n";
            ResponseText += $"Content-Type: {contentType}\r\n";
            ResponseText += $"Content-Length: {data.Length}\r\n";
            ResponseText += $"Connection: Close\r\n";

            if (Headers.Method == HTTPMethod.HEAD) {
                byte[] ResData = Encoding.ASCII.GetBytes(ResponseText);
                Main.Send(this, ResData, ResData.Length);
            } else {
                ResponseText += "\r\n";
                ResponseText += data;

                byte[] ResData = Encoding.ASCII.GetBytes(ResponseText);
                Main.Send(this, ResData, ResData.Length);
            }
        }
    }
}
