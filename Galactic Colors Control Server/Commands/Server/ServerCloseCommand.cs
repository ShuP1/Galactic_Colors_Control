using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ServerCloseCommand : ICommand
    {
        public string Name { get { return "close"; } }
        public string DescText { get { return "Close server."; } }
        public string HelpText { get { return "Use /server close to close server for connections"; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.server; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return false; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 1; } }

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
