using System;
using SillyWidgets;

namespace SillyWidgetsLambda
{
    public class SillyWidgetsProxy : SillyProxyApplication
    {
        public SillyWidgetsProxy()
            : base()
        {
            base.RegisterController("root", typeof(Root));

            GET("root", "/", "root", "Index");
        }
    }
}
