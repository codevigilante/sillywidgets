using SillyWidgets;
using SillyWidgets.Gizmos;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SillyDiagnostic
{
    public class Diagnostic : SillyController
    {
        public Diagnostic()
            : base()
        {

        }

        public ISillyView Index(ISillyContext context)
        {
            Stopwatch timer1 = new Stopwatch();
            timer1.Start();

            SillyView view = new SillyView();
            view.Content = "<html><head><title></title></head><body><h1>Hello World</h1></body></html>";

            timer1.Stop();

            // HtmlGizmo test

            Stopwatch timer2 = new Stopwatch();
            timer2.Start();

            SillyView home = new SillyView();
            Task<bool> result = home.LoadS3Async("sillywidgets.com", "diagnostic/diag.html", Amazon.RegionEndpoint.USWest1);

            result.Wait();

            timer2.Stop();
            
            home.Bind("content", "view.Content");
            home.Bind("loadViewLocal", timer1.Elapsed.TotalMilliseconds + "ms");
            home.Bind("s3source", "diagnostic/diag.html");
            home.Bind("loadViewValue", timer2.Elapsed.TotalMilliseconds + "ms");
            
            return(home);
        }
        
    }
}