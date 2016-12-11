﻿using Galactic_Colors_Control_Common.Protocol;
using MyCommon;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ServerOpenCommand : ICommand
    {
        public string Name { get { return "open"; } }
        public string DescText { get { return "Open server."; } }
        public string HelpText { get { return "Use 'server open' to open server for connections"; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.server; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return false; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 0; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            if (Server._open)
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("Allready"));

            Server._open = true;
            Server.logger.Write("Server opened", Logger.logType.warm, Logger.logConsole.show);
            return new RequestResult(ResultTypes.OK);
        }
    }
}