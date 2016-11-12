using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PartyCreateCommand : ICommand
    {
        public string Name { get { return "create"; } }
        public string DescText { get { return "Create new party."; } }
        public string HelpText { get { return "Use 'party create [name] [size]' to create new party."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.party; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 2; } }
        public int maxArgs { get { return 2; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            if (!server && Program.clients[soc].partyID != -1)
                return new RequestResult(ResultTypes.Error, Common.Strings("Allready"));

            int size;
            if (!int.TryParse(args[3], out size))
                return new RequestResult(ResultTypes.Error, Common.Strings("Format"));

            if (size < 1)
                return new RequestResult(ResultTypes.Error, Common.Strings("Too Small"));

            if (size > Program.config.size)
                return new RequestResult(ResultTypes.Error, Common.Strings("Too Big"));

            Program.AddParty(new Party(args[2], size, Utilities.GetName(soc)));
            Logger.Write("Party " + args[2] + " create with " + size + " slots as " + Program.GetPartyID(false), Logger.logType.info);
            if (server)
            {
                Program.selectedParty = Program.GetPartyID(false);
            }
            else
            {
                Program.clients[soc].partyID = Program.GetPartyID(false);
            }
            return new RequestResult(ResultTypes.OK, new string[3] { args[2], size.ToString(), (Program.GetPartyID(false)).ToString() });
        }
    }
}