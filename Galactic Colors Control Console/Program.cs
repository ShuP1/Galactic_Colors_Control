using Galactic_Colors_Control;
using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System;
using System.Reflection;

namespace Galactic_Colors_Control_Console
{
    /// <summary>
    /// Console Client
    /// </summary>
    internal class Program
    {
        public static bool _debug = false;
        public static bool _dev = false;

        private static Client client = new Client();
        private static MultiLang multilang = new MultiLang(); //TODO use multilang
        public static Config config = new Config();
        public static Logger logger = new Logger();
        private static bool run = true;

        private static void Main(string[] args)
        {
            config = config.Load();
            logger.Initialise(config.logPath, config.logBackColor, config.logForeColor, config.logLevel);
            multilang.Load();
            client.OnEvent += new EventHandler(OnEvent); //Set OnEvent function
            Console.Title = "Galactic Colors Control Client"; //Start display
            Console.Write(">");
            Common.ConsoleWrite("Galactic Colors Control Client", ConsoleColor.Red);
            Common.ConsoleWrite("Console " + Assembly.GetEntryAssembly().GetName().Version.ToString(), ConsoleColor.Yellow);
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "--debug":
                        _debug = true;
                        logger.Write("CLIENT IS IN DEBUG MODE !", Logger.logType.error, Logger.logConsole.show);
                        break;

                    case "--dev":
                        _dev = true;
                        logger.Write("CLIENT IS IN DEV MODE !", Logger.logType.error, Logger.logConsole.show);
                        break;

                    default:
                        Common.ConsoleWrite("Use --debug or --dev");
                        break;
                }
            }
            bool hostSet = false;
            while (!hostSet) //Request hostname
            {
                Common.ConsoleWrite("Enter server host:");
                string host = client.ValidateHost(Console.ReadLine());
                if (host[0] == '*')
                {
                    host = host.Substring(1);
                    Common.ConsoleWrite(host, ConsoleColor.Red);
                    client.ResetHost();
                }
                else
                {
                    Common.ConsoleWrite("Use " + host + "? y/n");
                    ConsoleKeyInfo c = new ConsoleKeyInfo();
                    while (c.Key != ConsoleKey.Y && c.Key != ConsoleKey.N)
                    {
                        c = Console.ReadKey();
                    }
                    if (c.Key == ConsoleKey.Y)
                    {
                        hostSet = true;
                    }
                    else
                    {
                        client.ResetHost();
                    }
                }
            }
            if (client.ConnectHost()) //Try connection
            {
                run = true;
                while (run)
                {
                    Execute(Console.ReadLine()); //Process console input
                    if (!client.isRunning) { run = false; }
                }
                Console.Read();
            }
            else
            {
                Common.ConsoleWrite("Can't connect sorry. Bye", ConsoleColor.Red);
                Console.Read();
            }
        }

        private static void Execute(string input)
        {
            if (input == null)
                return;

            if (input.Length == 0)
                return;

            string[] req;
            if (input[0] == config.commandChar)
            {
                input = input.Substring(1);
                req = Common.SplitArgs(input);
            }
            else
            {
                req = Common.Strings("say", input);
            }
            Common.ConsoleWrite(client.Request(req).ToSmallString()); //Add processing (common)
        }

        private static void OnEvent(object sender, EventArgs e)
        {
            EventData eve = ((EventDataArgs)e).Data;
            Common.ConsoleWrite(multilang.GetEventText(eve, config.lang));
        }
    }
}