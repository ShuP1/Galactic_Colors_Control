using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class SayCommand : ICommand
    {
        public string Name { get { return "say"; } }
        public string DescText { get { return "Said something."; } }
        public string HelpText { get { return "Use 'say [text]' to said something."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.root; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 1; } }
        public int maxArgs { get { return 1; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            if (args[1].Length == 0)
                return new RequestResult(ResultTypes.Error, Common.Strings("Any Message"));

            if (!Utilities.IsConnect(soc))
                return new RequestResult(ResultTypes.Error, Common.Strings("Must Be Connected"));

            int party = -1;
            party = Utilities.GetParty(soc);
            Utilities.BroadcastParty(new EventData(EventTypes.ChatMessage, Common.Strings(Utilities.GetName(soc) + " : " + args[1])), party);
            return new RequestResult(ResultTypes.OK);
        }
    }
}