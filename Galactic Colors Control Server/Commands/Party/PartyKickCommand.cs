using Galactic_Colors_Control_Common.Protocol;
using MyCommon;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PartyKickCommand : ICommand
    {
        public string Name { get { return "kick"; } }
        public string DescText { get { return "Kick player from party."; } }
        public string HelpText { get { return "Use 'party kick [name] <reason>' to kick palyer from party"; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.party; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 1; } }
        public int maxArgs { get { return 2; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            int partyId = -1;
            if (!Utilities.AccessParty(ref partyId, args, true, soc, server))
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("Access"));

            Socket target = null;
            foreach (Socket client in Server.parties[partyId].clients)
            {
                if (Utilities.GetName(client) == args[2]) { target = client; }
            }
            if (target == null)
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("CantFind"));

            Utilities.Send(target, new EventData(EventTypes.PartyKick, args.Length > 3 ? Strings.ArrayFromStrings(args[2]) : null));
            return Manager.Execute(new string[2] { "party", "leave" }, target, false);
        }
    }
}