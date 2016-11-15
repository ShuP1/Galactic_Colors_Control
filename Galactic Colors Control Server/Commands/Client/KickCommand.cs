using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ClientKickCommand : ICommand
    {
        public string Name { get { return "kick"; } }
        public string DescText { get { return "Kicks selected client."; } }
        public string HelpText { get { return "Use 'client kick [username] <reason>' to kick client from server."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.client; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return false; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return true; } }
        public int minArgs { get { return 1; } }
        public int maxArgs { get { return 2; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            Socket target = null;
            foreach (Socket client in Program.clients.Keys)
            {
                if (Utilities.GetName(client) == args[2]) { target = client; }
            }
            if (target == null)
                return new RequestResult(ResultTypes.Error, Common.Strings("Can't find"));

            Program.logger.Write(args[2] + " was kick by server.", Logger.logType.info, Logger.logConsole.show);
            if (args.Length > 2)
            {
                Utilities.Send(target, new EventData(EventTypes.ServerKick, Common.Strings(args[3])));
                Program.logger.Write("because" + args[3], Logger.logType.debug);
            }
            else
            {
                Utilities.Send(target, new EventData(EventTypes.ServerKick));
            }
            return new RequestResult(ResultTypes.OK);
        }
    }
}