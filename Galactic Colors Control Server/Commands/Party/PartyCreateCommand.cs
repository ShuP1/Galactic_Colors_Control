using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PartyCreateCommand : ICommand
    {
        public string Name { get { return "create"; } }
        public string DescText { get { return "Create new party."; } }
        public string HelpText { get { return "Use /party create [name] [size] to create new party."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.party; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 2; } }
        public int maxArgs { get { return 2; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            if (server || (!server && Program.clients[soc].partyID == -1))
            {
                int size;
                if (int.TryParse(args[3], out size))
                {
                    if (size > 0)
                    {
                        if (size <= Program.config.size)
                        {
                            Logger.Write("Party " + args[2] + " create with " + size + " slots as " + Program.partyID, Logger.logType.info);
                            Program.AddParty(new Party(args[2], size, Utilities.GetName(soc)));
                            if (server)
                            {
                                Program.selectedParty = Program.partyID-1;
                            }
                            else
                            {
                                Program.clients[soc].partyID = Program.partyID-1;
                                Utilities.Return("Party " + args[2] + " create with " + size + " slots as " + (Program.partyID-1), soc, server);
                            }
                        }
                        else
                        {
                            Utilities.Return("Too big size", soc, server);
                        }
                    }
                    else
                    {
                        Utilities.Return("Too small size", soc, server);
                    }
                }
                else
                {
                    Utilities.Return("Incorrect argument " + args[3], soc, server);
                }
            }
            else
            {
                Utilities.Return("Allready in a party.", soc, server);
            }
        }
    }
}
