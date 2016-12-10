using Galactic_Colors_Control;
using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System.Reflection;
using System.Threading;
using Consol = Galactic_Colors_Control_Common.Console;
using MyCommon;

namespace Galactic_Colors_Control_Console
{
    /// <summary>
    /// Console Client
    /// </summary>
    internal class Console
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
            System.Console.Title = "Galactic Colors Control Client"; //Start display
            System.Console.Write(">");
            logger.Write(System.Console.Title, Logger.logType.fatal);
            logger.Write("Console " + Assembly.GetEntryAssembly().GetName().Version.ToString(), Logger.logType.error);
            config = config.Load();
            logger.Initialise(config.logPath, config.logBackColor, config.logForeColor, config.logLevel, _debug, _dev);
            multilang.Initialise(Common.dictionary);
            client.OnEvent += new System.EventHandler(OnEvent); //Set OnEvent function
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
                        Consol.Write(new ColorStrings(new ColorString("Use"), new ColorString(" --debug", System.ConsoleColor.Red), new ColorString(" or"), new ColorString(" --dev", System.ConsoleColor.White, System.ConsoleColor.Red)));
                        break;
                }
            }
            bool hostSet = false;
            while (!hostSet) //Request hostname
            {
                Thread.Sleep(100);
                Consol.Write(new ColorStrings(multilang.GetWord("EnterHostname", config.lang) + ":"));
                string host = client.ValidateHost(System.Console.ReadLine());
                if (host[0] == '*')
                {
                    host = host.Substring(1);
                    logger.Write("Validate error " + host, Logger.logType.error);
                    Consol.Write(new ColorStrings(new ColorString(host, System.ConsoleColor.Blue)));
                    client.ResetHost();
                }
                else
                {
                    logger.Write("Validate " + host, Logger.logType.info);
                    Consol.Write(new ColorStrings(new ColorString(multilang.GetWord("Use", config.lang) + " "), new ColorString(host, System.ConsoleColor.Blue), new ColorString("? "), new ColorString("y", System.ConsoleColor.Green), new ColorString("/"), new ColorString("n", System.ConsoleColor.Red)));
                    System.ConsoleKeyInfo c = new System.ConsoleKeyInfo();
                    while (c.Key != System.ConsoleKey.Y && c.Key != System.ConsoleKey.N)
                    {
                        c = System.Console.ReadKey();
                    }
                    if (c.Key == System.ConsoleKey.Y)
                    {
                        hostSet = true;
                    }
                    else
                    {
                        client.ResetHost();
                    }
                }
            }
            Consol.Write(new ColorStrings(multilang.GetWord("Loading", config.lang)));
            if (client.ConnectHost()) //Try connection
            {
                logger.Write("Connected", Logger.logType.warm);
                run = true;
                bool connected = false;
                //Identifaction
                while (!connected)
                {
                    Consol.Write(new ColorStrings(multilang.GetWord("Username", config.lang) + ":"));
                    string username = System.Console.ReadLine();
                    if (username.Length > 3)
                    {
                        ResultData res = client.Request(new string[3] { "connect", username, Protocol.version.ToString() });
                        if (res.type == ResultTypes.OK) { connected = true; logger.Write("Identification", Logger.logType.info); }
                        else
                        {
                            logger.Write("Identification error " + res.result, Logger.logType.info);
                            Consol.Write(new ColorStrings(new ColorString(Parser.GetResultText(res, config.lang, multilang), System.ConsoleColor.Red)));
                        }
                    }
                    else
                    {
                        Consol.Write(new ColorStrings(new ColorString(multilang.GetWord("TooShort", config.lang), System.ConsoleColor.Red)));
                    }
                }
                bool inparty = false;
                while (!inparty)
                {
                    System.Console.Clear();
                    Consol.Write(new ColorStrings(Parser.GetResultText(client.Request(new string[2] { "party", "list" }), config.lang, multilang)));
                    Consol.Write(new ColorStrings(multilang.GetWord("Party", config.lang) + ":" + System.Environment.NewLine + "     (<id> [password] or 'c' for create)"));
                    string[] data = Common.SplitArgs(System.Console.ReadLine());
                    if (data.Length > 0)
                    {
                        if (data[0] == "c")
                        {
                            Consol.Write(new ColorStrings("<party name> <player count>:"));
                            string[] split = Common.SplitArgs(System.Console.ReadLine());
                            if (split.Length == 2)
                            {
                                ResultData createRes = client.Request(new string[4] { "party", "create", split[0], split[1] });
                                if (createRes.type == ResultTypes.OK) { inparty = true; }
                                else
                                {
                                    Consol.Write(new ColorStrings(new ColorString(Parser.GetResultText(createRes, config.lang, multilang), System.ConsoleColor.Red)));
                                    System.Console.Read();
                                }
                            }
                            else
                            {
                                Consol.Write(new ColorStrings(new ColorString(multilang.GetWord("Format", config.lang), System.ConsoleColor.Red)));
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
                                    Consol.Write(new ColorStrings(new ColorString(Parser.GetResultText(res, config.lang, multilang), System.ConsoleColor.Red)));
                                }
                            }
                            else
                            {
                                Consol.Write(new ColorStrings(new ColorString(multilang.GetWord("Format", config.lang), System.ConsoleColor.Red)));
                            }
                        }
                    }
                }
                logger.Write("Play", Logger.logType.info, Logger.logConsole.hide);
                Consol.Write(new ColorStrings(new ColorString("P", System.ConsoleColor.Red), new ColorString("L", System.ConsoleColor.Green), new ColorString("A", System.ConsoleColor.Blue), new ColorString("Y", System.ConsoleColor.White)));
                while (run)
                {
                    Execute(System.Console.ReadLine()); //Process console input
                    if (!client.isRunning) { run = false; }
                }
                System.Console.Read();
            }
            else
            {
                logger.Write("Connection error", Logger.logType.error);
                Consol.Write(new ColorStrings(new ColorString(multilang.GetWord("CantConnect", config.lang), System.ConsoleColor.Red)));
            }
            run = false;
            logger.Join();
            System.Console.Read();
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
            Consol.Write(new ColorStrings(Parser.GetResultText(client.Request(req), config.lang, multilang)));
        }

        private static void OnEvent(object sender, System.EventArgs e)
        {
            //TODO add PartyKick
            EventData eve = ((EventDataArgs)e).Data;
            Consol.Write(new ColorStrings(Parser.GetEventText(eve, config.lang, multilang)));
        }
    }
}