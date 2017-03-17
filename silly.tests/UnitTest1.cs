using System;
using Xunit;
using silly;

namespace silly.tests
{
    public class GeneralTests
    {
        [Fact]
        public void General()
        {
            Console.WriteLine("Starting General tests...");

            int result = SillyCLI.Main(new string[] {});
            Assert.Equal(result, 2);

            result = SillyCLI.Main(new string[] { "new" });
            Assert.Equal(result, 1);
            
            result = SillyCLI.Main(new string[] { "penis" });
            Assert.Equal(result, -1);
        }
    }
}
