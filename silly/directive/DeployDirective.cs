using System;
using System.IO;
using System.Collections.Generic;

namespace silly
{
    public class DeployDirective : SillyDirective
    {
        public DirectoryInfo DefaultLocation { get; private set; }
        public bool Clean { get; private set; }

        public DeployDirective(string id) : base(id, "makes content ready to deploy")
        {
            base.AddOption(new DirectiveOption("-location",  "renders the site located at the path",
                                                new List<string>() { "<path/to/site/directory>" }));
            base.AddOption(new DirectiveOption("-clean", "cleans out the deployment directory"));

            DefaultLocation = new DirectoryInfo("./");
            Clean = false;
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

            site.Compile();

            site.Deploy();
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
                else if (String.Compare(arg, "-clean", true) == 0)
                {
                    Clean = true;
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