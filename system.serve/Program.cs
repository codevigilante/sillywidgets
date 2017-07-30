using System;
using System.Threading.Tasks;
using SillyWidgets;
using SillyWidgets.Utilities;

namespace system.serve
{
    class Program
    {
        static void Main(string[] args)
        {
            SillySite site = new SillySite();

            SillySiteServer testServer = new SillySiteServer(site);

            Task server = testServer.Start();

            server.Wait(); 
        }
    }
}
