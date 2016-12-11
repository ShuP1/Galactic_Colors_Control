using Galactic_Colors_Control_Common.Protocol;
using MyCommon;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ClientCountCommand : ICommand
    {
        public string Name { get { return "count"; } }
        public string DescText { get { return "Counts connected clients."; } }
        public string HelpText { get { return "Use 'client count' to show connected clients count and size"; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.client; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            return new RequestResult(ResultTypes.OK, Strings.ArrayFromStrings(Server.clients.Count.ToString(), Server.config.size.ToString()));
        }
    }
}