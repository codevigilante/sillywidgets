using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SillyWidgets;
using SillyWidgets.Utilities.Server;

namespace dotnet_silly
{
    class Program
    {
        delegate void Command(string[] args);

        private static Dictionary<string, Command> CommandMap = new Dictionary<string, Command>()
        {
            { "info", PrintHelp },
            { "serve", Serve }
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, silly.");

            string command = "info";

            if (args.Length > 0)
            {
                command = args[0];
            }

            Command commandFunc = null;

            if (CommandMap.TryGetValue(command.ToLower(), out commandFunc))
            {
                commandFunc(args);

                return;
            }

            Console.WriteLine("What am '" + command + "'?");
        }

        private static void Serve(string[] args)
        {
            SillyProxyApplication site = null;//new SillyWidgetsProxy();
            SillySiteServer testServer = new SillySiteServer(site);

            Task server = testServer.Start();

            server.Wait(); 
        }

        private static void PrintHelp(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Usage: dotnet silly [command] [option]");
            Console.ResetColor();
            Console.WriteLine("Where command is...");
            Console.WriteLine("");
            Console.WriteLine("{0,-15} {1,-40}", "info", "how to use this thing");
        }
    }
}
