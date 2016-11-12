using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
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
        public string HelpText { get { return "Use 'help [command]' to display command help. ('hell -all' for full help)"; } }
        public Manager.CommandGroup Group { get { return Manager.CommandGroup.root; } }
        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsClientSide { get { return false; } }
        public bool IsNoConnect { get { return false; } }
        public int minArgs { get { return 0; } }
        public int maxArgs { get { return 2; } }

        public RequestResult Execute(string[] args, Socket soc, bool server = false)
        {
            bool isGroup = false;
            bool isAll = false;
            if (args.Length == 2)
            {
                isGroup = Enum.GetNames(typeof(Manager.CommandGroup)).Contains(args[1]);
                isAll = (args[1] == "-all");
            }
            if (args.Length == 1 || (isGroup || isAll))
            {
                int maxLen = 0;
                List<ICommand> list = new List<ICommand>();
                foreach (ICommand com in Manager.commands)
                {
                    if (Manager.CanAccess(com, soc, server))
                    {
                        if (!isGroup || (isGroup && com.Group == (Manager.CommandGroup)Enum.Parse(typeof(Manager.CommandGroup), args[1])))
                        {
                            list.Add(com);
                            if (com.Name.Length + (com.Group == 0 ? 0 : 4) > maxLen) { maxLen = com.Name.Length + (com.Group == 0 ? 0 : 4); }
                        }
                    }
                }
                list.Sort((x, y) => x.Group.CompareTo(y.Group));
                string text = "Use 'help [command]' for more informations." + Environment.NewLine + "Available commands:" + Environment.NewLine;
                Manager.CommandGroup actualGroup = 0;
                foreach (ICommand com in list)
                {
                    if (com.Group != actualGroup)
                    {
                        text += (Environment.NewLine + "  " + com.Group.ToString() + Environment.NewLine + ((isGroup || isAll) ? "" : ("    Use 'help " + com.Group.ToString() + "'")));
                        actualGroup = com.Group;
                    }
                    if ((!(isGroup || isAll) && com.Group == 0) || (isGroup || isAll))
                    {
                        text += ("  " + (com.Group != 0 ? new string(' ', 4) : "") + com.Name + new string(' ', maxLen - com.Name.Length - (com.Group == 0 ? 0 : 4)) + " : " + com.DescText + Environment.NewLine);
                    }
                }
                return new RequestResult(ResultTypes.OK, Common.Strings(text));
            }
            else
            {
                ICommand command = null;
                args = args.Skip(1).ToArray();
                if (!Manager.TryGetCommand(args, ref command))
                    return new RequestResult(ResultTypes.Error, Common.Strings("Any Command"));

                if (!Manager.CanAccess(command, soc, server))
                    return new RequestResult(ResultTypes.Error, Common.Strings("Any Command"));

                return new RequestResult(ResultTypes.OK, Common.Strings(command.HelpText));
            }
        }
    }
}