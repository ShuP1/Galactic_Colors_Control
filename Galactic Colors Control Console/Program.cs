using Galactic_Colors_Control;
using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System;
using System.Reflection;
using System.Threading;

namespace Galactic_Colors_Control_Console
{
    /// <summary>
    /// Console Client
    /// </summary>
    internal class COnsole
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
            Console.Title = "Galactic Colors Control Client"; //Start display
            Console.Write(">");
            logger.Write(Console.Title, Logger.logType.fatal);
            logger.Write("Console " + Assembly.GetEntryAssembly().GetName().Version.ToString(), Logger.logType.error);
            config = config.Load();
            logger.Initialise(config.logPath, config.logBackColor, config.logForeColor, config.logLevel, _debug, _dev);
            multilang.Load();
            client.OnEvent += new EventHandler(OnEvent); //Set OnEvent function         
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
                Thread.Sleep(100);
                Common.ConsoleWrite(multilang.Get("EnterHostname", config.lang) +":");
                string host = client.ValidateHost(Console.ReadLine());
                if (host[0] == '*')
                {
                    host = host.Substring(1);
                    logger.Write("Validate error " + host, Logger.logType.error);
                    Common.ConsoleWrite(host, ConsoleColor.Red);
                    client.ResetHost();
                }
                else
                {
                    logger.Write("Validate " + host, Logger.logType.info);
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
            Common.ConsoleWrite(multilang.Get("Loading", config.lang));
            if (client.ConnectHost()) //Try connection
            {
                logger.Write("Connected", Logger.logType.warm);
                run = true;
                bool connected = false;
                //Identifaction
                while (!connected)
                {
                    Common.ConsoleWrite(multilang.Get("Username", config.lang) + ":");
                    string username = Console.ReadLine();
                    if (username.Length > 3)
                    {
                        ResultData res = client.Request(new string[3] { "connect", username, Protocol.version.ToString() });
                        if(res.type == ResultTypes.OK) { connected = true; logger.Write("Identification", Logger.logType.info); }
                        else
                        {
                            logger.Write("Identification error " + res.result, Logger.logType.info);
                            Common.ConsoleWrite(multilang.GetResultText(res, config.lang));
                        }
                    }
                    else
                    {
                        Common.ConsoleWrite(multilang.Get("TooShort", config.lang));
                    }
                }
                bool inparty = false;
                while (!inparty)
                {
                    Console.Clear();
                    Common.ConsoleWrite(multilang.GetResultText(client.Request(new string[2] { "party", "list" }), config.lang));
                    Common.ConsoleWrite(multilang.Get("Party", config.lang) + ":" + Environment.NewLine + "     (<id> [password] or 'c' for create)");
                    string[] data = Common.SplitArgs(Console.ReadLine());
                    if (data.Length > 0)
                    {
                        if (data[0] == "c")
                        {
                            Common.ConsoleWrite("<party name> <player count>:");
                            string[] split = Common.SplitArgs(Console.ReadLine());
                            if (split.Length == 2)
                            {
                                ResultData createRes = client.Request(new string[4] { "party", "create", split[0], split[1] });
                                if (createRes.type == ResultTypes.OK) { inparty = true; }
                                else
                                {
                                    Common.ConsoleWrite(multilang.GetResultText(createRes, config.lang));
                                    Console.ReadLine();
                                }
                            }
                            else
                            {
                                Common.ConsoleWrite("Format");
                            }
                        }
                        else
                        {
                            int id;
                            if (int.TryParse(data[0], out id))
                            {
                                string[] request = data.Length == 1 ? new string[3] { "party", "join", id.ToString() } : new string[4] { "party", "join", id.ToString(), data[1] };
                                ResultData res = client.Request(request);
                                if (res.type == ResultTypes.OK) { inparty = true; }
                                else
                                {
                                    Common.ConsoleWrite(multilang.GetResultText(res, config.lang));
                                }
                            }
                            else
                            {
                                Common.ConsoleWrite("Format");
                            }
                        }
                    }
                }
                logger.Write("Play", Logger.logType.info, Logger.logConsole.hide);
                Common.ConsoleWrite(multilang.Get("Play", config.lang));
                while (run)
                {
                    Execute(Console.ReadLine()); //Process console input
                    if (!client.isRunning) { run = false; }
                }
                Console.Read();
            }
            else
            {
                logger.Write("Connection error", Logger.logType.error);
                Common.ConsoleWrite(multilang.Get("CantConnect", config.lang), ConsoleColor.Red);
            }
            run = false;
            logger.Join();
            Console.ReadLine();
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