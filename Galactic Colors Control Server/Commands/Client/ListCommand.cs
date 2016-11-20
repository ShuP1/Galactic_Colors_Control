using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ClientListCommand : ICommand
    {
        public string Name { get { return "list"; } }
        public string DescText { get { return "Lists connected clients."; } }
        public string HelpText { get { return "Use 'client list' to display all connected client username or IP."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.client; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            if (server)
            {
                string text = "  ";
                foreach (Socket socket in Server.clients.Keys)
                {
                    text += (Utilities.GetName(socket) + ", ");
                }
                text = text.Remove(text.Length - 2, 2);
                return new RequestResult(ResultTypes.OK, Common.Strings(text));
            }
            else
            {
                string[] data = new string[Server.clients.Count];
                int i = 0;
                foreach (Socket socket in Server.clients.Keys)
                {
                    data[i] = (Utilities.GetName(socket) + ", ");
                    i++;
                }
                return new RequestResult(ResultTypes.OK, data);
            }
        }
    }
}