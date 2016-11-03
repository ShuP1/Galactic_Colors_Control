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
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.root; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 2; } }

        public void Execute(string[] args, Socket soc, bool server = false)
        {
            if(args.Length == 1)
            {
                int maxLen = 0;
                List<ICommand> list = new List<ICommand>();
                foreach (ICommand com in Manager.commands)
                {
                    if(Manager.CanAccess(com, soc, server))
                    {
                        list.Add(com);
                        if(com.Name.Length + (com.Group == 0 ? 0 : 4) > maxLen) { maxLen = com.Name.Length + (com.Group == 0 ? 0 : 4); }
                    }
                }
                list.Sort((x,y) => x.Group.CompareTo(y.Group));
                string text = "Use /help [command] for more informations." + Environment.NewLine + "Available commands:" + Environment.NewLine;
                Manager.CommandGroup actualGroup = 0;
                foreach (ICommand com in list)
                {
                    if(com.Group != actualGroup)
                    {
                        text += (Environment.NewLine + "  " + com.Group.ToString() + Environment.NewLine);
                        actualGroup = com.Group;
                    }
                    text += ("  " + (com.Group != 0 ? new string(' ',4) : "") + com.Name + new string(' ', maxLen - com.Name.Length - (com.Group == 0 ? 0 : 4)) + " : " + com.DescText + Environment.NewLine);
                }
                Utilities.Return(text, soc, server);
            }
            else
            {
                ICommand command = null;
                args = args.Skip(1).ToArray();
                if (Manager.TryGetCommand(args, ref command))
                {
                    if (Manager.CanAccess(command, soc, server))
                    {
                        Utilities.Return(command.HelpText, soc, server);
                    }
                    else
                    {
                        Utilities.Return("Any help for " + Manager.CommandToString(args), soc, server);
                    }
                }
                else
                {
                    Utilities.Return("Any help for " + Manager.CommandToString(args), soc, server);
                }
            }
        }
    }
}
