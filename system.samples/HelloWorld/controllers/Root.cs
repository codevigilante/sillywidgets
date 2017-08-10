using System;

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
            SillyView content = new SillyView();

            content.Content = "<h1>Hello World</h1><h3>I'm Root.Index</h3>";

            return(content);
        }
        
    }
}