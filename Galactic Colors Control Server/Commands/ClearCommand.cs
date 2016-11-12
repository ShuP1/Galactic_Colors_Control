using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ClearCommand : ICommand
    {
        public string Name { get { return "clear"; } }
        public string DescText { get { return "Clears the console screen."; } }
        public string HelpText { get { return "Use 'clear' to execute Console.Clear()."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.root; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return true; } }
        public bool IsNoConnect { get { return true; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            if (server)
            {
                Console.Clear();
                Console.Write(">");
                return new RequestResult(ResultTypes.OK);
            }
            else
            {
                return new RequestResult(ResultTypes.Error, Common.Strings("Client Side"));
            }
        }
    }
}