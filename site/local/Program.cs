using System;
using System.Threading.Tasks;
using SillyWidgets;
using SillyWidgets.Utilities.Server;
using SillyWidgetsLambda;

namespace local
{
    class Program
    {
        static void Main(string[] args)
        {
            SillyProxyApplication site = new SillyWidgetsProxy();
            SillySiteServer testServer = new SillySiteServer(site);

            Task server = testServer.Start();

            server.Wait(); 
        }
    }
}
