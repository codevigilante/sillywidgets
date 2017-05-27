using System;
using System.IO;
using System.Collections.Generic;

namespace silly
{
    public class DeployDirective : SillyDirective
    {
        public bool Clean { get; private set; }

        public DeployDirective(string id) : base(id, "package static site for deployment")
        {
            Clean = false;
            base.HelpString = "Compile and package the silly site for deployment";
        }

        protected override void Run()
        {
            SillySite site = new SillySite(DefaultLocation);

            site.Compile();

            site.Deploy();
        }
    }
}