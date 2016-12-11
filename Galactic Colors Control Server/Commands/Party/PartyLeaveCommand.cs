using Galactic_Colors_Control_Common.Protocol;
using MyCommon;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PartyLeaveCommand : ICommand
    {
        public string Name { get { return "leave"; } }
        public string DescText { get { return "Leave party."; } }
        public string HelpText { get { return "Use 'party leave' to leave current party"; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.party; } }
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
                Server.selectedParty = -1;
                return new RequestResult(ResultTypes.OK);
            }
            else
            {
                int partyId = -1;
                if (!Utilities.AccessParty(ref partyId, args, false, soc, server))
                    return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("Access"));

                if (Server.parties[partyId].IsOwner(Utilities.GetName(soc)))
                    return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("Owner"));

                Server.clients[soc].partyID = -1;
                Utilities.BroadcastParty(new EventData(EventTypes.PartyLeave, Strings.ArrayFromStrings(Utilities.GetName(soc))), partyId);
                return new RequestResult(ResultTypes.OK);
            }
        }
    }
}