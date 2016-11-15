using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ExitCommand : ICommand
    {
        public string Name { get { return "exit"; } }
        public string DescText { get { return "Leave the server."; } }
        public string HelpText { get { return "Use 'exit' to stop actual program."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.root; } }
        public bool IsServer { get { return false; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return true; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            soc.Shutdown(SocketShutdown.Both);
            Program.logger.Write("Client disconnected from " + Utilities.GetName(soc), Logger.logType.info);
            string username = Utilities.GetName(soc);
            bool connected = Program.clients[soc].status != -1;
            soc.Close();
            Program.clients.Remove(soc);
            if (connected) { Utilities.Broadcast(new EventData(EventTypes.ServerLeave, Common.Strings(username))); }
            Program.logger.Write("Size: " + Program.clients.Count + "/" + Program.config.size, Logger.logType.debug);
            return new RequestResult(ResultTypes.OK);
        }
    }
}