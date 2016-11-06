using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PartyStatusCommand : ICommand
    {
        public string Name { get { return "status"; } }
        public string DescText { get { return "Shows party status."; } }
        public string HelpText { get { return "Use /party status to show party status."; } }
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
                Party party = Program.parties[partyId];
                string text = "";
                text += ("Name: " + party.name + Environment.NewLine);
                text += ("Count: " + party.count + "/" + party.size + Environment.NewLine);
                text += ("Status: " + (party.isPrivate ? "private" : (party.open ? "open" : "close")));
                Utilities.Return(text,soc,server);
            }
        }
    }
}
