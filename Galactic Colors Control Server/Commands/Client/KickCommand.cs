using Galactic_Colors_Control_Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ClientKickCommand : ICommand
    {
        public string Name { get { return "kick"; } }
        public string DescText { get { return "Kicks selected client."; } }
        public string HelpText { get { return "Use /client kick [username] <reason> to kick client from server."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.client; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return false; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return true; } }
        public int minArgs { get { return 1; } }
        public int maxArgs { get { return 2; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            Socket target = null;
            foreach(Socket client in Program.clients.Keys)
            {
                if(Utilities.GetName(client) == args[2]) { target = client; }
            }
            if (target != null)
            {
                Logger.Write(args[2] + " was kick by server.", Logger.logType.info);
                if (args.Length > 2)
                {
                    Utilities.Send(target, "/kick " + args[3], Common.dataType.message);
                    Logger.Write("because" + args[2], Logger.logType.debug);
                }
                else {
                    Utilities.Send(target, "/kick", Common.dataType.message);
                }
            }
            else
            {
                Utilities.Return("Can't find " + args[2], soc, server);
            }
        }
    }
}
