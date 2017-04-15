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

                return ((int)Globals.ExitReasons.About);
            }

            SillyDirective command = SillyDirective.CreateDirective(args[0].ToLower());

            if (command == null)
            {
                Console.WriteLine("Unknown directive: " + args[0]);

                OutputDirectives();

                return ((int)Globals.ExitReasons.UnknownDirective);
            }

            try
            {
                command.Execute(args);

                Console.WriteLine(command.Command + " successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine(command.Command + " Failed with: " + ex.Message);

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
            string usageString = "Usage: silly [";

            foreach(string directive in SillyDirective.KnowDirectiveNames())
            {
                usageString += " " + directive;
            }

            usageString += " ] [ options ]";

            Console.WriteLine(usageString);
            Console.WriteLine();

            foreach(SillyDirective directive in SillyDirective.KnownDirectives())
            {
                string options = string.Empty;

                foreach(KeyValuePair<string, DirectiveOption> option in directive.Options)
                {
                    options += "[" + option.Key + "]" + " ";
                }

                Console.WriteLine(directive.Command + " " + options + " - " + directive.Description);
            }
        }

        private static void OutputFinished()
        {
            Console.WriteLine("Finished. Bye.");
        }
    }
}
