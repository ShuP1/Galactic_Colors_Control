using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class BroadcastCommand : ICommand
    {
        public string DescText { get { return "Sends message to all clients."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.root; } }
        public string HelpText { get { return "Use 'broadcast [text]' to send message to all clients."; } }
        public bool IsClient { get { return false; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public bool IsServer { get { return true; } }
        public int maxArgs { get { return 1; } }
        public int minArgs { get { return 1; } }
        public string Name { get { return "broadcast"; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            Utilities.Broadcast(new EventData(EventTypes.ChatMessage, Common.Strings("Server : " + args[1])));
            return new RequestResult(ResultTypes.OK);
        }
    }
}