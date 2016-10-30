using Galactic_Colors_Control_Common;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Galactic_Colors_Control_Server
{
    class Utilities
    {
        /// <summary>
        /// Check if socket is connect
        /// </summary>
        /// <param name="soc">Client socket</param>
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

        /// <summary>
        /// Return username from socket
        /// </summary>
        /// <param name="soc">Cleint socket</param>
        /// <returns>Name</returns>
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

        /// <summary>
        /// Write line in console with correct colors
        /// </summary>
        /// <param name="v">Text to write</param>
        public static void ConsoleWrite(string v)
        {
            Console.Write("\b");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(v);
            ConsoleResetColor();
            Console.Write(">");
        }

        /// <summary>
        /// Reset Console Colors
        /// For non black background console as Ubuntu
        /// </summary>
        public static void ConsoleResetColor()
        {
            Console.ResetColor();
            Console.BackgroundColor = ConsoleColor.Black;
        }

        /// <summary>
        /// Send data to specified socket
        /// </summary>
        /// <param name="soc">Target socket</param>
        /// <param name="data">Data to send</param>
        /// <param name="dtype">Type of data</param>
        public static void Send(Socket soc, object data, Common.dataType dtype)
        {
            /*
            Format:
            0-3: dataType
            4-x: data
            */
            byte[] type = new byte[4];
            type = BitConverter.GetBytes((int)dtype);
            byte[] bytes = null;
            switch (dtype)
            {
                case Common.dataType.message:
                    bytes = Encoding.ASCII.GetBytes((string)data);
                    break;

                case Common.dataType.data:
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

        /// <summary>
        /// Send data to all clients
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <param name="dtype">Type of data</param>
        public static void Broadcast(object data, Common.dataType dtype)
        {
            foreach (Socket soc in Program.clients.Keys)
            {
                Send(soc, data, dtype);
            }
        }

        /// <summary>
        /// Send or display if server
        /// </summary>
        /// <param name="message">Text to send</param>
        /// <param name="soc">Target socket</param>
        /// <param name="server">Is server?</param>
        public static void Return(string message, Socket soc = null, bool server = false)
        {
            if (server)
            {
                ConsoleWrite(message);
            }
            else
            {
                Send(soc, message, Common.dataType.message);
            }
        }
    }
}
