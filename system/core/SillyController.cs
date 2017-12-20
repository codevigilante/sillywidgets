using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;

namespace SillyWidgets
{
    public class SillyController
    {
        public SillyController()
        {
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
    }
}