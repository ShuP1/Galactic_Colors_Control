using System;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Galactic_Colors_Control_Server
{
    class Utilities
    {
        public enum dataType { message, data };

        public static bool IsConnect(Socket soc)
        {
            if(soc == null)
            {
                return true;
            }
            else
            {
                if (Program.clients.ContainsKey(soc))
                {
                    return Program.clients[soc].status != -1;
                }
                else
                {
                    Logger.Write("IsConnect : Unknown client", Logger.logType.error);
                    return true;
                }
            }
        }

        public static string GetName(Socket soc)
        {
            if (soc != null)
            {
                if (Program.clients.ContainsKey(soc))
                {
                    string res = Program.clients[soc].pseudo;
                    if (res == "") { res = ((IPEndPoint)soc.LocalEndPoint).Address.ToString(); }
                    return res;
                }
                else
                {
                    return "?";
                }
            }
            else
            {
                return "Server";
            }
        }

        public static void ConsoleWrite(string v)
        {
            Console.Write("\b");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(v);
            ConsoleResetColor();
            Console.Write(">");
        }

        public static void ConsoleResetColor()
        {
            Console.ResetColor();
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public static void Send(Socket soc, object data, dataType dtype)
        {
            byte[] type = new byte[4];
            type = BitConverter.GetBytes((int)dtype);
            byte[] bytes = null;
            switch (dtype)
            {
                case dataType.message:
                    bytes = Encoding.ASCII.GetBytes((string)data);
                    break;

                case dataType.data:
                    BinaryFormatter bf = new BinaryFormatter();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bf.Serialize(ms, data);
                        bytes = ms.ToArray();
                    }
                    break;

                default:
                    bytes = new byte[] { 1 };
                    break;
            }
            byte[] final = new byte[type.Length + bytes.Length];
            type.CopyTo(final, 0);
            bytes.CopyTo(final, type.Length);
            soc.Send(final);
        }

        public static void Broadcast(object data, dataType dtype)
        {
            foreach (Socket soc in Program.clients.Keys)
            {
                Send(soc, data, dtype);
            }
        }

        public static void Return(string message, Socket soc = null, bool server = false)
        {
            if (server)
            {
                ConsoleWrite(message);
            }
            else
            {
                Send(soc, message, dataType.message);
            }
        }
    }
}
