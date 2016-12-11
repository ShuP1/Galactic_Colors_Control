using Galactic_Colors_Control_Common.Protocol;
using MyCommon;
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
            if (!server && Server.clients[soc].partyID != -1)
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("Allready"));

            int size;
            if (!int.TryParse(args[3], out size))
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("Format"));

            if (size < 1)
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("TooSmall"));

            if (size > Server.config.size)
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("TooBig"));

            if (Server.parties.Count >= Server.config.partysize)
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("Full"));

            Server.AddParty(new Party(args[2], size, Utilities.GetName(soc)));
            Server.logger.Write("Party " + args[2] + " create with " + size + " slots as " + Server.GetPartyID(false), Logger.logType.info);
            if (server)
            {
                Server.selectedParty = Server.GetPartyID(false);
            }
            else
            {
                Server.clients[soc].partyID = Server.GetPartyID(false);
            }
            return new RequestResult(ResultTypes.OK, new string[3] { args[2], size.ToString(), (Server.GetPartyID(false)).ToString() });
        }
    }
}