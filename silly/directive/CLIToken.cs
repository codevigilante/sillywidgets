using System;
using System.Collections.Generic;

namespace silly
{
    public class CLIToken
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public enum TokenTypes { Directive, Option, Parameter }
        public TokenTypes Type { get; private set; }

        private static Dictionary<string, CLIToken> TokenDictionary = new Dictionary<string, CLIToken>()
        {
            { AllowableTokens.New, new NewDirective(AllowableTokens.New) },
            { AllowableTokens.Compile, new CompileDirective(AllowableTokens.Compile) },
            { AllowableTokens.Build, new BuildDirective(AllowableTokens.Build) },
            { AllowableTokens.Deploy, new DeployDirective(AllowableTokens.Deploy) },
            { AllowableTokens.Help, new HelpOption(AllowableTokens.Help) },
            { AllowableTokens.Location, new LocationOption(AllowableTokens.Location) },
            { AllowableTokens.SiteName, new SiteNameOption(AllowableTokens.SiteName) }
        };

        public CLIToken(string name, string description, TokenTypes type)
        {
            Name = name;
            Description = description;
            Type = type;
        }
        
        public static SillyDirective CreateDirective(string command)
        {
            CLIToken directive = null;

            TokenDictionary.TryGetValue(command.ToLower(), out directive);

            return(directive as SillyDirective);
        }

        public static CLIToken CreateToken(string tokenStr)
        {
            CLIToken token = null;

            TokenDictionary.TryGetValue(tokenStr.ToLower(), out token);

            if (token == null)
            {
                token = new CLIToken(tokenStr, "parameter", TokenTypes.Parameter);
            }

            return(token);
        }

        public static List<SillyDirective> KnownDirectives()
        {
            List<SillyDirective> directives = new List<SillyDirective>();

            foreach(CLIToken token in TokenDictionary.Values)
            {
                if (token.Type == TokenTypes.Directive)
                {
                    directives.Add(token as SillyDirective);
                }
            }

            return (directives);
        }
    }

    public static class AllowableTokens
    {
        public static string New = "new";
        public static string Compile = "compile";
        public static string Build = "build";
        public static string Deploy = "deploy";
        public static string Help = "-help";
        public static string Location = "-location";
        public static string SiteName = "-sitename";
    }
}