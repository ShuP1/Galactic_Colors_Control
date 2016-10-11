using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class CloseCommand : ICommand
    {
        public string Name { get { return "close"; } }
        public string DescText { get { return "Closes server from connections."; } }
        public string HelpText { get { return "Use /close to stop connection process"; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return false; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            if (Program._open)
            {
                Program._open = false;
                Logger.Write("Server closed", Logger.logType.warm);
            }
            else
            {
                Utilities.ConsoleWrite("Server already close");
            }
        }
    }
}
