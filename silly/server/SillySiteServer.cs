using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using System.IO;

namespace silly
{
    public class SillySiteServer
    {
        public SillySite Site { get; private set; }
        public IPAddress IP { get; private set; }
        public DirectoryInfo WebRoot { get; private set; }

        private string TestPayload = "<html><body><h1>PLACEHOLDER</h1></body></html>";
        private string Error404 = "<html><body><p>404 - Not Found</p></body></html>";

        public SillySiteServer(SillySite site, bool verbose = true)
        {
            this.Site = site;
            IP = IPAddress.Loopback;

            WebRoot = new DirectoryInfo(Site.RootDir + "/" + Site.Config.Routes);

            if (!WebRoot.Exists)
            {
                throw new Exception("Root directory " + WebRoot.FullName + " does not exist");
            }
        }

        private TcpListener Listener = null;

        public async Task Start()
        {
            if (Site == null)
            {
                throw new Exception("Cannot start server, 'Site' is null");
            }

            Console.WriteLine("Starting build server on " + IP.ToString() + ":" + Site.Options.LocalPort);

            Listener = new TcpListener(IP, Site.Options.LocalPort);

            Listener.Start();

            while(true)
            {
                string consoleStr = string.Empty;
                Socket socket = null;
                SillyHttpResponse response = new SillyHttpResponse(SillyHttpResponse.MimeType.TextHtml, SillyHttpResponse.ResponseCodes.OK, TestPayload);

                try
                {
                    socket = await Listener.AcceptSocketAsync();

                    if (socket == null)
                    {
                        throw new Exception("Incoming connection is null");
                    }
                    
                    consoleStr += socket.RemoteEndPoint + " -- [" + DateTime.Now.ToString() + "] ";

                    if (socket.Connected)
                    {
                        Byte[] receiveData = new Byte[1024];

                        socket.Receive(receiveData, receiveData.Length, 0);

                        string buffer = Encoding.ASCII.GetString(receiveData);

                        SillyHttpRequestParser request = new SillyHttpRequestParser(buffer);

                        if (request.IsInvalid)
                        {
                            Console.WriteLine(request.InvalidReason);

                            continue;
                        }

                        consoleStr += request.Method + " " + request.URL + " " + request.Version + " ";

                        string dir = WebRoot.FullName + request.URL;
                        
                        if(!request.RequestIsFile)
                        {
                            dir += "index.html";
                        }

                        FileInfo requestPayload = new FileInfo(dir);

                        if (!requestPayload.Exists)
                        {
                            response.Code = SillyHttpResponse.ResponseCodes.NotFound;
                            response.Payload = Error404;

                            throw new Exception("Requested resource " + request.URL + " does not exist");
                        }

                        consoleStr += "-> Resolved: " + requestPayload.FullName + " ";
                    }
                }
                catch(Exception ex)
                {
                    consoleStr += "-> Exception: " + ex.Message + " ";
                }
                finally
                {
                    response.SendResponse(socket);
                    
                    consoleStr += response.Code;

                    Console.WriteLine(consoleStr);

                    if (socket != null && socket.Connected)
                    {
                        socket.Dispose();
                    }
                }
            }
        }
    }
}