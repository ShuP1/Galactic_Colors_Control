using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class KickCommand : ICommand
    {
        public string Name { get { return "kick"; } }
        public string DescText { get { return "Kicks selected client."; } }
        public string HelpText { get { return "Use /kick [username] <reason> to kick client from server."; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return false; } }
        public bool IsNoConnect { get { return true; } }
        public int minArgs { get { return 1; } }
        public int maxArgs { get { return 2; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            Socket target = null;
            foreach(Socket client in Program.clients.Keys)
            {
                if(Utilities.GetName(client) == args[1]) { target = client; }
            }
            if (target != null)
            {
                Logger.Write(args[1] + " was kick by server.", Logger.logType.info);
                if (args.Length > 2)
                {
                    Utilities.Send(target, "/kick " + args[2], Utilities.dataType.message);
                    Logger.Write("because" + args[1], Logger.logType.debug);
                }
                else {
                    Utilities.Send(target, "/kick", Utilities.dataType.message);
                }
            }
            else
            {
                Utilities.Return("Can't find " + args[1], soc, server);
            }
        }
    }
}
