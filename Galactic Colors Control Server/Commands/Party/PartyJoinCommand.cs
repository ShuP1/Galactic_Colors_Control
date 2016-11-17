using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PartyJoinCommand : ICommand
    {
        public string Name { get { return "join"; } }
        public string DescText { get { return "Join a party."; } }
        public string HelpText { get { return "Use 'party join [id] <password>' to join a party."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.party; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 1; } }
        public int maxArgs { get { return 2; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            if ((server && Program.selectedParty != -1) || (!server && Program.clients[soc].partyID != -1))
                return new RequestResult(ResultTypes.Error, Common.Strings("Allready"));

            int id;
            if (!int.TryParse(args[2], out id))
                return new RequestResult(ResultTypes.Error, Common.Strings("Format"));

            if (!Program.parties.ContainsKey(id))
                return new RequestResult(ResultTypes.Error, Common.Strings("CantFind"));

            Party party = Program.parties[id];
            if (args.Length == 3)
            {
                Array.Resize(ref args, 4);
                args[3] = "";
            }
            if (!server && !party.TestPassword(args[3]))
                return new RequestResult(ResultTypes.Error, Common.Strings("Password"));

            if (server)
            {
                Program.selectedParty = id;
                return new RequestResult(ResultTypes.OK);
            }
            else
            {
                if (!party.open)
                    return new RequestResult(ResultTypes.Error, Common.Strings("Close"));

                if (party.clients.Count + 1 > party.size)
                    return new RequestResult(ResultTypes.Error, Common.Strings("Full"));

                Program.clients[soc].partyID = id;
                Utilities.BroadcastParty(new EventData(EventTypes.PartyJoin, Common.Strings(Utilities.GetName(soc))), id);
                return new RequestResult(ResultTypes.OK);
            }
        }
    }
}