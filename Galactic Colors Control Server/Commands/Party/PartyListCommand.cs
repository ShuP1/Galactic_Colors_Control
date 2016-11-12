using Galactic_Colors_Control_Common.Protocol;
using System;
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
            string[] text = new string[Program.parties.Keys.Count];
            int i = 0;
            foreach (int key in Program.parties.Keys)
            {
                Party party = Program.parties[key];
                text[i] = (key + " : " + party.name + " : " + party.count + "/" + party.size + " : " + (party.open ? (party.isPrivate ? "private" : "open") : "close") + Environment.NewLine + " ");
                i++;
            }
            return new RequestResult(ResultTypes.OK, text);
        }
    }
}