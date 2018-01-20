using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Amazon;

[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SillyWidgets
{
    public abstract class SillyProxyApplication : SillyApplication, ISillyContext
    {
        public string Path { get; set; }
        public SillyProxyRequest OriginalRequest { get; private set; }
        public SupportedHttpMethods HttpMethod { get; set; }

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
                OriginalRequest.queryStringParameters.Count == 0)
            {
                return(false);
            }           

            return(OriginalRequest.queryStringParameters.TryGetValue(name, out value));
        }

        public object POST(string name)
        {
            return(null);
        }

        //********************************************************************
        // Lambda signature: aws-netcore-serverless-hello-world::AWSNetCore.<MyLambdaHandlerDerivedClass>::Handle
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

                OriginalRequest = input;
                HttpMethod = StringToHttpMethod(input.httpMethod);
                Path = input.path;               

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
