using System;
using System.Diagnostics;
using Xunit;
using SillyWidgets;

namespace system.test
{
    public class FakeApplication : SillyApplication
    {
        public FakeApplication()
            : base()
        {
        }

        [SillyUrlHandler(IsIndex = true)] // /
        public ISillyView Home(ISillyContext context)
        {
            return(null);
        }

        [SillyUrlHandler()] // /about
        public ISillyView About(ISillyContext context)
        {
            return(null);
        }

        [SillyUrlHandler()] // /contact
        public ISillyView Contact(ISillyContext context)
        {
            return(null);
        }

        [SillyUrlHandler("/admin")] // /admin/login/{name}/{password}
        public ISillyView Login(ISillyContext context, string name, string password)
        {
            return(null);
        }

        public ISillyView WontBeMapped(ISillyContext context)
        {
            return(null);
        }

        public void TotallySuperfluous()
        {
            
        }

    }

    public class HandlerTests
    {
        public HandlerTests()
        {

        }

        [Fact]
        public void AppStartupPerformance()
        {
            Console.WriteLine("Performance tests...");
            Stopwatch z = new Stopwatch();
            z.Start();
            FakeApplication app = new FakeApplication();
            z.Stop();
            Console.WriteLine("Discover handlers: " + z.ElapsedMilliseconds + "ms");
        }

        [Fact]
        public void DispatchTests()
        {
            Console.WriteLine("Dispatch tests...");
        }
    }
}