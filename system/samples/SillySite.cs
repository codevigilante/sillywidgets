using System;
using System.Collections.Generic;

namespace SillyWidgets.Samples
{
    public class SillySite : SillyProxyHandler
    {
        public SillySite()
            : base()
        {
            Console.WriteLine("SillySite");
            
            SillyRouteMap.SetAvailableControllers(new List<AbstractSillyController>()
            {
                new Root()
            });

            SillyRouteMap.MapRoute("root", "/", new SillyRouteDetails("Root", "Index"));
        }
    }
}
