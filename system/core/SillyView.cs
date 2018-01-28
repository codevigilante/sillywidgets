using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using SillyWidgets.Gizmos;
using Amazon.S3;
using Amazon.S3.Util;
using Amazon.S3.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;

namespace SillyWidgets
{
    public class SillyView : HtmlGizmo, ISillyWidget
    {
        private Dictionary<string, SillyAttribute> BindVals = new Dictionary<string, SillyAttribute>();

        //protected HtmlGizmo Html = null;

        public SillyView()
        {

        }

        public void Load(StreamReader data)
        {
            //Html = new HtmlGizmo();
            bool success = base.Load(data);

            if (!success)
            {
                throw new Exception("Parsing HTML: " + base.ParseError);
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

        public bool Load(string filepath)
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

            FileStream fileStream = new FileStream(filepath, FileMode.Open);

            using (StreamReader reader = new StreamReader(fileStream))
            {
                Load(reader);
            }

            return(true);
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
            ISillyWidget widget = new SillyTextWidget(text);

            Bind(key, widget);
        }

        public void Bind(Document dynamoItem)
        {
            foreach(KeyValuePair<string, DynamoDBEntry> entry in dynamoItem)
            {
                Bind(entry.Key, new SillyTextWidget(entry.Value.AsString()));
            }
        }

        public void Bind(string key, ISillyWidget widget)
        {            
            if (widget == null)
            {
                Bind(key, "Error binding " + key + ": no view or HTML to bind");

                return;
            }

            BindVals[key] = new SillyAttribute(key, SillyAttribute.SillyAttrType.Widget, widget);
        }

        public virtual bool Attach(TreeNodeGizmo node)
        {
            return(false);
        }

        public virtual string Render()
        {
            if (Root == null ||
                Root.Count == 0)
            {
                return(string.Empty);
            }

            HtmlPayloadVisitor payloadCreator = new HtmlPayloadVisitor(BindVals);

            base.ExecuteHtmlVisitor(payloadCreator);
            string content = payloadCreator.Payload.ToString();
            
            return(content);
        }
    }

    internal class HtmlPayloadVisitor : ITreeNodeVisitor
    {
        public StringBuilder Payload { get; private set; }
        private bool Exiting = false;
        private Dictionary<string, SillyAttribute> BindVals = null;
        private bool RenderChildren = true;
        private bool ForceCloseTag = false;

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

            if (RenderChildren)
            {
                foreach(TreeNodeGizmo child in node.GetChildren())
                {
                    Go(child);
                }
            }            

            Exiting = true;
            node.Accept(this);

            RenderChildren = true;
            ForceCloseTag = false;
        }

        public void VisitElement(ElementNode node)
        {
            if (Exiting)
            {
                if (ForceCloseTag || node.HasCloseTag)
                {
                    Payload.Append("</");
                    Payload.Append(node.Name);
                    Payload.Append(">");
                }
                else if (node.SelfCloseTag)
                {
                    Payload.Append(" />");
                }

                return;
            }

            Payload.Append("<");
            Payload.Append(node.Name);

            ISillyWidget widget = null;

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
                                if (boundAttr.Widget == null)
                                {
                                    widget = new SillyTextWidget("Error rendering " + attr.Value + ": trying to bind null node");
                                }
                                else
                                {
                                    widget = boundAttr.Widget;
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

            if (widget != null)
            {
                RenderChildren = widget.Attach(node);
                Payload.Append(">");
                Payload.Append(widget.Render());
                ForceCloseTag = true;
            }
            else if (!node.SelfCloseTag)
            {
                Payload.Append(">");
            }
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