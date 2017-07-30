using System;

namespace SillyWidgets.Samples
{
    public class Root : AbstractSillyController
    {
        public Root()
            : base()
        {

        }

        public ISillyContent Index(ISillyContext context)
        {
            SillyContent content = new SillyContent();

            content.Content = "<h1>Root.Index()</h1>";

            return(content);
        }
        
    }
}