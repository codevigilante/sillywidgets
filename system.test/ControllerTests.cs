using Xunit;
using SillyWidgets;
using System;
using System.Threading.Tasks;

namespace system.test
{
    public class ControllerTests
    {
        [Fact]
        public void GetFromS3Test()
        {
            Console.Write("Controller Get S3 -> ");
            SillyController controller = new SillyController();

            Task<ISillyView> view = controller.LoadViewAsync("sillywidgets.com", "testdata/testS3.html", Amazon.RegionEndpoint.USWest1);

            view.Wait();

            Assert.True(view != null);

            Console.WriteLine(view.Result.Content);
        }
    }
}