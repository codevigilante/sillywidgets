using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace SillyWidgets
{
    internal class RouteMatchingVisitor : ISillySegmentVisitor
    {
        public Type Controller { get; private set; }
        public MethodInfo Method { get; private set; }
        public List<object> Vars { get; private set; }

        private Dictionary<string, Type> Controllers = null;
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
                Controller = GetController(route.Controller);
            }

            if (Method == null &&
                Controller != null)
            {
                AssignMethod(route.Method, string.Empty);
            }

            return(Controller != null && Method != null);
        }

        public RouteMatchingVisitor(Dictionary<string, Type> registeredControllers)
        {
            Controllers = registeredControllers;
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
            if (!AssignController(currentSegment, currentRoute.Controller))
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

        private Type GetController(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                return(null);
            }

            Type selectedController = null;

            Controllers.TryGetValue(name.ToLower(), out selectedController);

            return(selectedController);
        }

        private bool AssignMethod(string primary, string fallback)
        {
            MethodInfo[] allMethods = Controller.GetMethods();

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
                        m.ReturnType.IsAssignableFrom(typeof(ISillyView)) &&
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
                if (!AssignController(string.Empty, currentRoute.Controller))
                {
                    matchFailed = true;

                    return;
                }
            }

            if (!AssignMethod(currentSegment, currentRoute.Method))
            {
                matchFailed = true;
            }
        }

        public void VisitVariable(SillyVariableSegment variable)
        {
            if (String.IsNullOrEmpty(currentSegment))
            {
                if (variable.DefaultExists())
                {
                    Vars.Add(variable.Default);
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
}