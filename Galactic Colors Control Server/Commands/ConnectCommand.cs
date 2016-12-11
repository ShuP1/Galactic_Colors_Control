using Galactic_Colors_Control_Common.Protocol;
using MyCommon;
using System.Net;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ConnectCommand : ICommand
    {
        public string Name { get { return "connect"; } }
        public string DescText { get { return "Gets an username."; } }
        public string HelpText { get { return "Use 'connect [username] [version]' to start identification"; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.root; } }
        public bool IsServer { get { return false; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return true; } }
        public int minArgs { get { return 2; } }
        public int maxArgs { get { return 2; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            if (Utilities.IsConnect(soc))
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("Connected"));

            if (args[2] != Protocol.version.ToString()) //Check client protocol version
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("Version", Protocol.version.ToString()));

            if (args[1].Length < 3)
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("TooShort"));

            Server.logger.Write("Identifiaction request from " + Utilities.GetName(soc), Logger.logType.debug);
            bool allreadyconnected = false;
            args[1] = args[1][0].ToString().ToUpper()[0] + args[1].Substring(1);
            foreach (Client client in Server.clients.Values)
            {
                if (client.pseudo == args[1]) { allreadyconnected = true; break; }
            }
            if (allreadyconnected)
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("AllreadyTaken"));

            lock (Server.clients_lock)
            {
                Server.clients[soc].status = 0;
                Server.clients[soc].pseudo = args[1];
            }
            Utilities.Broadcast(new EventData(EventTypes.ServerJoin, Strings.ArrayFromStrings(args[1])));
            Server.logger.Write("Identified as " + Utilities.GetName(soc) + " form " + ((IPEndPoint)soc.LocalEndPoint).Address.ToString(), Logger.logType.info);
            return new RequestResult(ResultTypes.OK, Strings.ArrayFromStrings(args[1]));
        }
    }
}