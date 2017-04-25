using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace silly
{
    public class BuildDirective : SillyDirective
    {
        public DirectoryInfo DefaultLocation { get; private set; }

        public BuildDirective(string id) : base(id, "renders the static content")
        {
            base.AddOption(new DirectiveOption("-location",  "renders the site located at the path",
                                                new List<string>() { "<path/to/site/directory>" }));

            DefaultLocation = new DirectoryInfo("./");
        }

        public override void Execute(string[] args)
        {
            Console.Write("Parsing args...");

            ParseArgs(args);

            if (!DefaultLocation.Exists)
            {
                throw new Exception ("The location provided, " + DefaultLocation.FullName + ", doesn't seem to exist.");
            }

            Console.WriteLine("done");

            SillySite site = new SillySite(DefaultLocation);

            site.Setup();

            SillySiteServer buildServer = new SillySiteServer(site);

            Task server = buildServer.Start();

            server.Wait();
        }

        private void ParseArgs(string[] args)
        {
            for(int i = 0; i < args.Length; ++i)
            {
                string arg = args[i].ToLower();

                if (String.Compare(arg, "-location", true) == 0)
                {
                    ++i;

                    DefaultLocation = (i < args.Length) ? new DirectoryInfo(args[i]) : throw new Exception("Expected <path/to/site.json> for -location");
                }
                else if (String.Compare(arg, "silly", true) == 0 ||
                         String.Compare(arg, Command, true) == 0)
                {
                }
                else
                {
                    throw new Exception ("Unexpected build option: " + arg);
                }
            }
        }
    }
}