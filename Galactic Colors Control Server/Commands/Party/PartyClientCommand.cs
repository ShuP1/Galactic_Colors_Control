using Galactic_Colors_Control_Common.Protocol;
using MyCommon;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PartyClientCommand : ICommand
    {
        public string Name { get { return "client"; } }
        public string DescText { get { return "Lists party clients."; } }
        public string HelpText { get { return "Use 'party client' to show party clients list."; } }
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
            if (!Utilities.AccessParty(ref partyId, args, false, soc, server))
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("Access"));

            string[] data = new string[Server.parties[partyId].clients.Count];
            int i = 0;
            foreach (Socket client in Server.parties[partyId].clients)
            {
                data[i] = Utilities.GetName(client);
                i++;
            }
            return new RequestResult(ResultTypes.OK, data);
        }
    }
}