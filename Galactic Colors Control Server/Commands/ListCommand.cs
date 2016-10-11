using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ListCommand : ICommand
    {
        public string Name { get { return "list"; } }
        public string DescText { get { return "Lists connected clients."; } }
        public string HelpText { get { return "Use /list to display all connected client username or IP."; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return false; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            string text = "  ";
            foreach (Socket socket in Program.clients.Keys)
            {
                text += (Utilities.GetName(socket) + ", ");
            }
            text = text.Remove(text.Length - 2, 2);
            Utilities.ConsoleWrite(text);
        }
    }
}
