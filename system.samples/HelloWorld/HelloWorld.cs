using System;

namespace SillyWidgets.Samples
{
    public class HelloWorld : SillyProxyApplication
    {
        public HelloWorld()
            : base()
        {
            base.RegisterController("root", typeof(Root));

            GET("root", "/", "root", "Index");
        }
    }
}
