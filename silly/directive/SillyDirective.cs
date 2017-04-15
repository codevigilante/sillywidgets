using System;
using System.Collections.Generic;

namespace silly
{
    public abstract class SillyDirective
    {
        public static class Directives
        {
            public static string New = "new";
            public static string Compile = "compile";
            public static string Build = "build";
        }

        private static Dictionary<string, SillyDirective> SupportedDirectives = new Dictionary<string, SillyDirective>()
        {
            { Directives.New, new NewDirective(Directives.New) },
            { Directives.Compile, new CompileDirective(Directives.Compile) },
            { Directives.Build, new BuildDirective(Directives.Build) }
            // { "deploy", new DeployDirective("deploy") }
        };
        public string Command { get; private set; }
        public string Description { get; private set; }
        public Dictionary<string, DirectiveOption> Options { get; private set; }

        public SillyDirective(string command, string description)
        {
            this.Command = command;
            this.Description = description;
            this.Options = new Dictionary<string, DirectiveOption>();
        }

        public abstract void Execute(string[] args);

        protected void AddOption(DirectiveOption option)
        {
            if (Options.ContainsKey(option.Name))
            {
                return;
            }

            Options.Add(option.Name, option);
        }

        protected DirectiveOption GetOption(string name)
        {
            DirectiveOption option = null;

            Options.TryGetValue(name, out option);

            return(option);
        }

        public static SillyDirective CreateDirective(string command)
        {
            SillyDirective directive = null;

            SupportedDirectives.TryGetValue(command, out directive);

            return(directive);
        }

        public static Dictionary<string, SillyDirective>.ValueCollection KnownDirectives()
        {
            return (SupportedDirectives.Values);
        }

        public static Dictionary<String, SillyDirective>.KeyCollection KnowDirectiveNames()
        {
            return(SupportedDirectives.Keys);
        }
    }
}