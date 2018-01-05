using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using SillyWidgets.Gizmos;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;

namespace SillyWidgets
{
    public class SillyView : ISillyView
    {
        public SillyContentType ContentType { get; set; }

        private Dictionary<string, SillyAttribute> BindVals = new Dictionary<string, SillyAttribute>();

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

        public string Name
        {
            get { return(_name); }
        }

        public string UrlPrefix
        {
            get { return(_urlPrefix); }
        }

        public bool AcceptsUrlParameters
        {
            get { return(_acceptUrlParams); }
        }

        private string _content = string.Empty;
        private string _name = string.Empty;
        private string _urlPrefix = string.Empty;
        private bool _acceptUrlParams = false;
        private HtmlGizmo Html = null;

        public SillyView(bool acceptUrlParams = false)
        {
            ContentType = SillyContentType.Html;
            Content = string.Empty;
            _acceptUrlParams = acceptUrlParams;
        }

        public SillyView(string name, bool acceptUrlParams = false)
            : this(acceptUrlParams)
        {
            _name = name;
        }

        public SillyView(string name, string urlPrefix, bool acceptUrlParams = false)
            : this(name, acceptUrlParams)
        {
            _urlPrefix = urlPrefix;
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

        public async Task<Document> DynamoGetItemAsync(Amazon.RegionEndpoint endpoint, string table, string hashKey)
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(endpoint); 
            Table dbTable = Table.LoadTable(client, table);
            Document result = await dbTable.GetItemAsync(hashKey);
            
            return(result);
        }

        public async Task<Document> DynamoGetItemAsync(Amazon.RegionEndpoint endpoint, string table, Primitive hashKey, Primitive rangeKey)
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(endpoint); 
            Table dbTable = Table.LoadTable(client, table);
            Document result = await dbTable.GetItemAsync(hashKey, rangeKey);
            
            return(result);
        }

        public async Task<List<Document>> DynamoGetItemsAsync(Amazon.RegionEndpoint endpoint, string table, List<Primitive> hashKeys)
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(endpoint); 
            Table dbTable = Table.LoadTable(client, table);
            DocumentBatchGet batchDocument = dbTable.CreateBatchGet();
            
            foreach(Primitive hashKey in hashKeys)
            {
                batchDocument.AddKey(hashKey);
            }

            await batchDocument.ExecuteAsync();
            
            return(batchDocument.Results);
        }

        public void Bind(string key, string text)
        {
            BindVals[key] = new SillyTextAttribute(key, new TextNode(text));
        }

        public void Bind(Document dynamoItem)
        {
            foreach(KeyValuePair<string, DynamoDBEntry> entry in dynamoItem)
            {
                BindVals[entry.Key] = new SillyTextAttribute(entry.Key, new TextNode(entry.Value.AsString()));
            }
        }

        public void Bind(string key, SillyView view)
        {
            if (view == null ||
                view.Html == null)
            {
                TextNode errorNode = new TextNode("Error binding " + key + ": no view or HTML to bind");

                BindVals[key] = new SillyTextAttribute(key, errorNode);

                return;
            }

            BindVals[key] =  new SillyWidgetAttribute(key, view.Html.Root);
        }

        public async Task<bool> BindAsync(string key, string bucket, string bucketKey, Amazon.RegionEndpoint endpoint)
        {
            SillyView s3View = new SillyView();

            bool loaded = await s3View.LoadS3Async(bucket, bucketKey, endpoint);

            if (loaded)
            {
                Bind(key, s3View);
            }
            else
            {
                Bind(key, "Silly view not found: " + key);
            }

            return(loaded);
        }

        public virtual string Render(ISillyContext context, string[] urlParams)
        {
            return(string.Empty);
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
        private Dictionary<string, SillyAttribute> BindVals = null;

        public HtmlPayloadVisitor(Dictionary<string, SillyAttribute> bindVals)
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
                        if (SillyAttribute.IsSillyAttribute(attr.Key))
                        {
                            SillyAttribute boundAttr = null;

                            if (BindVals.TryGetValue(attr.Value, out boundAttr))
                            {
                                node.DeleteChildren();

                                if (boundAttr.BoundValues().Count == 0)
                                {
                                    node.AddChild(new TextNode("Error rendering " + attr.Value + ": trying to bind null node"));
                                }
                                else
                                {
                                    foreach(TreeNodeGizmo childNode in boundAttr.BoundValues())
                                    {
                                        node.AddChild(childNode);
                                    }
                                }                     

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