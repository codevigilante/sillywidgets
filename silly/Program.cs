using System;
using System.Collections.Generic;

namespace silly
{
    public class SillyCLI
    {
        public static int Main(string[] args)
        {
            if (args.Length <= 0)
            {
                OutputAbout();
                OutputDirectives();

                return ((int)Globals.ExitReasons.OK);
            }

            SillyDirective directive = SillyDirective.CreateDirective(args[0]);

            if (directive == null)
            {
                CLIError(args[0], "unknown directive");
                OutputDirectives();

                return ((int)Globals.ExitReasons.UnknownDirective);
            }

            SillyOption option = null;

            for(UInt16 i = 1; i < args.Length; ++i)
            {
                string arg = args[i];

                CLIToken token = CLIToken.CreateToken(arg);

                if (token.Type == CLIToken.TokenTypes.Directive)
                {
                    CLIError(arg, "cannot execute multiple directives in same command");

                    return ((int)Globals.ExitReasons.DirectiveFail);
                }
                else if (token.Type == CLIToken.TokenTypes.Option)
                {
                    option = token as SillyOption;

                    bool isValid = directive.AddOption(option);
                    
                    if (!isValid)
                    {
                        CLIError(arg, "invalid option for directive '" + directive.Name + "'");
                        directive.PrintOptions();

                        return ((int)Globals.ExitReasons.InvalidOption);
                    }
                }
                else if (token.Type == CLIToken.TokenTypes.Parameter)
                {
                    if (option == null)
                    {
                        CLIError(arg, "invalid option");
                        directive.PrintOptions();

                        return ((int)Globals.ExitReasons.InvalidOption);
                    }

                    bool isValid = option.AddParameter(token.Name);

                    if (!isValid)
                    {
                        CLIError(arg, "excessive parameters for option '" + option.Name + "' or invalid parameter");

                        return ((int)Globals.ExitReasons.InvalidParameter);
                    }
                }
            }

            try
            {
                directive.Execute();

                Console.WriteLine();
                Console.WriteLine(directive.Name + " successful");
            }
            catch (Exception ex)
            {
                CLIError(directive.Name, ex.Message);
                
                return ((int)Globals.ExitReasons.DirectiveFail);
            }

            return((int)Globals.ExitReasons.OK);
        }

        private static void OutputAbout()
        {
            Console.WriteLine("silly version " + Globals.Version);
            Console.WriteLine();
        }

        private static void OutputDirectives()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Usage: silly [directive] [options]");
            Console.ResetColor();
            Console.WriteLine("where [directive] is one of the following...");
            Console.WriteLine("");

            foreach(SillyDirective directive in SillyDirective.KnownDirectives())
            {
                Console.WriteLine(directive.Name + "\t\t" + directive.Description);
            }
        }

        private static void CLIError(string arg, string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("'" + arg + "' : " + error);
            Console.ResetColor();
        }

        private static void OutputFinished()
        {
            Console.WriteLine("Finished. Bye.");
        }
    }
}
