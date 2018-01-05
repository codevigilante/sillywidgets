using Xunit;
using System;
using SillyWidgets;

namespace system.test
{
    // /
    // /home
    // /blog
    // /admin
    // /seg1
    // /seg2
    // /seg3
    // /folder/home
    // /folder/blog
    // /folder/taco/anus/admin
    public class Home : SillyView
    {
        public Home()
        {

        }

        public Home(string segment, string prefix = "")
            : base(segment, prefix)
        {

        }

        public override string Render(ISillyContext context, string[] urlParams)
        {
            return("Home");
        }
    }

    public class Blog : SillyView
    {
        public Blog()
        {

        }

        public Blog(string segment, string prefix = "")
            : base(segment, prefix)
        {

        }

        public override string Render(ISillyContext context, string[] urlParams)
        {
            return("Blog");
        }
    }

    public class Admin : SillyView
    {
        public Admin(bool acceptParams = false)
            : base(acceptParams)
        {

        }

        public Admin(string segment, bool acceptParams = false, string prefix = "")
            : base(segment, prefix, acceptParams)
        {

        }

        public override string Render(ISillyContext context, string[] urlParams)
        {
            return("Admin");
        }
    }

    public class ProxyTests : SillyProxyApplication
    {
        public ProxyTests()
            : base(new Home())
        {
        }

        [Fact]
        public void MapViewTest()
        {
            Map(new Home()); // /home
            Dispatch("/home", "home");
            Map(new Blog()); // /blog
            Dispatch("/blog", "blog");
            Map(new Admin()); // /admin
            Dispatch("/admin", "admin");
            Map(new Home("seg1")); // /seg1
            Dispatch("/seg1", "home");
            Map(new Blog("seg2")); // /seg2
            Dispatch("/seg2", "blog");
            Map(new Admin("seg3")); // /seg3
            Dispatch("/seg3", "admin");
            Map(new Home(), true);
            Dispatch("/penis", "home", true);
            Map(new Home("", "/folder")); // /folder/home
            Dispatch("/folder/home", "home");
            Map(new Blog("", "/folder")); // /folder/blog
            Dispatch("/folder/blog", "blog");
            Map(new Admin("", true, "/folder/taco/anus")); // /folder/taco/anus/admin/{var}/{var2}
            Dispatch("/folder/taco/anus/admin", "admin");
            Dispatch("/folder/taco/anus/admin/10/sex", "admin");
            Dispatch("/folder/taco/anus/45/62", "admin", true);
            Map(new Home("", "/folder"), true);
            Dispatch("/folder/taco/anus/home", "home", true);
            Map(new Blog("", "/home")); // /home/blog
            Dispatch("/home/blog", "blog");
            Dispatch("/home", "home");
        }

        [Fact]
        public void DispatchHomeTest()
        {
            Dispatch("/", "home");
        }

        private void Dispatch(string path, string content, bool shouldFail = false)
        {
            Console.Write("Dispatch " + path + "...");
            HttpMethod = SupportedHttpMethods.Get;
            Path = path;
            try
            {
                string body = Render(this);
                Console.WriteLine(body);
                Assert.True(String.Compare(body, content, true) == 0);
            }
            catch(SillyException sillyEx)
            {
                Console.WriteLine(sillyEx.Message);

                if (shouldFail)
                {
                    Assert.True(true);
                }
                else
                {
                    Assert.True(false);
                }
            }
        }

        private void Map(SillyView view, bool isNeg = false)
        {
            Console.Write("Map " + view.GetType().Name + "...");
            bool ok = base.MapView(view);
            Console.WriteLine(ok);
            Assert.True(isNeg ? !ok : ok);
        }
        
    }
}