using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using SillyWidgets;

namespace SillyDiagnostic
{
    public class Home : SillyPage
    {
        public Home()
            : base()
        {

        }

        public override bool Accept(ISillyContext context, string[] urlParams)
        {
            Stopwatch dynamoTimer = new Stopwatch();
            dynamoTimer.Start();
            Task<Document> data = base.DynamoGetItemAsync(Amazon.RegionEndpoint.USWest1, "sillywidgets", "codevigilante@gmail.com");
            data.Wait();
            dynamoTimer.Stop();
            
            Stopwatch homeTimer = new Stopwatch();
            homeTimer.Start();
            Task<bool> homeResult = base.LoadS3Async("sillywidgets.com", "diagnostic/diag.html", Amazon.RegionEndpoint.USWest1);
            homeResult.Wait();
            homeTimer.Stop();

            Stopwatch headerTimer = new Stopwatch();
            headerTimer.Start();
            SillyView header = new SillyView();
            Task<bool> headerResult = header.LoadS3Async("sillywidgets.com", "diagnostic/header.html", Amazon.RegionEndpoint.USWest1);
            headerResult.Wait();
            headerTimer.Stop();

            Stopwatch contentTimer = new Stopwatch();
            contentTimer.Start();
            SillyView content = new SillyView();
            Task<bool> contentResult = content.LoadS3Async("sillywidgets.com", "diagnostic/content.html", Amazon.RegionEndpoint.USWest1);
            contentResult.Wait();
            contentTimer.Stop();

            Bind(data.Result);
            Bind("content", content);
            Bind("header", header);
            Bind("dynamoGetValue", dynamoTimer.Elapsed.TotalMilliseconds + "ms");
            Bind("s3source", "diagnostic/diag.html");
            Bind("loadViewValue", homeTimer.Elapsed.TotalMilliseconds + "ms");
            Bind("s3header", "diagnostic/header.html");
            Bind("loadHeaderValue", headerTimer.Elapsed.TotalMilliseconds + "ms");
            Bind("s3content", "diagnostic/content.html");
            Bind("loadContentValue", contentTimer.Elapsed.TotalMilliseconds + "ms");

            return(true);
        }
    }
}