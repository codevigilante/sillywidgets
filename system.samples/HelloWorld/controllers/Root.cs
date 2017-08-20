using System;
using System.Collections.Generic;

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
            // base.LoadDataAsync() : Data
            // Home view = LoadViewAsync<Home>();
            // Data.wait();
            // view.wait();
            // view.BindModel(Data);
            Home content = new Home();

            content.Content = "<h1>Hello World</h1><h3>I'm Root.Index</h3>";

            return(content);
        }
        
    }

    public class Home : SillyView
    {
        /*public string Title { get; set; }
        public string Sub { get; set; }
        public object Sample { get; set; }*/

        public Home()
            //: base("views/home.html", new List<string> { "widgets/sample.html" })
        {
            /*Title = "This is a title";
            Sub = "This is a sub";
            Sample = null;*/
        }

        public void BindModel(object Data)
        {
            // base.BindText("title", Data.title);
            
        }
    }
}