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

        public override bool Accept(ISillyContext context, string[] urlParams)
        {
            return(true);
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

        public override bool Accept(ISillyContext context, string[] urlParams)
        {
            return(true);
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

        public override bool Accept(ISillyContext context, string[] urlParams)
        {
            return(true);
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
            Dispatch("/home", typeof(Home));
            Map(new Blog()); // /blog
            Dispatch("/blog", typeof(Blog));
            Map(new Admin()); // /admin
            Dispatch("/admin", typeof(Admin));
            Map(new Home("seg1")); // /seg1
            Dispatch("/seg1", typeof(Home));
            Map(new Blog("seg2")); // /seg2
            Dispatch("/seg2", typeof(Blog));
            Map(new Admin("seg3")); // /seg3
            Dispatch("/seg3", typeof(Admin));
            Map(new Home(), true);
            Dispatch("/penis", typeof(Home), true);
            Map(new Home("", "/folder")); // /folder/home
            Dispatch("/folder/home", typeof(Home));
            Map(new Blog("", "/folder")); // /folder/blog
            Dispatch("/folder/blog", typeof(Blog));
            Map(new Admin("", true, "/folder/taco/anus")); // /folder/taco/anus/admin/{var}/{var2}
            Dispatch("/folder/taco/anus/admin", typeof(Admin));
            Dispatch("/folder/taco/anus/admin/10/sex", typeof(Admin));
            Dispatch("/folder/taco/anus/45/62", typeof(Admin), true);
            Map(new Home("", "/folder"), true);
            Dispatch("/folder/taco/anus/home", typeof(Home), true);
            Map(new Blog("", "/home")); // /home/blog
            Dispatch("/home/blog", typeof(Blog));
            Dispatch("/home", typeof(Home));
        }

        [Fact]
        public void DispatchHomeTest()
        {
            Dispatch("/", typeof(Home));
        }

        private void Dispatch(string path, Type viewType, bool shouldFail = false)
        {
            Console.Write("Dispatch " + path + "...");
            HttpMethod = SupportedHttpMethods.Get;
            Path = path;
            try
            {
                ISillyView view = base.Dispatch(this);

                if (view == null)
                {
                    throw new SillyException(SillyHttpStatusCode.BadRequest, "View was null");
                }

                Assert.True(view != null);
                Assert.True(viewType == view.GetType());
                Console.WriteLine(view.GetType());
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