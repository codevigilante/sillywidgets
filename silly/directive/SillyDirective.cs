using System;
using System.IO;
using System.Collections.Generic;

namespace silly
{
    public abstract class SillyDirective : CLIToken
    {
        public DirectoryInfo DefaultLocation { get; private set; }

        protected string HelpString { get; set; }
        protected Dictionary<string, SillyOption> ValidOptions = new Dictionary<string, SillyOption>();

        public SillyDirective(string name, string description)
            : base(name, description, TokenTypes.Directive)
        {
            DefaultLocation = new DirectoryInfo("./");

            ValidOptions.Add(AllowableTokens.Help, null);
            ValidOptions.Add(AllowableTokens.Location, null);
        }

        protected abstract void Run();
        protected virtual void PrintHelp()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("silly " + base.Name + " [options]");
            Console.ResetColor();
            Console.WriteLine(HelpString);

            PrintOptions();
        }

        public void Execute()
        {
            if (ValidOptions[AllowableTokens.Help] != null)
            {
                PrintHelp();

                return;
            }

            LocationOption location = ValidOptions[AllowableTokens.Location] as LocationOption;

            if (location != null)
            {
                Console.Write("Checking location...");

                DefaultLocation = location.GetLocation();

                Console.WriteLine("done");
            }

            Run();
        }

        public virtual void PrintOptions()
        {
            Console.WriteLine();
            Console.WriteLine("valid options for '" + base.Name + "':");

            const string format = "{0,-15} : {1}";

            foreach(string option in ValidOptions.Keys)
            {
                SillyOption opt = CLIToken.CreateToken(option) as SillyOption;

                Console.WriteLine(format, opt.Name, opt.Description);
            }
        }

        public bool AddOption(SillyOption option)
        {
            if (option == null ||
                !ValidOptions.ContainsKey(option.Name))
            {
                return(false);
            }

            ValidOptions[option.Name] = option;

            return(true);
        }
    }
}