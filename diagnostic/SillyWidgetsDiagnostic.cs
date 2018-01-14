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
            : base(new Home())
        {
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
    }
}
