using System;
using System.Threading.Tasks;
using SillyDiagnostic;
using SillyWidgets.Utilities.Server;

namespace local
{
    class Program
    {
        static void Main(string[] args)
        {
            SillyWidgetsDiagnostic diagnostic = new SillyWidgetsDiagnostic();
            SillySiteServer testServer = new SillySiteServer(diagnostic);

            Task server = testServer.Start();

            server.Wait();
        }
    }
}
