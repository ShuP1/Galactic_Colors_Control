using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class PartyJoinCommand : ICommand
    {
        public string Name { get { return "join"; } }
        public string DescText { get { return "Join a party."; } }
        public string HelpText { get { return "Use /party join [id] <password> to join a party."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.party; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 1; } }
        public int maxArgs { get { return 2; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            if ((server && Program.selectedParty == -1) || (!server && Program.clients[soc].partyID == -1))
            {
                int id;
                if (int.TryParse(args[2], out id))
                {
                    if (Program.parties.ContainsKey(id))
                    {
                        Party party = Program.parties[id];
                        if (args.Length == 3)
                        {
                            Array.Resize(ref args, 4);
                            args[3] = "";
                        }
                        if (server || (!server && party.TestPassword(args[3])))
                        {
                            if(server)
                            {
                                Program.selectedParty = id;
                            }
                            else
                            {
                                if(party.open)
                                {
                                    if (party.clients.Count < party.size)
                                    {
                                        Program.clients[soc].partyID = id;
                                        Utilities.BroadcastParty(Utilities.GetName(soc) + " join the party", Galactic_Colors_Control_Common.Common.dataType.message, id);
                                    }
                                    else
                                    {
                                        Utilities.Return("Party is full", soc, server);
                                    }
                                }
                                else
                                {
                                    Utilities.Return("Party close", soc, server);
                                }
                            }
                        }
                        else
                        {
                            Utilities.Return("Wrong party password", soc, server);
                        }
                    }
                    else
                    {
                        Utilities.Return("Can't find party " + id, soc, server);
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
