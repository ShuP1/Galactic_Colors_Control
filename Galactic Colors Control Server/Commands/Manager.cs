﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;

namespace Galactic_Colors_Control_Server.Commands
{
    class Manager
    {
        public static Dictionary<string, ICommand> commands { get; private set; } = new Dictionary<string, ICommand>();

        public static void Load()
        {
            IEnumerable<ICommand> coms = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetInterfaces().Contains(typeof(ICommand)) && x.GetConstructor(Type.EmptyTypes) != null).Select(x => Activator.CreateInstance(x) as ICommand);
            foreach (ICommand com in coms)
            {
                commands.Add(com.Name, com);
                Logger.Write("Added command " + com.GetType().Name, Logger.logType.debug);
            }
        }

        public static void Execute(string[] args, Socket soc = null, bool server = false)
        {
            if (commands.ContainsKey(args[0]))
            {
                ICommand command = commands[args[0]];
                if (CanAccess(command,soc,server))
                {
                    if(args.Length > command.minArgs)
                    {
                        if(args.Length - 1 <= command.maxArgs)
                        {
                            command.Execute(args, soc, server);
                        }
                        else
                        {
                            Utilities.Return("Command " + command.Name + " require at most " + command.minArgs + " argument(s).", soc, server);
                        }
                    }
                    else
                    {
                        Utilities.Return("Command " + command.Name + " require at least " + command.minArgs + " argument(s).", soc, server);
                    }
                }
                else
                {
                    Utilities.Return("Unknown command : " + args[0], soc, server);
                }
            }
            else
            {
                Utilities.Return("Unknown command : " + args[0], soc, server);
            }
        }

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