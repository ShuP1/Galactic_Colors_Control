using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server.Commands
{
    public class HelpCommand : ICommand
    {
        public string Name { get { return "help"; } }
        public string DescText { get { return "Shows the help."; } }
        public string HelpText { get { return "Use /help [command] to display command help."; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 1; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            if(args.Length == 1)
            {
                List<string> list = new List<string>();
                int maxLen = 0;
                foreach (string com in Manager.commands.Keys)
                {
                    if(Manager.CanAccess(Manager.commands[com], soc, server))
                    {
                        list.Add(com);
                        if(com.Length > maxLen) { maxLen = com.Length; }
                    }
                }
                list.Sort();
                string text = "Use /help [command] for more informations." + Environment.NewLine + "Available commands:" + Environment.NewLine;
                foreach (var key in list)
                {
                    text += ("  " + key + new string(' ', maxLen - key.Length) + " : " + Manager.commands[key].DescText + Environment.NewLine);
                }
                Utilities.Return(text, soc, server);
            }
            else
            {
                if (Manager.commands.ContainsKey(args[1]))
                {
                    if (Manager.CanAccess(Manager.commands[args[1]], soc, server))
                    {
                        Utilities.Return(Manager.commands[args[1]].HelpText, soc, server);
                    }
                    else
                    {
                        Utilities.Return("Any help for " + args[1], soc, server);
                    }
                }
                else
                {
                    Utilities.Return("Any help for " + args[1], soc, server);
                }
            }
        }
    }
}
