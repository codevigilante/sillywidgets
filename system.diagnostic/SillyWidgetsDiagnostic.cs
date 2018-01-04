using System;
using System.Diagnostics;
using Amazon.Lambda.Core;
using SillyWidgets;

namespace SillyDiagnostic
{
    // Lambda method signature: system.diagnostic::SillyDiagnostic.SillyWidgetsDiagnostic::Handle
    public class SillyWidgetsDiagnostic : SillyProxyApplication
    {
        public SillyWidgetsDiagnostic()
            : base()
        {
            base.RegisterController("diagnostic", new Diagnostic());

            GET("root", "/", "diagnostic", "Index");
        }

        public override SillyProxyResponse Handle(SillyProxyRequest input, ILambdaContext lambdaContext)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            SillyProxyResponse response = base.Handle(input, lambdaContext);

            timer.Stop();

            response.body += "<p>Total Lambda Time -> " + timer.Elapsed.TotalMilliseconds + "ms</p>";

            return(response);
        }

        [SillyUrlHandler(IsIndex = true)] // /[penis]
        public ISillyView Index(ISillyContext context, string penis = "")
        {
            return(null);
        }

        [SillyUrlHandler()] // /about
        public ISillyView About(ISillyContext context)
        {
            return(null);
        }

        [SillyUrlHandler("/blog")] // /blog/posts/{num}
        public ISillyView Posts(ISillyContext context, int num)
        {
            return(null);
        }
    }
}
