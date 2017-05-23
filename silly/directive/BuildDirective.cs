using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace silly
{
    public class BuildDirective : SillyDirective
    {
        public BuildDirective(string id) : base(id, "compile and start server for development")
        {
            base.HelpString = "Compiles the silly site and starts a development HTTP server";
        }

        protected override void Run()
        {
            SillySite site = new SillySite(DefaultLocation);

            site.Setup();

            SillySiteServer buildServer = new SillySiteServer(site);

            Task server = buildServer.Start();

            server.Wait();
        }
    }
}