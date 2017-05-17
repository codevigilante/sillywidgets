namespace silly
{
    public class SillyParameter : CLIToken
    {
        public SillyParameter(string name, string description)
            : base(name, description, TokenTypes.Parameter)
        {

        }
    }
}