namespace SillyWidgets
{
    public enum SillyResourceLocation { S3, LambdaFileSystem }

    public class SillyResource
    {
        public string Filename { get; set; }
        public SillyResourceLocation Location { get; set; }
        public SillyContentType Type { get; set; }

        public SillyResource(string filename, SillyResourceLocation location = SillyResourceLocation.LambdaFileSystem, SillyContentType type = SillyContentType.Html)
        {
            Filename = filename;
            Location = location;
            Type = type;
        }
    }
}