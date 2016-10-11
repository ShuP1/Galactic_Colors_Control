using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ClearCommand : ICommand
    {
        public string Name { get { return "clear"; } }
        public string DescText { get { return "Clears the console screen."; } }
        public string HelpText { get { return "Use /clear to execute Console.Clear()."; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return true; } }
        public bool IsNoConnect { get { return true; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            Console.Clear();
            Console.Write(">");
        }
    }
}
