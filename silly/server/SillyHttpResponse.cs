using System;
using System.Text;
using System.Net.Sockets;

namespace silly
{
    public class SillyHttpResponse
    {
        public enum MimeType { TextHtml, TextCss, ApplicationJavascript }
        public enum ResponseCodes { OK, NotFound, ServerError }
        public MimeType Mime { get; set; }
        public ResponseCodes Code { get; set; }
        public string Version { get; set; }
        public string Payload { get; set; }

        public SillyHttpResponse(MimeType mime, ResponseCodes code, string payload)
        {
            Mime = mime;
            Version = "HTTP/1.1";
            Code = code;
            Payload = payload;
        }
        
        private void Send(Socket socket, Byte[] data)
        {
            if (socket == null || !socket.Connected)
            {
                return;
            }

            try
            {
                int bytesSent = socket.Send(data, data.Length, 0);

                if (bytesSent == -1)
                {
                    throw new Exception("Something went wrong sending the response");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error responding: " + ex.Message);
            }
        }

        public void SendResponse(Socket socket)
        {
            SendHeader(socket, Payload.Length);

            Byte[] data = Encoding.ASCII.GetBytes(Payload);

            Send(socket, data);
        }

        private void SendHeader(Socket socket, int contentSize)
        {
            if (socket == null)
            {
                return;
            }

            string buffer = Version + " " + Code + "\r\n";
            buffer += "Server: silly server v0.1\r\n";
            buffer += "Content-Type: " + MimeToString() + "\r\n";
            buffer += "Accept-Ranges: bytes\r\n";
            buffer += "Content-Length: " + contentSize + "\r\n\r\n";

            Byte[] data = Encoding.ASCII.GetBytes(buffer);

            Send(socket, data);
        }

        private string MimeToString()
        {
            switch(Mime)
            {
                case MimeType.TextCss:
                    return("text/css");
                case MimeType.ApplicationJavascript:
                    return("application/javascript");
                case MimeType.TextHtml:
                default:
                    return("text/html");
            }
        }
    }
}