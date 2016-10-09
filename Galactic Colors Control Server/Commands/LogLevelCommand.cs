using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class LogLevelCommand : ICommand
    {
        public string Name { get { return "loglevel"; } }
        public string DescText { get { return "Change console loglevel."; } }
        public string HelpText { get { return "Use /loglevel [loglevel] to change Loglevel."; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 1; } }
        public int maxArgs { get { return 1; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            if (Enum.TryParse(args[1], true, out Program.config.logLevel))
            {
                Utilities.ConsoleWrite("LogLevel: " + Program.config.logLevel.ToString());
            }
            else
            {
                Utilities.ConsoleWrite("Incorrect argument (debug, info, important, error, fatal)");
            }
        }
    }
}
