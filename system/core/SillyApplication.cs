using System;
using System.Collections.Generic;

namespace SillyWidgets
{
    public abstract class SillyApplication
    {
        private SillySegment Root = null;

        public SillyApplication(ISillyPage homePage = null)
        {  
            Root = new SillySegment("/", homePage);
        }

        protected bool MapView(ISillyPage page)
        {
            if (page == null)
            {
                return(false);
            }

            string segment = page.Name;
            
            if (String.IsNullOrEmpty(segment) ||
                String.IsNullOrWhiteSpace(segment))
            {
                segment = page.GetType().Name;
            }  

            string[] prefixSegments = page.UrlPrefix.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
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

            SillySegment pageSegment = new SillySegment(segment, page);

            bool wasMapped = current.AddChild(pageSegment);

            return(wasMapped);
        }

        public virtual ISillyPage Dispatch(ISillyContext context)
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
                TryAcceptView(Root.Page, context, new string[] {}, path);

                return (Root.Page);
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

                    TryAcceptView(current.Page, context, urlParams.Array, path);

                    return(current.Page);
                }
            }

            TryAcceptView(current.Page, context, new string[] {}, path);

            return(current.Page);
        }

        private void TryAcceptView(ISillyPage page, ISillyContext context, string[] urlParams, string path)
        {
            if (page == null)
            {
                throw new SillyException(SillyHttpStatusCode.NotFound, "The path cannot be found: " + path);
            }         

            if (!page.AcceptsUrlParameters && urlParams.Length > 0)
            {
                throw new SillyException(SillyHttpStatusCode.NotFound, "The path cannot be found: " + path);
            }       

            bool accepted = page.Accept(context, urlParams);

            if (!accepted)
            {
                throw new SillyException(SillyHttpStatusCode.NotFound, "The path cannot be found: path " + path + " rejected");
            }
        }
    }
}