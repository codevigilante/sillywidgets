using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using SillyWidgets.Gizmos;
using Amazon.S3;
using Amazon.S3.Model;

namespace SillyWidgets
{
    public class SillyView : ISillyView
    {
        public SillyContentType ContentType { get; set; }

        private Dictionary<string, TreeNodeGizmo> BindVals = new Dictionary<string, TreeNodeGizmo>();

        public string Content
        { 
            get
            {
                if (String.IsNullOrEmpty(_content))
                {
                    return(Render());
                }

                return(_content);
            } 
            set
            {
                _content = value;
            }
        }

        private string _content = string.Empty;
        private HtmlGizmo Html = null;

        public SillyView()
        {
            ContentType = SillyContentType.Html;
            Content = string.Empty;
        }

        public void Load(StreamReader data)
        {
            Html = new HtmlGizmo();
            bool success = Html.Load(data);

            if (!success)
            {
                throw new Exception("Parsing HTML: " + Html.ParseError);
            }
        }

        public async Task<bool> LoadS3Async(string bucket, string key, Amazon.RegionEndpoint endpoint)
        {
            AmazonS3Client S3Client = new AmazonS3Client(endpoint);
            GetObjectResponse response = await S3Client.GetObjectAsync(bucket, key);

            using (StreamReader reader = new StreamReader(response.ResponseStream))
            {
                Load(reader);
            }

            return(true);
        }

        public SillyView Load(string filepath)
        {
            if (filepath == null ||
                filepath.Length == 0)
            {
                throw new SillyException(SillyHttpStatusCode.NotFound, "Invalid view file specified, either NULL or empty");
            }

            if (!File.Exists(filepath))
            {
                throw new SillyException(SillyHttpStatusCode.NotFound, "View file '" + filepath + "' does not exist");
            }

            SillyView view = null;
            FileStream fileStream = new FileStream(filepath, FileMode.Open);

            using (StreamReader reader = new StreamReader(fileStream))
            {
                view = new SillyView();
                view.Load(reader);
            }

            return(view);
        }

        public void Bind(string key, string text)
        {       
            TextNode textNode = new TextNode(text);

            BindVals[key] = new TextNode(text);
        }

        public string Render()
        {
            if (Html == null)
            {
                return(_content);
            }

            HtmlPayloadVisitor payloadCreator = new HtmlPayloadVisitor(BindVals);

            Html.ExecuteHtmlVisitor(payloadCreator);
            _content = payloadCreator.Payload.ToString();
            
            return(_content);
        }

    }

    internal class HtmlPayloadVisitor : IVisitor
    {
        public StringBuilder Payload { get; private set; }
        private bool Exiting = false;
        private Dictionary<string, TreeNodeGizmo> BindVals = null;

        public HtmlPayloadVisitor(Dictionary<string, TreeNodeGizmo> bindVals)
        {
            Payload = new StringBuilder();
            BindVals = bindVals;
        }

        public void Go(TreeNodeGizmo node)
        {
            if (node == null)
            {
                return;
            }

            //Console.WriteLine(node.Name);
            Exiting = false;
            node.Accept(this);

            foreach(TreeNodeGizmo child in node.GetChildren())
            {
                Go(child);
            }

            Exiting = true;
            node.Accept(this);
        }

        public void VisitElement(ElementNode node)
        {
            if (Exiting)
            {
                if (node.SelfCloseTag)
                {
                    Payload.Append(" />");
                }
                else if (node.HasCloseTag)
                {
                    Payload.Append("</");
                    Payload.Append(node.Name);
                    Payload.Append(">");
                }

                return;
            }

            Payload.Append("<");
            Payload.Append(node.Name);

            if (node.Attributes.Count > 0)
            {
                foreach(KeyValuePair<string, string> attr in node.Attributes)
                {
                    if (BindVals != null)
                    {
                        SillyAttribute sillyAttr = null;

                        if (SillyAttribute.TryCreateSillyAttribute(attr.Key, out sillyAttr))
                        {
                            //Console.WriteLine("Bind: " + attr.Key + " = " + attr.Value);
                            TreeNodeGizmo bindNode = null;

                            if (BindVals.TryGetValue(attr.Value, out bindNode))
                            {
                                node.DeleteChildren();
                                node.AddChild(bindNode);

                                continue;
                            }
                        }
                    }

                    Payload.Append(" ");
                    Payload.Append(attr.Key);
                    
                    if (!String.IsNullOrEmpty(attr.Value))
                    {
                        Payload.Append("=\"");
                        Payload.Append(attr.Value);
                        Payload.Append("\"");
                    }
                }
            }

            Payload.Append(">");
        }

        public void VisitText(TextNode node)
        {
            if (Exiting)
            {
                return;
            }

            Payload.Append(node.Text);
        }
    }
}