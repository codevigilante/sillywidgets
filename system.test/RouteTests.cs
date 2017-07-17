using Xunit;
using SillyWidgets;
using System;
using System.Collections.Generic;

namespace system.test
{
    public class RouteTests
    {
        private readonly SillyRoute route;

        public RouteTests()
        {
            route = null;
        }

        [Fact]
        public void ValidRouteTests()
        {
            List<string> validRoutes = new List<string>()
            {
                "/",
                "/controller",
                "/a/b/c/d/4/8/4/",
                "/under_score/dash-dash/mix_987-iuFDS",
                "/56/76/32",
                "/:controller/:method/{var}",
                "/something/:controller/somethingELSE/:method/something_even_more/another-thing-that-is-here/{var_67#er}",
                "/hello/world/",
                "/:method/{lskdfnvlnweoirn4}/",
                "/{werv435vw3v3rnqoi3rnvoiq3vq 3rvqi3v  3qrv q3 v q3ovi}",
                "/_____________________/------------------------/{var56}/"
            };

            foreach(string route in validRoutes)
            {
                Console.WriteLine("Testing valid route: " + route);
                
                bool success = SillyRoute.IsValidUrlPattern(route);

                Assert.True(success);
            }
        }

        [Fact]
        public void InvalidRouteTests()
        {
            List<string> validRoutes = new List<string>()
            {
                "",
                " ",
                "something/dingus",
                "/with spaces/dash-dash/mix_987-iuFDS",
                "/kdlope*&",
                "/:moop/:koop/{var}",
                "/something/:controller/somethingELSE/:snatch/something_even_more/another-thing-that-is-here/{var_67#er}",
                "{askldhflaksdhfalksdjf",
                "/:method/lskdfnvlnweoirn4}/",
                "/_____________________/------------------------/stink/12***43/"
            };

            foreach(string route in validRoutes)
            {
                Console.WriteLine("Testing invalid route: " + route);
                
                bool success = SillyRoute.IsValidUrlPattern(route);

                Assert.False(success);
            }
        }
        
    }
}