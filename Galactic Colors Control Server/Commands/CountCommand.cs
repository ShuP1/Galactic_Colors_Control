using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class CountCommand : ICommand
    {
        public string Name { get { return "count"; } }
        public string DescText { get { return "Counts connected clients."; } }
        public string HelpText { get { return "Use /count to show connected clients count and size"; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            Utilities.ConsoleWrite(Program.clients.Count + "/" + Program.config.size);
        }
    }
}
