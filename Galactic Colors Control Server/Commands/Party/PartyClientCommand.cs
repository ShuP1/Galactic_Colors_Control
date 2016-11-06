using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PartyClientCommand : ICommand
    {
        public string Name { get { return "client"; } }
        public string DescText { get { return "Lists party clients."; } }
        public string HelpText { get { return "Use /party client to show party clients list."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.party; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            int partyId = -1;
            if (Utilities.AccessParty(ref partyId, false, soc, server))
            {
                string text = "  ";
                foreach(Socket client in Program.parties[partyId].clients)
                {
                    text += (Utilities.GetName(client) + Environment.NewLine + "  ");
                }
                Utilities.Return(text, soc, server);
            }
        }
    }
}
