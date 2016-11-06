using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PartyLeaveCommand : ICommand
    {
        public string Name { get { return "leave"; } }
        public string DescText { get { return "Leave party."; } }
        public string HelpText { get { return "Use /party leave to leave current party"; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.party; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            if (server)
            {
                Program.selectedParty = -1;
            }
            else
            {
                int partyId = -1;
                if (Utilities.AccessParty(ref partyId, false, soc, server))
                {
                    if (!Program.parties[partyId].IsOwner(Utilities.GetName(soc)))
                    {
                        Utilities.BroadcastParty(Utilities.GetName(soc) + " leave the party", Galactic_Colors_Control_Common.Common.dataType.message, partyId);
                        Program.clients[soc].partyID = -1;
                    }
                    else
                    {
                        Utilities.Return("Owner can't leave party. Look for /party stop", soc, server);
                    }
                }
            }
        }
    }
}
