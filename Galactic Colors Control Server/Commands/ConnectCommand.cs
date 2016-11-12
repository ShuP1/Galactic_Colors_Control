using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System.Net;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ConnectCommand : ICommand
    {
        public string Name { get { return "connect"; } }
        public string DescText { get { return "Gets an username."; } }
        public string HelpText { get { return "Use 'connect [username]' to start identification"; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.root; } }
        public bool IsServer { get { return false; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return true; } }
        public int minArgs { get { return 1; } }
        public int maxArgs { get { return 1; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            if (Utilities.IsConnect(soc))
                return new RequestResult(ResultTypes.Error, Common.Strings("Connected"));

            if (args[1].Length < 3)
                return new RequestResult(ResultTypes.Error, Common.Strings("Too Short"));

            Logger.Write("Identifiaction request from " + Utilities.GetName(soc), Logger.logType.debug);
            bool allreadyconnected = false;
            args[1] = args[1][0].ToString().ToUpper()[0] + args[1].Substring(1);
            foreach (Client client in Program.clients.Values)
            {
                if (client.pseudo == args[1]) { allreadyconnected = true; break; }
            }
            if (allreadyconnected)
                return new RequestResult(ResultTypes.Error, Common.Strings("Taken"));

            Program.clients[soc].status = 0;
            Program.clients[soc].pseudo = args[1];
            Utilities.Broadcast(new EventData(EventTypes.ServerJoin, Common.Strings(args[1])));
            Logger.Write("Identified as " + Utilities.GetName(soc) + " form " + ((IPEndPoint)soc.LocalEndPoint).Address.ToString(), Logger.logType.info);
            return new RequestResult(ResultTypes.OK, Common.Strings(args[1]));
        }
    }
}