using Xunit;
using SillyWidgets.Gizmos;
using System;
using System.IO;
using System.Diagnostics;

namespace system.test
{
    public class HtmlGizmoTests
    {
        public HtmlGizmoTests()
        {

        }

        /*
        [Fact]
        public void ValidHtmlTests()
        {
            HtmlGizmo html = new HtmlGizmo();

            FileStream fileStream = new FileStream("testdata/simple.html", FileMode.Open);

            using (StreamReader reader = new StreamReader(fileStream))
            {
                Console.Write("Parsing simple.html...");
                Stopwatch timer = new Stopwatch();
                timer.Start();
                bool success = html.Load(reader);
                timer.Stop();

                Console.WriteLine("Parse time: " + timer.ElapsedMilliseconds + "ms");

                Assert.True(success);
            }

            fileStream = new FileStream("testdata/google.html", FileMode.Open);

            using (StreamReader reader = new StreamReader(fileStream))
            {
                Console.Write("Parsing google.html...");
                Stopwatch timer = new Stopwatch();
                timer.Start();
                bool success = html.Load(reader);
                timer.Stop();

                Console.WriteLine("Parse time: " + timer.ElapsedMilliseconds + "ms");

                Assert.True(success);
            }

            Console.WriteLine();
        }*/
    }
}