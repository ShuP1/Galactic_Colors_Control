using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PartyOpenCommand : ICommand
    {
        public string Name { get { return "open"; } }
        public string DescText { get { return "Opens party."; } }
        public string HelpText { get { return "Use /party open to open party for join."; } }
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
            if (Utilities.AccessParty(ref partyId, true, soc, server))
            {
                if (!Program.parties[partyId].open)
                {
                    Program.parties[partyId].open = true;
                    Utilities.Return("Party opened", soc, server);
                }
                else
                {
                    Utilities.Return("Party allready open", soc, server);
                }
            }
        }
    }
}
