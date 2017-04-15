using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace silly
{
    public class CompileDirective : SillyDirective
    {
        public DirectoryInfo DefaultLocation { get; private set; }

        public CompileDirective(string id)
            : base(id, "consumes the site.json and checks for errors")
        {
            base.AddOption(new DirectiveOption("-location",  "compiles the ",
                                                new List<string>() { "<path/to/site/directory>" }));

            DefaultLocation = new DirectoryInfo("./");
        }

        public override void Execute(string[] args)
        {
            Console.Write("Checking args...");

            ParseArgs(args);

            if (!DefaultLocation.Exists)
            {
                throw new Exception ("The location provided, " + DefaultLocation.FullName + ", doesn't seem to exist.");
            }

            Console.WriteLine("done");

            SillySite site = new SillySite(DefaultLocation);

            site.Compile();
        }

        private void ParseArgs(string[] args)
        {
            for(int i = 0; i < args.Length; ++i)
            {
                string arg = args[i].ToLower();

                if (String.Compare(arg, "-location", true) == 0)
                {
                    ++i;
                    DefaultLocation = (i < args.Length) ? new DirectoryInfo(args[i]) : throw new Exception("Expected <path/to/silly/site> for -location");
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