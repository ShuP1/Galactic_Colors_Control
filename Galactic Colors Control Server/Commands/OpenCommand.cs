using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class OpenCommand : ICommand
    {
        public string Name { get { return "open"; } }
        public string DescText { get { return "Opens server for connections."; } }
        public string HelpText { get { return "Use /open to restart connection process"; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            if (!Program._open)
            {
                Program._open = true;
                Logger.Write("Server opened", Logger.logType.warm);
            }
            else
            {
                Utilities.ConsoleWrite("Server already open");
            }
        }
    }
}
