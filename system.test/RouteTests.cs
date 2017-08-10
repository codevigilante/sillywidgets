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
                "/dingus/{var}=poophole/{var}/{var}=89",
                "/something/:controller/somethingELSE/:method/something_even_more/another-thing-that-is-here/{var_67#er}",
                "/hello/world/",
                "/:method/{lskdfnvlnweoirn4}/",
                "/{werv435vw3v3rnqoi3rnvoiq3vq 3rvqi3v  3qrv q3 v q3ovi}",
                "/_____________________/------------------------/{var56}/",
                "/something/{var}="
            };

            foreach(string route in validRoutes)
            {
                Console.WriteLine("Testing valid route: " + route);
                
                SillyRoute sr = new SillyRoute("something", route, "", "");

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
                "/something/:controller/:method/{var}/{var2}/:method",
                "/hello/{var}89"
            };

            foreach(string route in validRoutes)
            {
                Console.WriteLine("Testing invalid route: " + route);

                SillyRoute sr = new SillyRoute("something", route, "", "");

                Assert.False(sr.IsValid);
            }
        }

        [Fact]
        public void DispatchTests()
        {
            DispatchTester dispatcher = new DispatchTester();            

            List<string> routes = new List<string>()
            {
                "/",
                "/about",
                "/content",
                "/admin",
                "/admin/dashboard",
                "/admin/users/123/sexdolle",
                "/admin/dildo/sex/pistol",
                "/admin/dildo",
                "/admin/users/456"
            };

            List<string> invalidRoutes = new List<string>()
            {
                "/poop",
                "/123/annihilation",
                "/admin/users/123/sexdolle/inyourmouth",
                "/admin/uppers"
            };

            foreach(string route in routes)
            {
                Console.Write("Dispatch Route " + route + " -> ");
                
                try
                {
                    SillyProxyRequest request = new SillyProxyRequest();
                    request.path = route;
                    request.httpMethod = "GET";
                    SillyProxyContext context = new SillyProxyContext(request);
                    ISillyView content = dispatcher.Dispatch(context);

                    Console.WriteLine((content == null) ? "null" : content.Content);

                    Assert.NotEqual(content, null);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    Assert.True(false);
                }
            }

            foreach(string invalid in invalidRoutes)
            {
                Console.Write("Dispatch Invalid Route " + invalid + " -> ");
                
                try
                {
                    SillyProxyRequest request = new SillyProxyRequest();
                    request.path = invalid;
                    request.httpMethod = "GET";
                    SillyProxyContext context = new SillyProxyContext(request);
                    ISillyView content = dispatcher.Dispatch(context);

                    Console.WriteLine((content == null) ? "null" : content.Content);

                    Assert.Equal(content, null);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    Assert.True(false);
                }
            }
        }

        public class DispatchTester : SillyProxyApplication
        {
            public DispatchTester()
                : base()
            {
                base.RegisterController("root", typeof(Root));
                base.RegisterController("admin", typeof(Admin));

                GET("empty", "/", "root", "index");
                GET("home", "/:method", "root", "index");
                GET("users", "/admin/users/{var}/{names}", "admin", "Users");
                GET("twovars", "/:controller/:method/{var1}=herpes/{var2}=AIDS", "admin", "Index");
                GET("admin", "/:controller/:method", "Admin", "Index");
            }
        }

        public class Root : AbstractSillyController
        {
            public Root()
            {          
            }

            public ISillyView Index(ISillyContext context)
            {
                ISillyView content = new SillyView();

                content.Content = "<h1>Root.Index</h1>";

                return(content);
            }

            public ISillyView About(ISillyContext context)
            {
                ISillyView content = new SillyView();

                content.Content = "<h1>Root.About</h1>";

                return(content);
            }

            public ISillyView Content(ISillyContext context)
            {
                ISillyView content = new SillyView();

                content.Content = "<h1>Root.Content</h1>";

                return(content);
            }

            public ISillyView SexTaco(ISillyContext context)
            {
                ISillyView content = new SillyView();

                content.Content = "<h1>Root.SexTaco</h1>";

                return(content);
            }
        }

        public class Admin : AbstractSillyController
        {
            public Admin()
            {
            }

            public ISillyView Index(ISillyContext context)
            {
                ISillyView content = new SillyView();

                content.Content = "<h1>Admin.Index</h1>";

                return(content);
            }

            public ISillyView Dashboard(ISillyContext context)
            {
                ISillyView content = new SillyView();

                content.Content = "<h1>Admin.Dashboard</h1>";

                return(content);
            }

            public ISillyView Users(ISillyContext context, object var, object names)
            {
                ISillyView content = new SillyView();

                content.Content = "<h1>Admin.Users(v1, v2)</h1>";

                return(content);
            }

            public ISillyView Dildo(ISillyContext context, object var, object var2)
            {
                ISillyView content = new SillyView();

                content.Content = "<h1>Admin.Dildo(v1, v2)</h1>";

                return(content);
            }
        }
        
    }
}