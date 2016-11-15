using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class LogLevelCommand : ICommand
    {
        public string Name { get { return "loglevel"; } }
        public string DescText { get { return "Change console loglevel."; } }
        public string HelpText { get { return "Use 'loglevel [loglevel]' to change Loglevel. (dev ,debug, info, warm, error, fatal)"; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.root; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return false; } }
        public bool IsClientSide { get { return true; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 1; } }
        public int maxArgs { get { return 1; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            if (Enum.TryParse(args[1], true, out Program.config.logLevel))
            {
                Program.logger.ChangeLevel(Program.config.logLevel);
                return new RequestResult(ResultTypes.OK, Common.Strings(Program.config.logLevel.ToString()));
            }
            else
            {
                return new RequestResult(ResultTypes.Error, Common.Strings("Incorrect argument"));
            }
        }
    }
}