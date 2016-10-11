using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PingCommand : ICommand
    {
        public string Name { get { return "ping"; } }
        public string DescText { get { return "Clears the console screen."; } }
        public string HelpText { get { return "Use /ping to display our ping."; } }
        public bool IsServer { get { return false; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return true; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {

        }
    }
}
