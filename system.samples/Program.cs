using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SillyWidgets;
using SillyWidgets.Utilities.Server;
using SillyWidgets.Samples;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;

namespace system.serve
{
    class Program
    {
        const string HELP = "--info";
        const string HELLOWORLD = "helloworld";
        
        static void Main(string[] args)
        {
            /*try
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
            }*/

            string command = string.Empty;

            if (args.Length > 0)
            {
                command = args[0];
            }            

            SillyProxyApplication site = CreateSite(command);

            if(site == null)
            {
                Console.WriteLine("");
                Console.WriteLine("Done");

                return;
            }

            SillySiteServer testServer = new SillySiteServer(site);

            Task server = testServer.Start();

            server.Wait(); 
        }

        private static SillyProxyApplication CreateSite(string command)
        {
            SillyProxyApplication site = null;

            switch(command.ToLower())
            {
                case HELP:
                    PrintHelp();
                    break;
                case HELLOWORLD:
                default:
                    site = new HelloWorld();

                    Console.WriteLine("Launching HelloWorld");

                    break;
            }

            return(site);
        }

        private static void PrintHelp()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Usage: silly [application]");
            Console.ResetColor();
            Console.WriteLine("Where application is...");
            Console.WriteLine("");
            Console.WriteLine("{0,-15} {1,-40}", HELP, "this");
            Console.WriteLine("{0,-15} {1,-40}", HELLOWORLD, "Simple Hello World app");
        }
    }
}
