using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ExitCommand : ICommand
    {
        public string Name { get { return "exit"; } }
        public string DescText { get { return "Leave the program."; } }
        public string HelpText { get { return "Use /exit to stop actual program."; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsNoConnect { get { return true; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            if (server)
            {
                Program._run = false;
                Utilities.ConsoleWrite("Exit server");
            }
            else
            {
                soc.Shutdown(SocketShutdown.Both);
                Logger.Write("Client disconnected from " + Utilities.GetName(soc), Logger.logType.info);
                soc.Close();
                Program.clients.Remove(soc);
                Logger.Write("Size: " + Program.clients.Count + "/" + Program.config.size, Logger.logType.debug);
            }
        }
    }
}
