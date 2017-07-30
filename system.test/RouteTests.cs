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
                
                SillyRoute sr = new SillyRoute("something", route);

                Assert.True(sr.IsValid);
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
                "/_____________________/------------------------/stink/12***43/",
                "/:controller/something/:controller",
                "/something/:controller/:method/{var}/{var2}/:method"
            };

            foreach(string route in validRoutes)
            {
                Console.WriteLine("Testing invalid route: " + route);

                SillyRoute sr = new SillyRoute("something", route);

                Assert.False(sr.IsValid);
            }
        }

        [Fact]
        public void DispatchTests()
        {
            SillyRouteMap.SetAvailableControllers(new List<AbstractSillyController>()
            {
                new Root(),
                new Admin()
            });

            SillyRouteMap.MapRoute("home", "/:method", new SillyRouteDetails("Root", "Index"));
            SillyRouteMap.MapRoute("users", "/admin/users/{var}/{names}", new SillyRouteDetails("Admin", "Users"));
            SillyRouteMap.MapRoute("twovars", "/:controller/:method/{var1}/{var2}", new SillyRouteDetails("Admin", "Index")
            {
                { "var1", "herpes" },
                { "var2", "AIDS" }
            });
            SillyRouteMap.MapRoute("admin", "/:controller/:method", new SillyRouteDetails("Admin", "Index"));

            List<string> routes = new List<string>()
            {
                "/",
                "/about",
                "/content",
                "/admin",
                "/admin/dashboard",
                "/admin/users/123/sexdolle",
                "/admin/dildo/sex/pistol",
                "/admin/dildo"
            };

            foreach(string route in routes)
            {
                Console.Write("Dispatch Route " + route + " -> ");
                
                try
                {
                    ISillyContent content = SillyRouteMap.Dispatch(route, null);

                    Console.WriteLine((content == null) ? "null" : content.Content);

                    Assert.NotEqual(content, null);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    Assert.True(false);
                }
            }
        }

        public class Root : AbstractSillyController
        {
            public Root()
            {          
            }

            public ISillyContent Index(ISillyContext context)
            {
                ISillyContent content = new SillyContent();

                content.Content = "<h1>Root.Index</h1>";

                return(content);
            }

            public ISillyContent About(ISillyContext context)
            {
                ISillyContent content = new SillyContent();

                content.Content = "<h1>Root.About</h1>";

                return(content);
            }

            public ISillyContent Content(ISillyContext context)
            {
                ISillyContent content = new SillyContent();

                content.Content = "<h1>Root.Content</h1>";

                return(content);
            }
        }

        public class Admin : AbstractSillyController
        {
            public Admin()
            {
            }

            public ISillyContent Index(ISillyContext context)
            {
                ISillyContent content = new SillyContent();

                content.Content = "<h1>Admin.Index</h1>";

                return(content);
            }

            public ISillyContent Dashboard(ISillyContext context)
            {
                ISillyContent content = new SillyContent();

                content.Content = "<h1>Admin.Dashboard</h1>";

                return(content);
            }

            public ISillyContent Users(ISillyContext context, object var, object names)
            {
                ISillyContent content = new SillyContent();

                content.Content = "<h1>Admin.Users(v1, v2)</h1>";

                return(content);
            }

            public ISillyContent Dildo(ISillyContext context, object var, object var2)
            {
                ISillyContent content = new SillyContent();

                content.Content = "<h1>Admin.Dildo(v1, v2)</h1>";

                return(content);
            }
        }
        
    }
}