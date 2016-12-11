using Galactic_Colors_Control_Common.Protocol;
using MyCommon;
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

        public enum CommandGroup { root, server, party, client }

        private static RequestResult AnyCommand = new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("AnyCommand"));

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
                Server.logger.Write("Added command " + com.Group.ToString() + " " + com.Name, Logger.logType.debug);
            }
        }

        /// <summary>
        /// Execute sended command
        /// </summary>
        /// <param name="args">command with args</param>
        /// <param name="soc">Sender socket</param>
        /// <param name="server">Is server?</param>
        public static RequestResult Execute(string[] args, Socket soc = null, bool server = false)
        {
            ICommand command = null;
            if (!TryGetCommand(args, ref command))
                return AnyCommand;

            if (!CanAccess(command, soc, server))
                return AnyCommand;

            if (!server && command.IsClientSide)
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("ClientSide"));

            if (args.Length - (command.Group == 0 ? 0 : 1) <= command.minArgs)
                return new RequestResult(ResultTypes.Error, new string[2] { "TooShort", command.minArgs.ToString() });

            if (args.Length - (command.Group == 0 ? 1 : 2) > command.maxArgs)
                return new RequestResult(ResultTypes.Error, new string[2] { "TooLong", command.maxArgs.ToString() });

            try
            {
                return command.Execute(args, soc, server);
            }
            catch (Exception e)
            {
                Server.logger.Write("Command " + args[0] + " Exception : " + e.Message, Logger.logType.error);
                return new RequestResult(ResultTypes.Error, Strings.ArrayFromStrings("ExecuteException"));
            }
        }

        public static string CommandToString(ICommand command)
        {
            string text = "";
            if (command.Group != 0) { text += (command.Group.ToString() + " "); }
            text += command.Name;
            return text;
        }

        /// <summary>
        /// Convert command args in readable string
        /// </summary>
        /// <param name="args">Command args</param>
        public static string CommandToString(string[] args)
        {
            if (args.Length > 0)
            {
                string text = "";
                foreach (string arg in args)
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

        /// <summary>
        /// Try to get a command
        /// </summary>
        /// <param name="args">command args</param>
        /// <param name="command">Command result</param>
        /// <returns>Correct command</returns>
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
                    if (!Utilities.IsConnect(soc))
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