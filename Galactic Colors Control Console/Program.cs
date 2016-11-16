﻿using Galactic_Colors_Control;
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
        private static MultiLang multilang = new MultiLang();
        public static Config config = new Config();
        public static Logger logger = new Logger();
        private static bool run = true;

        private static void Main(string[] args)
        {
            config = config.Load();
            logger.Initialise(config.logPath, config.logBackColor, config.logForeColor, config.logLevel, _debug, _dev);
            multilang.Load();
            client.OnEvent += new EventHandler(OnEvent); //Set OnEvent function
            Console.Title ="Galactic Colors Control Client"; //Start display
            Console.Write(">");
            Common.ConsoleWrite(Console.Title, ConsoleColor.Red);
            Common.ConsoleWrite(multilang.Get("Console", config.lang) + " " + Assembly.GetEntryAssembly().GetName().Version.ToString(), ConsoleColor.Yellow);
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
                Common.ConsoleWrite(multilang.Get("EnterHostname", config.lang) +":");
                string host = client.ValidateHost(Console.ReadLine());
                if (host[0] == '*')
                {
                    host = host.Substring(1);
                    Common.ConsoleWrite(host, ConsoleColor.Red);
                    client.ResetHost();
                }
                else
                {
                    Common.ConsoleWrite(multilang.Get("Use", config.lang) + " " + host + "? y/n");
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
                Common.ConsoleWrite(multilang.Get("CantConnect", config.lang), ConsoleColor.Red);
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
            Common.ConsoleWrite(multilang.GetResultText(client.Request(req),config.lang));
        }

        private static void OnEvent(object sender, EventArgs e)
        {
            //TODO add PartyKick
            EventData eve = ((EventDataArgs)e).Data;
            Common.ConsoleWrite(multilang.GetEventText(eve, config.lang));
        }
    }
}