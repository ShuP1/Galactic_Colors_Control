using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System;
using System.Net;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ClientStatusCommand : ICommand
    {
        public string Name { get { return "status"; } }
        public string DescText { get { return "Get client status."; } }
        public string HelpText { get { return "Use 'client status [username]' to show client status."; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.client; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return false; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 1; } }
        public int maxArgs { get { return 1; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            Socket target = null;
            foreach (Socket client in Server.clients.Keys)
            {
                if (Utilities.GetName(client) == args[2]) { target = client; }
            }
            if (target == null)
                return new RequestResult(ResultTypes.Error, Common.Strings("CantFind"));

            string text = "";
            text += ("Name   : " + Utilities.GetName(target) + Environment.NewLine);
            text += ("IP     : " + ((IPEndPoint)target.LocalEndPoint).Address.ToString() + Environment.NewLine);
            if (Server.clients[target].party != null)
            {
                text += ("Party : " + Server.clients[target].party + Environment.NewLine);
            }
            return new RequestResult(ResultTypes.OK, Common.Strings(text));
        }
    }
}