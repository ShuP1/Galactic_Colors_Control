using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PartyStopCommand : ICommand
    {
        public string Name { get { return "stop"; } }
        public string DescText { get { return "Stop party."; } }
        public string HelpText { get { return "Use /party stop to stop current party"; } }
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
                foreach(Socket client in Program.parties[partyId].clients)
                {
                    Manager.Execute(new string[4] { "party","kick",Utilities.GetName(client),"stop_party" } ,soc,server);
                }
                Logger.Write("Party " + Program.parties[partyId].name + " closed", Logger.logType.info);
                if(Program.selectedParty == partyId) { Program.selectedParty = -1; }
                Program.parties.Remove(partyId);
            }
        }
    }
}
