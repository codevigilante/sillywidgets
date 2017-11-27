using Xunit;
using SillyWidgets;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

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
    }
}