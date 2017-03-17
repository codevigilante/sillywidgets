using System;
using System.Collections.Generic;

namespace silly
{
    public abstract class SillyDirective
    {
        private static Dictionary<string, SillyDirective> SupportedDirectives = new Dictionary<string, SillyDirective>()
        {
            { "new", new NewDirective("new") },
            { "build", new BuildDirective("build") }
            // "dev"
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
    }
}