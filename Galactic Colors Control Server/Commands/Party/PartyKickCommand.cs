using Galactic_Colors_Control_Common;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PartyKickCommand : ICommand
    {
        public string Name { get { return "kick"; } }
        public string DescText { get { return "Kick player from party."; } }
        public string HelpText { get { return "Use /party kick [name] <reason> to kick palyer from party"; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.party; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 1; } }
        public int maxArgs { get { return 2; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            int partyId = -1;
            if (Utilities.AccessParty(ref partyId, true, soc, server))
            {
                Socket target = null;
                foreach (Socket client in Program.parties[partyId].clients)
                {
                    if (Utilities.GetName(client) == args[2]) { target = client; }
                }
                if (target != null)
                {
                    Utilities.BroadcastParty(args[2] + " was kick", Common.dataType.message, partyId);
                    if (args.Length > 3)
                    {
                        Utilities.Send(target, "/party kick " + args[3], Common.dataType.message);
                    }
                    else
                    {
                        Utilities.Send(target, "/party kick", Common.dataType.message);
                    }
                    Manager.Execute(new string[2] { "party", "leave" },target,false);
                }
                else
                {
                    Utilities.Return("Can't find " + args[2] + "in party", soc, server);
                }
            }
        }
    }
}
