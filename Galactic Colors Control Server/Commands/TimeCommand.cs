using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class TimeCommand : ICommand
    {
        public string Name { get { return "time"; } }
        public string DescText { get { return "Gives server time."; } }
        public string HelpText { get { return "Use /time to display server time. (format is server dependent)"; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.root; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            Utilities.Return(DateTime.Now.ToLongTimeString(), soc, server);
        }
    }
}
