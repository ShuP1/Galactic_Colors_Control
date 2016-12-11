using Galactic_Colors_Control_Common.Protocol;
using MyCommon;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PartyListCommand : ICommand
    {
        public string Name { get { return "list"; } }
        public string DescText { get { return "Shows parties list."; } }
        public string HelpText { get { return "Use 'party list' to show parties list."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.party; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            if (Server.parties.Keys.Count == 0)
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("AnyParty"));

            string[] text = new string[Server.parties.Keys.Count];
            int i = 0;
            foreach (int key in Server.parties.Keys)
            {
                Party party = Server.parties[key];
                text[i] = (key + " : " + party.name + " : " + party.count + "/" + party.size + " : " + (party.open ? (party.isPrivate ? "private" : "open") : "close"));
                i++;
            }
            return new RequestResult(ResultTypes.OK, text);
        }
    }
}