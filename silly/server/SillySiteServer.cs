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

            WebRoot = new DirectoryInfo(Site.RootDir + "/" + Site.Options.Routes);

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

                        if (request.Ignore)
                        {
                            consoleStr += "-> Empty request received, ignoring";

                            continue;
                        }

                        if (request.IsInvalid)
                        {
                            consoleStr += "-> " + request.InvalidReason + " ";

                            continue;
                        }

                        consoleStr += request.Method + " " + request.URL + " " + request.Version + " ";

                        string normalizedRequest = WebRoot.FullName + request.URL.Trim().ToLower();

                        if (Directory.Exists(normalizedRequest))
                        {
                            normalizedRequest += "/index.html";
                        }

                        FileInfo requestedFile = new FileInfo(normalizedRequest);

                        if (requestedFile.Exists)
                        {
                            if (String.Compare(requestedFile.Extension, ".html", true) == 0)
                            {
                                SillyRoute route = new SillyRoute(requestedFile, WebRoot);

                                response.Payload = route.Resolve();
                            }
                            else
                            {
                                SillyResource resource = new SillyResource(requestedFile);

                                // this won't work for images, Payload needs to be a byte[]
                                response.Payload = ASCIIEncoding.ASCII.GetString(resource.Contents());
                            }

                            consoleStr += "-> RESOURCE " + request.URL + " RESOLVED ";
                        }
                        else
                        {
                            response.Code = SillyHttpResponse.ResponseCodes.NotFound;
                            response.Payload = Error404;

                            throw new Exception(" RESOURCE " + request.URL + " DOES NOT EXIST ");
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    consoleStr += "-> Exception: " + ex.Message + " ";
                }
                finally
                {
                    response.SendResponse(socket);
                    
                    consoleStr += response.Code;

                    Console.WriteLine(consoleStr);
                    Console.ResetColor();

                    if (socket != null && socket.Connected)
                    {
                        socket.Dispose();
                    }
                }
            }
        }
    }
}