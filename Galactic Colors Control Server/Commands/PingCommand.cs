using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PingCommand : ICommand
    {
        public string Name { get { return "ping"; } }
        public string DescText { get { return "Clears the console screen."; } }
        public string HelpText { get { return "Use 'ping' to display our ping."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.root; } }
        public bool IsServer { get { return false; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return true; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            return new RequestResult(ResultTypes.Error, Common.Strings("ClientSide"));
        }
    }
}