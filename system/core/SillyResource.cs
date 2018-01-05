using System;
using System.IO;

namespace SillyWidgets
{
    public class SillyResource
    {
        public enum Types { CSS, JS, Img, Unknown }
        public Types Type { get; private set; }
        public FileInfo File { get; private set; }

        public SillyResource(FileInfo resourceFile)
        {
            Type = SillyResource.DetermineTypeFromExtension(resourceFile.Extension);
            File = resourceFile;
        }

        public byte[] Contents()
        {
            byte[] content = null;

            using (FileStream fs = new FileStream(File.FullName, FileMode.Open))
            {
                content = new byte[File.Length];
                int numBytesToRead = (int)File.Length;
                int numBytesRead = 0;

                while (numBytesToRead > 0)
                {
                    int n = fs.Read(content, numBytesRead, numBytesToRead);

                    if (n == 0) break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }

                numBytesToRead = content.Length;
            }

            return(content);
        }

        static public Types DetermineTypeFromExtension(string extension)
        {
            if (String.IsNullOrEmpty(extension))
            {
                return(Types.Unknown);
            }

            string lowerExtension = extension.ToLower();
            
            if (lowerExtension.Contains("css") )
            {
                return(Types.CSS);
            }
            else if (lowerExtension.Contains("js"))
            {
                return(Types.JS);
            }
            else if (lowerExtension.Contains("png"))
            {
                return(Types.Img);
            }
            else
            {
                return(Types.Unknown);
            }
        }
    }
}