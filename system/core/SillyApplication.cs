using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SillyWidgets
{
    public abstract class SillyApplication
    {
        private Dictionary<string, SillyController> Controllers = new Dictionary<string, SillyController>();
        private Dictionary<SupportedHttpMethods, List<SillyRoute>> Routes = new Dictionary<SupportedHttpMethods, List<SillyRoute>>()
        {
            { SupportedHttpMethods.Get, new List<SillyRoute>() },
            { SupportedHttpMethods.Post, new List<SillyRoute>() }

        };

        public SillyApplication()
        {    
            DiscoverHandlers(this.GetType());
        }

        public SillyRoute GET(string key, string urlPattern, string controller, string method)
        {
            return(AddRoute(SupportedHttpMethods.Get, key, urlPattern, controller, method));
        }

        public SillyRoute POST(string key, string urlPattern, string controller, string method)
        {
            return(AddRoute(SupportedHttpMethods.Post, key, urlPattern, controller, method));
        }

        protected void RegisterController(string key, SillyController controller)
        {
            if (String.IsNullOrEmpty(key))
            {
                if (controller == null)
                {
                    throw new SillyException(SillyHttpStatusCode.ServerError, "Cannot register empty key AND null Controller");
                }

                key = controller.GetType().Name.ToLower();
            }

            // if controller is null, it will be culled out if the controller is ever selected for execution, so we don't
            // blow up the entire system if one controller is "bad"
            // should probably log this somewhere, that'd be nice, m'kay
            Controllers[key] = controller;
        }

        private void DiscoverHandlers(Type type)
        {
            // method has to be public, return an ISillyView, and take ISillyContext as first argument
            MethodInfo[] allMethods = type.GetMethods();

            IEnumerable<MethodInfo> methods = allMethods.Where
            (
                m => m.ReturnType.IsAssignableFrom(typeof(ISillyView)) &&
                     m.GetParameters().Length >= 1 &&
                     m.GetParameters()[0].ParameterType == typeof(ISillyContext) &&
                     m.IsPublic
            );

            if (methods == null ||
                methods.Count() == 0)
            {
                return;
            }

            foreach(MethodInfo info in methods)
            {
                IEnumerable<Attribute> attributes = info.GetCustomAttributes();

                foreach(Attribute attr in attributes)
                {
                    SillyUrlHandlerAttribute urlHandler = attr as SillyUrlHandlerAttribute;

                    if (urlHandler != null)
                    {
                        // map this Url to the handler method
                        
                    }
                }
            }

            /*int varCount = currentRoute.VarCount + 1;

            IEnumerable<MethodInfo> methods = candidateMethods.Where
            (
                m => String.Compare(m.Name, name, true) == 0 &&
                        m.ReturnType.IsAssignableFrom(typeof(ISillyView)) &&
                        m.GetParameters().Length == varCount &&
                        m.GetParameters()[0].ParameterType == typeof(ISillyContext)
            );

            if (methods == null ||
                methods.Count() == 0)
            {
                return(null);
            }

            return(methods.ElementAt(0));*/

            // also has to declare SillyUrlHandler, passing in an optional prefix
            // this particular method maps to the url: /admin/user/[entries]
            // [SillyUrlHandler(prefix = "")]
            // maybe allow SillyUrlHandler to be applied to a class, adding a prefix to all the methods?
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

                    try
                    {
                        ISillyView view = visitor.Method.Invoke(visitor.Controller, vars) as ISillyView;

                        return(view);
                    }
                    catch(Exception ex)
                    {
                        throw ex.InnerException;
                    }
                }
            }

            return(null);
        }
    }
}