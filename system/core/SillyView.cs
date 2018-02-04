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

        public SillyView()
        {
        }

        public void Load(StreamReader data)
        {
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

            SillyHtmlVisitor payloadCreator = new SillyHtmlVisitor(BindVals);

            base.ExecuteHtmlVisitor(payloadCreator);
            string content = payloadCreator.Payload.ToString();
            
            return(content);
        }
    }
}