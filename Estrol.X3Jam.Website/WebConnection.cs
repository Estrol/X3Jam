using System;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace Estrol.X3Jam.Website {
    public class WebConnection {
        private TcpClient tc;
        private NetworkStream ns;

        public HTTPHeader header;
        public byte[] data;
        public Uri url;
        public string sData;
        public string sBody;

        public bool forwarded;

        public WebConnection(TcpClient tc, NetworkStream ns) {
            forwarded = true;

            this.tc = tc;
            this.ns = ns;
            data = new byte[2000];
            ns.Read(data, 0, 2000);
            int actualData = ReadDataUntilNull(data);
            byte[] aData = new byte[actualData];
            Buffer.BlockCopy(this.data, 0, aData, 0, actualData);
            sData = Encoding.ASCII.GetString(aData, 0, actualData);

            string[] RawHeaderSeperator = new string[] { "\r\n" };
            string[] HeaderData = sData.Split(RawHeaderSeperator, StringSplitOptions.RemoveEmptyEntries);

            string[] data_1 = HeaderData[0].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            header = new HTTPHeader() {
                Method = (HTTPMethod)Enum.Parse(typeof(HTTPMethod), data_1[0], true),
                URLParams = data_1[1],
                HTTPVersion = data_1[2]
            };

            if (header.Method == HTTPMethod.OPTIONS) {
                Send_Cors();
                forwarded = false;
            }

            int data_post_index = 0;

            for (int i = 0; i < HeaderData.Length - 1; i++) {
                string hData = HeaderData[i + 1];
                string[] SpaceSeperator = new string[] { " " };
                string[] spData = hData.Split(SpaceSeperator, StringSplitOptions.RemoveEmptyEntries);
                if (spData[0] == "\0" || spData[0].EndsWith("\0")) break;
                string rData = spData[1];
                data_post_index++;

                switch(spData[0].Replace(":", string.Empty)) {
                    case "Host": {
                        header.Host = rData;

                        string uriData = rData;
                        if (!uriData.Contains("http")) {
                            uriData = "http://" + uriData;
                        }

                        header.URLFull = new Uri(uriData + header.URLParams);
                        break;
                    }

                    case "Connection": {
                        header.Connection = rData;
                        break;
                    }

                    case "DNT": {
                        if (rData == "null") {
                            header.DNT = 0;
                        } else {
                            header.DNT = int.Parse(rData);
                        }

                        break;
                    }

                    case "Upgrade-Insecure-Requests": {
                        header.UIR = int.Parse(rData);
                        break;
                    }

                    case "User-Agent": {
                        header.UserAgent = rData;
                        break;
                    }

                    case "Accept": {
                        header.Accept = rData;
                        break;
                    }
                }
            }

            if (header.Method == HTTPMethod.POST) {
                sBody = HeaderData[HeaderData.Length - 1];
            }
        }

        public void Send(string sData, int status = 200, string mime = "text/plain") {
            StringBuilder header = new StringBuilder();
            string resHeader = string.Format("{0} {1}\r\n", this.header.HTTPVersion, HTTPStatus.GetResponseStatus(status));
            header.Append(resHeader);
            string resMime = string.Format("Content-Type: {0}\r\n", mime);
            header.Append(resMime);
            string resSize = string.Format("Content-Length: {0}\r\n", sData.Length);
            header.Append(resSize);
            header.Append(Environment.NewLine);
            header.Append(sData);
            header.Append(Environment.NewLine);

            byte[] hData = Encoding.ASCII.GetBytes(header.ToString());

            ns.Write(hData, 0, hData.Length);
            ns.Flush();

            //Console.WriteLine("[DEBUG] Sending HTTP response!");
            tc.Close();
        }

        public void Send_Cors() {
            StringBuilder header = new StringBuilder();
            string resHeader = string.Format("{0} {1}\r\n", this.header.HTTPVersion, HTTPStatus.GetResponseStatus(204));
            header.Append(resHeader);
            string res1 = string.Format("Connection: Keep-Alive");
            header.Append(res1);
            string res2 = string.Format("Access-Control-Allow-Origin: *");
            header.Append(res2);
            string res3 = string.Format("Access-Control-Allow-Methods: POST");
            header.Append(res3);
            string res4 = string.Format("Access-Control-Max-Age: 3600");
            header.Append(res4);
            header.Append(Environment.NewLine);

            byte[] hData = Encoding.ASCII.GetBytes(header.ToString());

            ns.Write(hData, 0, hData.Length);
            ns.Flush();

            //Console.WriteLine("[DEBUG] Sending HTTP response!");
            tc.Close();
        }

        private int ReadDataUntilNull(byte[] bData) {
            int count = 0;

            for (int i = 0; i < bData.Length; i++) {
                if (bData[i] == 0x00) {
                    break;
                }

                count++;
            }

            return count + 1;
        }
    }
}
