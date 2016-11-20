using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ServerStatusCommand : ICommand
    {
        public string Name { get { return "status"; } }
        public string DescText { get { return "Shows server status."; } }
        public string HelpText { get { return "Use 'server status' to display server actual status."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.server; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return false; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            string text = "";
            text += "Server : " + (Server._open ? "open" : "close");
            text += "Clients : " + Server.clients.Count + "/" + Server.config.size;
            text += "Parties : " + Server.parties.Count;
            return new RequestResult(ResultTypes.OK, Common.Strings(text));
        }
    }
}