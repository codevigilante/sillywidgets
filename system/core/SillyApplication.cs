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

        public virtual ISillyView Dispatch(ISillyContext context)
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
                TryAcceptView(Root.View, context, new string[] {}, path);

                return (Root.View);
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
                else
                {
                    ArraySegment<string> urlParams = new ArraySegment<string>(pathSegments, i, pathSegments.Length - i);

                    TryAcceptView(current.View, context, urlParams.Array, path);

                    return(current.View);
                }
            }

            TryAcceptView(current.View, context, new string[] {}, path);

            return(current.View);
        }

        private void TryAcceptView(ISillyView view, ISillyContext context, string[] urlParams, string path)
        {
            if (view == null)
            {
                throw new SillyException(SillyHttpStatusCode.NotFound, "The path cannot be found: " + path);
            }         

            if (!view.AcceptsUrlParameters && urlParams.Length > 0)
            {
                throw new SillyException(SillyHttpStatusCode.NotFound, "The path cannot be found: " + path);
            }       

            bool accepted = view.Accept(context, urlParams);

            if (!accepted)
            {
                throw new SillyException(SillyHttpStatusCode.NotFound, "The path cannot be found: path " + path + " rejected");
            }
        }
    }
}