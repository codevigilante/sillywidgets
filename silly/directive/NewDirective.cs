using System;
using System.Collections.Generic;
using System.IO;
//using Newtonsoft.Json;

namespace silly
{
    public class NewDirective : SillyDirective
    {
        public string DefaultSiteName { get; set; }

        public NewDirective(string id) : base(id, "creates a new [-name] directory and populates it with the default silly structure.")
        {
            DefaultSiteName = "newsite";

            base.AddOption(new DirectiveOption("-name", "name of the site to create", new List<string>() { "[site-name]" }));
        }

        public override void Execute(string[] args)
        {
            string dirName = DefaultSiteName;

            if (args.Length > 1)
            {
                dirName = args[1];
            }

            Directory.CreateDirectory(dirName);
        }
    }
}