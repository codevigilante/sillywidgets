using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;

namespace SillyWidgets
{
    public class SillyController
    {
        public SillyController()
        {
        }

        public SillyView LoadView(string filepath)
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

        public async Task<ISillyView> LoadViewAsync(string bucket, string key, Amazon.RegionEndpoint endpoint)
        {
            SillyView view = null;
            AmazonS3Client client = new AmazonS3Client(endpoint);
            GetObjectResponse response = await client.GetObjectAsync(bucket, key);

            using (StreamReader reader = new StreamReader(response.ResponseStream))
            {
                view = new SillyView();
                view.Load(reader);
            }

            return(view);
        }
    }
}