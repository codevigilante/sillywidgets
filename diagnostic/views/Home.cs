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

            Stopwatch listTimer = new Stopwatch();
            listTimer.Start();
            SillyListWidget list = new SillyListWidget();
            Bind("groceryList", list);

            for(int i = 0; i < 10; ++i)
            {
               SillyListItem item = new SillyListItem();
               item.Bind("item", "Hello " + i);
               
               list.AddItem(item);
            }

            listTimer.Stop();
            SillyListItem bindTime = new SillyListItem();
            bindTime.Bind("item", "10 item bind time: " + listTimer.Elapsed.TotalMilliseconds + "ms");
            list.AddItem(bindTime);

            Bind(data.Result);
            Bind("content", content);
            Bind("header", header);
            content.Bind("dynamoGetValue", dynamoTimer.Elapsed.TotalMilliseconds + "ms");
            content.Bind("s3source", "diagnostic/diag.html");
            content.Bind("loadViewValue", homeTimer.Elapsed.TotalMilliseconds + "ms");
            content.Bind("s3header", "diagnostic/header.html");
            content.Bind("loadHeaderValue", headerTimer.Elapsed.TotalMilliseconds + "ms");
            content.Bind("s3content", "diagnostic/content.html");
            content.Bind("loadContentValue", contentTimer.Elapsed.TotalMilliseconds + "ms");
            header.Bind(data.Result);

            return(true);
        }
    }
}