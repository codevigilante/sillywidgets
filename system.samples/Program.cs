using System;
using System.Threading.Tasks;
using SillyWidgets;
using SillyWidgets.Utilities;
using SillyWidgets.Samples;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;

namespace system.serve
{
    class Program
    {
        static string BUCKET_NAME = "awsnetcore.com";
        static string AccessKey = "AKIAJLECGMENALAF7P6A";
        static string SecretKey = "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";

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Bucket " + BUCKET_NAME + " Objects:");

                AmazonS3Client client = new AmazonS3Client(AccessKey, SecretKey, Amazon.RegionEndpoint.USWest1);

                ListObjectsV2Request request = new ListObjectsV2Request
                {
                    BucketName = BUCKET_NAME,
                    MaxKeys = 100
                };

                Task<ListObjectsV2Response> response = client.ListObjectsV2Async(request);

                response.Wait();

                foreach(S3Object entry in response.Result.S3Objects)
                {
                    Console.WriteLine(entry.BucketName + ":" + entry.Key + ":" + entry.ETag);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            

            SillySite site = new SillySite();

            SillySiteServer testServer = new SillySiteServer(site);

            Task server = testServer.Start();

            server.Wait(); 
        }
    }
}
