using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ServerCloseCommand : ICommand
    {
        public string Name { get { return "close"; } }
        public string DescText { get { return "Close server."; } }
        public string HelpText { get { return "Use 'server close' to close server for connections"; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.server; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return false; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            if (!Program._open)
                return new RequestResult(ResultTypes.Error, Common.Strings("Allready"));

            Program._open = false;
            Program.logger.Write("Server closed", Logger.logType.warm, Logger.logConsole.show);
            return new RequestResult(ResultTypes.OK);
        }
    }
}