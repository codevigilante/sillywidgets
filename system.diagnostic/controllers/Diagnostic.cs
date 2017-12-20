using SillyWidgets;
using SillyWidgets.Gizmos;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Amazon.DynamoDBv2.DocumentModel;

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
            Stopwatch timer2 = new Stopwatch();
            Stopwatch timer3 = new Stopwatch();
            timer3.Start();

            Task<Document> data = base.DynamoGetItemAsync(Amazon.RegionEndpoint.USWest1, "sillywidgets", "codevigilante@gmail.com");
            
            timer2.Start();

            SillyView home = new SillyView();
            SillyView content = new SillyView();
            SillyView header = new SillyView();
            Task<bool> result = home.LoadS3Async("sillywidgets.com", "diagnostic/diag.html", Amazon.RegionEndpoint.USWest1);
            Task<bool> result2 = content.LoadS3Async("sillywidgets.com", "diagnostic/content.html", Amazon.RegionEndpoint.USWest1);
            Task<bool> result3 = header.LoadS3Async("sillywidgets.com", "diagnostic/header.html", Amazon.RegionEndpoint.USWest1);

            data.Wait();
            timer3.Stop();
            result.Wait();
            result2.Wait();
            result3.Wait();

            timer2.Stop();
            
            home.Bind(data.Result);
            home.Bind("content", content);
            home.Bind("header", header);
            home.Bind("dynamoGetValue", timer3.Elapsed.TotalMilliseconds + "ms");
            home.Bind("s3source", "diagnostic/diag.html");
            home.Bind("loadViewValue", timer2.Elapsed.TotalMilliseconds + "ms");
                        
            return(home);
        }
        
    }
}