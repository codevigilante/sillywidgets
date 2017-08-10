using System;
using System.Collections.Generic;

namespace SillyWidgets
{
    public abstract class SillyApplication
    {
        public SillyOptions Options { get; private set; }

        private Dictionary<string, Type> Controllers = new Dictionary<string, Type>();
        private Dictionary<SupportedHttpMethods, List<SillyRoute>> Routes = new Dictionary<SupportedHttpMethods, List<SillyRoute>>()
        {
            { SupportedHttpMethods.Get, new List<SillyRoute>() },
            { SupportedHttpMethods.Post, new List<SillyRoute>() }

        };

        public SillyApplication()
        {
            Options = new SillyOptions();    
        }

        public SillyRoute GET(string key, string urlPattern, string controller, string method)
        {
            return(AddRoute(SupportedHttpMethods.Get, key, urlPattern, controller, method));
        }

        public SillyRoute POST(string key, string urlPattern, string controller, string method)
        {
            return(AddRoute(SupportedHttpMethods.Post, key, urlPattern, controller, method));
        }

        protected void RegisterController(string key, Type controllerType)
        {
            if (String.IsNullOrEmpty(key))
            {
                key = controllerType.Name.ToLower();
            }

            Controllers[key] = controllerType;
        }

        private SillyRoute AddRoute(SupportedHttpMethods httpMethod, string key, string urlPattern, string controller, string method)
        {
            if (String.IsNullOrEmpty(key))
            {
                key = "default";
            }

            SillyRoute route = new SillyRoute(key, urlPattern, controller, method);

            Routes[httpMethod].Add(route);

            return(route);
        }

        public ISillyView Dispatch(ISillyContext context)
        {
            if (context == null ||
                context.HttpMethod == SupportedHttpMethods.Unsupported)
            {
                return(null);
            }

            string path = context.Path;

            if (String.IsNullOrEmpty(path) || String.IsNullOrWhiteSpace(path))
            {
                return(null);
            }

            List<SillyRoute> availableRoutes = Routes[context.HttpMethod];

            if (availableRoutes.Count == 0)
            {
                return(null);
            }

            string[] pathSegments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach(SillyRoute route in availableRoutes)
            {
                if (route.Count == 0 ||
                    !route.IsValid ||
                    pathSegments.Length > route.Count)
                {
                    continue;
                }

                RouteMatchingVisitor visitor = new RouteMatchingVisitor(Controllers);

                if (visitor.TryMatch(route, pathSegments))
                {
                    if (visitor.Controller == null ||
                        visitor.Method == null)
                    {
                        continue;
                    }

                    int varCount = 1 + visitor.Vars.Count;
                    object[] vars = new object[varCount];

                    vars[0] = context;

                    if (visitor.Vars.Count > 0)
                    {
                        int index = 1;

                        foreach(object var in visitor.Vars)
                        {
                            vars[index] = var;
                        }
                    }

                    object controllerTarget = Activator.CreateInstance(visitor.Controller);

                    return(visitor.Method.Invoke(controllerTarget, vars) as ISillyView);
                }
            }

            return(null);
        }
    }
}