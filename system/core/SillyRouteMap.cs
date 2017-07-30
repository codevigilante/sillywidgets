using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SillyWidgets
{

    public static class SillyRouteMap
    {
        private static List<SillyRoute> Routes = new List<SillyRoute>();
        private static Dictionary<string, AbstractSillyController> Controllers = new Dictionary<string, AbstractSillyController>();

        private class RouteMatchingVisitor : ISillySegmentVisitor
        {
            public AbstractSillyController Controller { get; private set; }
            public MethodInfo Method { get; private set; }
            public List<object> Vars { get; private set; }

            private string currentSegment;
            private SillyRoute currentRoute;
            private bool segmentConsumed;
            private bool matchFailed;

            public bool TryMatch(SillyRoute route, string[] pathSegments)
            {
                if (pathSegments == null ||
                    route == null)
                {
                    return(false);
                }

                //Console.WriteLine("Testing route = " + route.UrlPattern + ":" + route.Name);

                currentRoute = route;

                int index = 0;
                int consumed = 0;

                for(; consumed < pathSegments.Length; ++consumed)
                {
                    if (index >= route.Count)
                    {
                        return(false);
                    }

                    currentSegment = pathSegments[consumed].ToLower();
                    segmentConsumed = false;

                    for(; index < route.Count && !segmentConsumed; ++index)
                    {
                        SillySegment segment = route[index];

                        segment.Visit(this);

                        if (matchFailed)
                        {
                            return(false);
                        }
                    }
                }

                currentSegment = string.Empty;

                if (index < route.Count)
                {
                    currentSegment = string.Empty;

                    for(; index < route.Count; ++index)
                    {
                        SillySegment segment = route[index];

                        segment.Visit(this);

                        if (matchFailed)
                        {
                            return(false);
                        }
                    }
                }

                if (Controller == null)
                {
                    Controller = GetController(route.Settings.Controller);
                }

                if (Method == null &&
                    Controller != null)
                {
                    AssignMethod(route.Settings.Method, string.Empty);
                }

                return(Controller != null && Method != null);
            }

            public RouteMatchingVisitor()
            {
                Controller = null;
                Method = null;
                Vars = new List<object>();
                currentSegment = string.Empty;
                segmentConsumed = false;
                matchFailed = false;
                currentRoute = null;
            }

            public void VisitController(SillyControllerSegment controller)
            {
                if (!AssignController(currentSegment, currentRoute.Settings.Controller))
                {
                    matchFailed = true;
                }
            }

            private bool AssignController(string primary, string fallback)
            {
                Controller = GetController(primary);

                if (Controller == null)
                {
                    if (!String.IsNullOrEmpty(primary))
                    {
                        return(false);
                    }
                    
                    Controller = GetController(fallback);

                    return(Controller != null);
                }
                
                segmentConsumed = true;

                return(true);
            }

            private AbstractSillyController GetController(string name)
            {
                if (String.IsNullOrEmpty(name))
                {
                    return(null);
                }

                AbstractSillyController selectedController = null;

                Controllers.TryGetValue(name.ToLower(), out selectedController);

                return(selectedController);
            }

            private bool AssignMethod(string primary, string fallback)
            {
                MethodInfo[] allMethods = Controller.GetType().GetMethods();

                Method = GetMethod(primary, allMethods);

                if (Method == null)
                {
                    if (!String.IsNullOrEmpty(primary))
                    {
                        return(false);
                    }

                    Method = GetMethod(fallback, allMethods);

                    return (Method != null);
                }

                segmentConsumed = true;

                return(true);
            }

            private MethodInfo GetMethod(string name, MethodInfo[] candidateMethods)
            {
                if (String.IsNullOrEmpty(name))
                {
                    return(null);
                }

                int varCount = currentRoute.VarCount + 1;

                IEnumerable<MethodInfo> methods = candidateMethods.Where
                (
                    m => String.Compare(m.Name, name, true) == 0 &&
                         m.ReturnType.IsAssignableFrom(typeof(ISillyContent)) &&
                         m.GetParameters().Length == varCount &&
                         m.GetParameters()[0].ParameterType == typeof(ISillyContext)
                );

                if (methods == null ||
                    methods.Count() == 0)
                {
                    return(null);
                }

                return(methods.ElementAt(0));
            }

            public void VisitMethod(SillyMethodSegment method)
            {
                if (Controller == null)
                {
                    if (!AssignController(string.Empty, currentRoute.Settings.Controller))
                    {
                        matchFailed = true;

                        return;
                    }
                }

                if (!AssignMethod(currentSegment, currentRoute.Settings.Method))
                {
                    matchFailed = true;
                }
            }

            public void VisitVariable(SillyVariableSegment variable)
            {
                object value = null;

                if (String.IsNullOrEmpty(currentSegment))
                {
                    if (currentRoute.Settings.TryGetValue(variable.Value.ToLower(), out value))
                    {
                        Vars.Add(value);
                    }
                    else
                    {
                        matchFailed = true;
                    }
                }
                else
                {
                    Vars.Add(currentSegment);

                    segmentConsumed = true;
                }
            }

            public void VisitStatic(SillyHardCodedSegment hardCoded)
            {
                if (hardCoded.IsRoot && String.IsNullOrEmpty(currentSegment))
                {
                    segmentConsumed = true;

                    return;
                }

                if (String.Compare(hardCoded.Value, currentSegment, true) == 0)
                {
                    segmentConsumed = true;
                }
                else
                {
                    matchFailed = true;
                }
            }
        }

        public static int RouteCount()
        {
            return(Routes.Count);
        }

        public static bool SetAvailableControllers(List<AbstractSillyController> controllers)
        {
            if (controllers == null)
            {
                return(false);
            }

            foreach(AbstractSillyController controller in controllers)
            {
                string name = controller.GetType().Name.ToLower();

                Controllers[name] = controller;
            }

            return(true);
        }

        public static SillyRoute MapRoute(string name, string urlPattern, SillyRouteDetails settings)
        {
            if (String.IsNullOrEmpty(name))
            {
                name = "default";
            }

            SillyRoute route = new SillyRoute(name, urlPattern, settings);

            Routes.Add(route);

            return(route);
        }   

        public static void ClearRoutes()
        {
            Routes.Clear();
        }

        public static ISillyContent Dispatch(string path, ISillyContext context)
        {
            if (String.IsNullOrEmpty(path) || String.IsNullOrWhiteSpace(path))
            {
                return(null);
            }

            string[] pathSegments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach(SillyRoute route in Routes)
            {
                if (route.Count == 0 ||
                    !route.IsValid ||
                    pathSegments.Length > route.Count)
                {
                    continue;
                }

                RouteMatchingVisitor visitor = new RouteMatchingVisitor();

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

                    return(visitor.Method.Invoke(visitor.Controller, vars) as ISillyContent);
                }
            }

            return(null);
        }
    }
}