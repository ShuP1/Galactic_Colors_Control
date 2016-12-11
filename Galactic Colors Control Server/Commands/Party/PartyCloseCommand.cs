using Galactic_Colors_Control_Common.Protocol;
using MyCommon;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PartyCloseCommand : ICommand
    {
        public string Name { get { return "close"; } }
        public string DescText { get { return "Closes party."; } }
        public string HelpText { get { return "Use 'party close' to close party for join."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.party; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            int partyId = -1;
            if (!Utilities.AccessParty(ref partyId, args, true, soc, server))
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("Access"));

            if (!Server.parties[partyId].open)
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("Allready"));

            Server.parties[partyId].open = false;
            return new RequestResult(ResultTypes.OK);
        }
    }
}