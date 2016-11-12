using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class TimeCommand : ICommand
    {
        public string Name { get { return "time"; } }
        public string DescText { get { return "Gives server time."; } }
        public string HelpText { get { return "Use 'time' to display server time."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.root; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            return new RequestResult(ResultTypes.OK, Common.Strings(DateTime.Now.ToLongTimeString()));
        }
    }
}