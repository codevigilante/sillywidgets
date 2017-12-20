using Xunit;
using SillyWidgets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using Amazon.DynamoDBv2.DocumentModel;

namespace system.test
{
    public class ControllerTests
    {
        [Fact]
        public void GetFromS3Test()
        {
            Console.Write("Controller Get S3 -> ");
            SillyView view = new SillyView();

            Task<bool> result = view.LoadS3Async("sillywidgets.com", "testdata/testS3.html", Amazon.RegionEndpoint.USWest1);

            result.Wait();

            Assert.True(result.Result);

            Console.WriteLine(view.Render());
        }

        [Fact]
        public void GetFromDynamoDb()
        {
            Console.Write("Controller Get DynamoDB Item -> ");
            SillyController controller = new SillyController();

            Task<Document> result = controller.DynamoGetItemAsync(Amazon.RegionEndpoint.USWest1, "sillywidgets", "codevigilante@gmail.com");

            result.Wait();

            Assert.True(result.Result.ContainsKey("email"));
            Assert.True(String.Compare(result.Result["email"].AsString(), "codevigilante@gmail.com", true) == 0);
            Assert.True(String.Compare(result.Result["brand"].AsString(), "Silly Widgets", true) == 0);

            Console.WriteLine(result.Result["email"].AsString() + ";" + result.Result["brand"].AsString());
        }

        [Fact]
        public void WidgetTest()
        {
            Console.Write("Controller Widget Test -> ");
            SillyController controller = new SillyController();
            SillyView view = new SillyView();

            Task<Document> data = controller.DynamoGetItemAsync(Amazon.RegionEndpoint.USWest1, "sillywidgets", "codevigilante@gmail.com");
            Task<bool> result = view.LoadS3Async("sillywidgets.com", "diagnostic/diag.html", Amazon.RegionEndpoint.USWest1);
            Task<bool> bound = view.BindAsync("content", "sillywidgets.com", "diagnostic/content.html", Amazon.RegionEndpoint.USWest1);
            Task<bool> widgetBound = view.BindAsync("header", "sillywidgets.com", "diagnostic/header.html", Amazon.RegionEndpoint.USWest1);

            data.Wait();
            result.Wait();
            bound.Wait();
            widgetBound.Wait();

            view.Bind(data.Result);
            view.Bind("dynamoGetValue", "This is a test");

            Console.WriteLine(view.Render());
        }
    }
}