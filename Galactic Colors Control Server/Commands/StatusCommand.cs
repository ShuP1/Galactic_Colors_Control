using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class StatusCommand : ICommand
    {
        public string Name { get { return "status"; } }
        public string DescText { get { return "Shows server status."; } }
        public string HelpText { get { return "Use /status to display server actual status."; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            if (Program._open)
            {
                Utilities.ConsoleWrite("Server open");
            }
            else {
                Utilities.ConsoleWrite("Server close");
            }
        }
    }
}
