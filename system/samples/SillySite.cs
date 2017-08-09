using System;

namespace SillyWidgets.Samples
{
    public class SillySite : SillyProxyApplication
    {
        public SillySite()
            : base()
        {
            base.RegisterController("root", typeof(Root));

            GET("root", "/", "root", "Index");
        }
    }
}
