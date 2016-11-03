using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;

namespace Galactic_Colors_Control_Server.Commands
{
    public class Manager
    {
        public static List<ICommand> commands { get; private set; } = new List<ICommand>();
        public enum CommandGroup { root, server, party, client}

        /// <summary>
        /// Find all ICommand and add them to commands
        /// </summary>
        public static void Load()
        {
            //C# is magic
            IEnumerable<ICommand> coms = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetInterfaces().Contains(typeof(ICommand)) && x.GetConstructor(Type.EmptyTypes) != null).Select(x => Activator.CreateInstance(x) as ICommand);
            foreach (ICommand com in coms)
            {
                commands.Add(com);
                Logger.Write("Added command " + com.Group.ToString() + " " + com.Name, Logger.logType.debug);
            }
        }

        /// <summary>
        /// Execute sended command
        /// </summary>
        /// <param name="args">command with args</param>
        /// <param name="soc">Sender socket</param>
        /// <param name="server">Is server?</param>
        public static void Execute(string[] args, Socket soc = null, bool server = false)
        {
            ICommand command = null;
            if (TryGetCommand(args, ref command))
            {
                if (CanAccess(command, soc, server))
                {
                    if (!server && command.IsClientSide)
                    {
                        Utilities.Return("It's a client side command", soc, server);
                    }
                    else
                    {
                        if (args.Length > command.minArgs)
                        {
                            if (args.Length - (command.Group == 0 ? 1 : 2) <= command.maxArgs)
                            {
                                command.Execute(args, soc, server);
                            }
                            else
                            {
                                Utilities.Return("Command " + CommandToString(command) + " require at most " + command.minArgs + " argument(s).", soc, server);
                            }
                        }

                        else
                        {
                            Utilities.Return("Command " + CommandToString(command) + " require at least " + command.minArgs + " argument(s).", soc, server);
                        }
                    }
                }
                else
                {
                    Utilities.Return("Unknown command : " + CommandToString(args), soc, server);
                }
            }
            else
            {
                Utilities.Return("Unknown command : " + CommandToString(args), soc, server);
            }
        }

        public static string CommandToString(ICommand command)
        {
            string text = "";
            if(command.Group != 0) { text += (command.Group.ToString() + " "); }
            text += command.Name;
            return text;
        }

        public static string CommandToString(string[] args)
        {
            if (args.Length > 0)
            {
                string text = "";
                foreach(string arg in args)
                {
                    text += (arg + " ");
                }
                return text;
            }
            else
            {
                return null;
            }
        }

        public static bool TryGetCommand(string[] args, ref ICommand command)
        {
            if (args.Length > 0)
            {
                List<string> groups = Enum.GetNames(typeof(CommandGroup)).ToList();
                CommandGroup group = 0;
                if (groups.Contains(args[0]))
                {
                    if (args.Length > 1)
                    {
                        group = (CommandGroup)Enum.Parse(typeof(CommandGroup), args[0]);
                    }
                }
                IEnumerable<ICommand> coms = commands.Where(p => (p.Name == args[group == 0 ? 0 : 1] && p.Group == group));
                if (coms.Count() == 1)
                {
                    command = coms.First();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check is socket can use specified command
        /// </summary>
        public static bool CanAccess(ICommand command, Socket soc = null, bool server = false)
        {
            if (server)
            {
                return command.IsServer;
            }
            else
            {
                if (command.IsClient)
                {
                    if(!Utilities.IsConnect(soc))
                    {
                        return command.IsNoConnect;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
