using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PartyPasswordCommand : ICommand
    {
        public string Name { get { return "password"; } }
        public string DescText { get { return "Set party password."; } }
        public string HelpText { get { return "Use /party password [newPass] [oldPass] to set party private with password."; } }
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
                if (args.Length == 3)
                {
                    Array.Resize(ref args, 4);
                    args[3] = "";
                }
                if (Program.parties[partyId].SetPassword(args[2],args[3]))
                {
                    Utilities.Return("Password changed", soc, server);
                }
                else
                {
                    Utilities.Return("Can't change password", soc, server);
                }
            }
        }
    }
}
