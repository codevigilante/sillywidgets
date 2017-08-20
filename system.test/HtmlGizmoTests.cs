using Xunit;
using SillyWidgets.Gizmos;
using System;
using System.IO;

namespace system.test
{
    public class HtmlGizmoTests
    {
        public HtmlGizmoTests()
        {

        }

        [Fact]
        public void ValidHtmlTests()
        {
            HtmlGizmo html = new HtmlGizmo();

            FileStream fileStream = new FileStream("testdata/simple.html", FileMode.Open);

            using (StreamReader reader = new StreamReader(fileStream))
            {
                bool success = html.Load(reader);

                Assert.True(success);
            }
        }
    }
}