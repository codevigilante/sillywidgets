using System;
using System.Collections.Generic;

namespace SillyWidgets
{
    public abstract class SillyApplication
    {
        private SillySegment Root = null;

        public SillyApplication(ISillyView homeView = null)
        {  
            Root = new SillySegment("/", homeView);
        }

        protected bool MapView(ISillyView view)
        {
            if (view == null)
            {
                return(false);
            }

            string segment = view.Name;
            
            if (String.IsNullOrEmpty(segment) ||
                String.IsNullOrWhiteSpace(segment))
            {
                segment = view.GetType().Name;
            }  

            string[] prefixSegments = view.UrlPrefix.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            SillySegment current = Root;

            foreach(string prefix in prefixSegments)
            {
                SillySegment child = null;

                if (current.FindPath(prefix, out child))
                {
                    current = child;
                }
                else
                {
                    child = new SillySegment(prefix);

                    current.AddChild(child);
                    current = child;
                }
            }

            SillySegment viewSegment = new SillySegment(segment, view);

            bool wasMapped = current.AddChild(viewSegment);

            return(wasMapped);
        }

        public virtual string Render(ISillyContext context)
        {
            if (context == null)
            {
                throw new SillyException(SillyHttpStatusCode.ServerError, "No context to process request");
            }

            if (context.HttpMethod == SupportedHttpMethods.Unsupported)
            {
                throw new SillyException(SillyHttpStatusCode.BadRequest, "Unsupported HTTP method");
            }

            string path = context.Path;

            if (String.IsNullOrEmpty(path) || String.IsNullOrWhiteSpace(path))
            {
                throw new SillyException(SillyHttpStatusCode.ServerError, "Path is invalid: " + path);
            }            

            string[] pathSegments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (Root == null)
            {
                throw new SillyException(SillyHttpStatusCode.NotImplemented, "No suitable view found");
            }

            if (pathSegments.Length == 0)
            {
                if (Root.View == null)
                {
                    throw new SillyException(SillyHttpStatusCode.NotFound, "The path cannot be found: no home view");
                }                

                return(Root.View.Render(context, new string[] {}));
            }

            SillySegment current = Root;

            for (int i = 0; i < pathSegments.Length; ++i)
            {
                string segment = pathSegments[i];
                SillySegment next = null;

                if (current.FindPath(segment, out next))
                {
                    current = next;
                }
                else if (current.View != null &&
                         current.View.AcceptsUrlParameters)
                {
                    ArraySegment<string> urlParams = new ArraySegment<string>(pathSegments, i, pathSegments.Length - i);
                    
                    return(current.View.Render(context, urlParams.Array));
                }
                else
                {
                    throw new SillyException(SillyHttpStatusCode.NotFound, "The path cannot be found: " + path);
                }
            }

            if (current.View == null)
            {
                throw new SillyException(SillyHttpStatusCode.NotFound, "The path cannot be found: " + path);
            }

            return(current.View.Render(context, new string[] {}));
        }
    }
}