using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace SillyWidgets.Samples
{
    public class Root : SillyController
    {
        public Root()
            : base()
        {

        }

        public ISillyView Index(ISillyContext context)
        {
            SillyView home = base.LoadView(@"HelloWorld/Views/index.html");
            
            // view.Bind("name", value);
            // view.Bind("name2", value2);

            return(home);
        }
        
    }
}