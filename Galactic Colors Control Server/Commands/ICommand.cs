using Galactic_Colors_Control_Common.Protocol;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public interface ICommand
    {
        string Name { get; }
        string DescText { get; }
        string HelpText { get; }
        Manager.CommandGroup Group { get; }
        bool IsServer { get; }
        bool IsClient { get; }
        bool IsClientSide { get; }
        bool IsNoConnect { get; }
        int minArgs { get; }
        int maxArgs { get; }

        RequestResult Execute(string[] args, Socket soc = null, bool server = false);
    }
}