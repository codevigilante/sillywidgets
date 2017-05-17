using System;
using System.Collections.Generic;
using System.IO;

namespace silly
{
    public class CompileDirective : SillyDirective
    {
        public CompileDirective(string id)
            : base(id, "check the site for errors")
        {
            base.HelpString = "Checks the silly site for errors, ensuring widgets referenced exist and all assets are accounted for";
        }

        protected override void Run()
        {
            SillySite site = new SillySite(DefaultLocation);

            site.Compile();
        }
    }
}