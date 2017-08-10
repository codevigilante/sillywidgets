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

            content.Content = "<h1>Silly Site</h1><h3>Root.Index</h3>";

            return(content);
        }
        
    }
}