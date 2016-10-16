using System;
using System.Net;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class ConnectCommand : ICommand
    {
        public string Name { get { return "connect"; } }
        public string DescText { get { return "Gets an username."; } }
        public string HelpText { get { return "Use /connect [username] to start identification"; } }
        public bool IsServer { get { return false; } }
        public bool IsClient { get { return true; } }
        public bool IsNoConnect { get { return true; } }
        public int minArgs { get { return 1; } }
        public int maxArgs { get { return 1; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            if (!Utilities.IsConnect(soc))
            {
                Logger.Write("Identifiaction request from " + Utilities.GetName(soc), Logger.logType.debug);
                bool allreadyconnected = false;
                foreach(Data client in Program.clients.Values)
                {
                    if(client.pseudo == args[1]) { allreadyconnected = true; break; }
                }
                if (!allreadyconnected)
                {
                    Program.clients[soc].status = 0;
                    //args[1] = args[1][0].ToString().ToUpper()[0] + args[1].Substring(1);
                    Program.clients[soc].pseudo = args[1];
                    Utilities.Send(soc, "Identified as " + args[1], Utilities.dataType.message);
                    Utilities.Broadcast(args[1] + " joined the server", Utilities.dataType.message);
                    Logger.Write("Identified as " + Utilities.GetName(soc) + " form " + ((IPEndPoint)soc.LocalEndPoint).Address.ToString(), Logger.logType.info);
                }
                else
                {
                    Utilities.Send(soc, "/kick username_allready_taken", Utilities.dataType.message);
                }
            }
            else
            {
                Utilities.Send(soc, "You are allready " + Utilities.GetName(soc), Utilities.dataType.message);
            }
        }
    }
}
