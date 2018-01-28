using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Amazon;
using SillyWidgets.Gizmos;

[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SillyWidgets
{
    public abstract class SillyProxyApplication : SillyApplication, ISillyContext
    {
        public string Path { get; set; }
        public SillyProxyRequest OriginalRequest { get; private set; }
        public SupportedHttpMethods HttpMethod { get; set; }

        private Dictionary<string, object> Post = new Dictionary<string, object>();

        public SillyProxyApplication(ISillyPage homePage = null)
            : base(homePage)
        {
            Path = string.Empty;
            OriginalRequest = null;
            HttpMethod = SupportedHttpMethods.Unsupported;
        }

        public bool GET(string name, out object value)
        {
            value = null;

            if (OriginalRequest == null ||
                OriginalRequest.queryStringParameters == null ||
                OriginalRequest.queryStringParameters.Count == 0)
            {
                return(false);
            }           

            return(OriginalRequest.queryStringParameters.TryGetValue(name, out value));
        }

        public bool POST(string name, out object value)
        {
            value = null;

            return(Post.TryGetValue(name, out value));
        }

        public bool HEADER(string name, out string value)
        {
            value = string.Empty;

            if (OriginalRequest == null ||
                OriginalRequest.headers == null ||
                OriginalRequest.headers.Count == 0)
            {
                return(false);
            }

            object val = null;

            if (OriginalRequest.headers.TryGetValue(name, out val))
            {
                value = val.ToString();

                return(true);
            }

            return(false);
        }

        //********************************************************************
        // Lambda signature: <assembly>::<namespace>.<MyProxyDerivedClass>::<method>
        //********************************************************************
        [Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public virtual SillyProxyResponse Handle(SillyProxyRequest input, ILambdaContext lambdaContext)
        {
            try
            {
                if (input == null)
                {
                    throw new SillyException(SillyHttpStatusCode.ServerError, "Request aborted upon delivery.");
                }

                DoStuffWithRequest(input);     

                ISillyPage page = base.Dispatch(this);

                if (page == null)
                {
                    throw new SillyException(SillyHttpStatusCode.NotFound, "The path cannot be found: " + Path);
                }

                SillyProxyResponse response = new SillyProxyResponse();
                response.body = page.Render();

                return(response);
            }
            catch (SillyException sillyEx)
            {
                return(buildErrorResponse(sillyEx));
            }
            catch (Exception Ex)
            {
                return(buildErrorResponse(SillyHttpStatusCode.ServerError, Ex.Message + "\n" + Ex.StackTrace));
            }
        }

        private void DoStuffWithRequest(SillyProxyRequest request)
        {
            OriginalRequest = request;
            HttpMethod = StringToHttpMethod(request.httpMethod);
            Path = request.path;   

            if (!String.IsNullOrEmpty(request.body) && HttpMethod == SupportedHttpMethods.Post)
            {
                string decodedBody = WebUtilityGizmo.UrlDecode(request.body);
                string[] nameValuePairs = decodedBody.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                foreach(string nameValue in nameValuePairs)
                {
                    string[] divided = nameValue.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                    if (divided.Length == 0)
                    {
                        continue;
                    }

                    string name = divided[0];
                    string value = string.Empty;

                    if (divided.Length == 2)
                    {
                        value = divided[1];
                    }

                    Post[name] = value;
                }
            }       
        }

        private SupportedHttpMethods StringToHttpMethod(string httpMethod)
        {
            if (String.IsNullOrEmpty(httpMethod))
            {
                return(SupportedHttpMethods.Unsupported);
            }

            if (String.Compare(httpMethod, "GET", true) == 0)
            {
                return(SupportedHttpMethods.Get);
            }
            
            if (String.Compare(httpMethod, "Post", true) == 0)
            {
                return(SupportedHttpMethods.Post);
            }

            return(SupportedHttpMethods.Unsupported);
        }

        private SillyProxyResponse buildErrorResponse(SillyException ex)
        {
            return(buildErrorResponse(ex.StatusCode, ex.Message));
        }

        private SillyProxyResponse buildErrorResponse(SillyHttpStatusCode statusCode, string details)
        {
            SillyProxyResponse errorResponse = new SillyProxyResponse(statusCode);

            errorResponse.body = "<h1>Server ERROR</h1><h3>" + errorResponse.StatusCodeToString() + "</h3>";
            errorResponse.body += "<p>" + details + "</p>";

            return(errorResponse);
        }
    }
}
