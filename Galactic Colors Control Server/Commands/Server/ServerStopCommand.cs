using Galactic_Colors_Control_Common;
using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ServerStopCommand : ICommand
    {
        public string Name { get { return "stop"; } }
        public string DescText { get { return "Stop the server."; } }
        public string HelpText { get { return "Use /server stop to completly stop server."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.server; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return false; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            Program._run = false;
            Utilities.ConsoleWrite("Stop server");
        }
    }
}
