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
        /// <param name="Fore">Foreground color</param>
        /// <param name="Back">Background color</param>
        public static void ConsoleWrite(string v, ConsoleColor Fore = ConsoleColor.White, ConsoleColor Back = ConsoleColor.Black)
        {
            Console.Write("\b");
            Console.ForegroundColor = Fore;
            Console.BackgroundColor = Back;
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
            if (dtype == Common.dataType.message) { ConsoleWrite((string)data); }
        }

        /// <summary>
        /// Send data to all client of the party
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <param name="dtype">Type of data</param>
        /// <param name="party">Id of the party</param>
        public static void BroadcastParty(object data, Common.dataType dtype, int party)
        {
            foreach(Socket soc in Program.clients.Keys)
            {
                if (Program.clients[soc].partyID == party)
                {
                    Send(soc, data, dtype);
                }
            }
            if (dtype == Common.dataType.message) { if (Program.selectedParty == party) { ConsoleWrite((string)data); } }
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

        /// <summary>
        /// Try get party of the socket
        /// </summary>
        /// <param name="partyId">Result party ID</param>
        /// <param name="needOwn">Return true only for owner and server</param>
        /// <param name="soc">Target socket</param>
        /// <param name="server">Is server?</param>
        /// <returns>Can access?</returns>
        public static bool AccessParty(ref int partyId, bool needOwn,  Socket soc = null, bool server = false)
        {
            if (server)
            {
                if(Program.selectedParty != -1)
                {
                    if (Program.parties.ContainsKey(Program.selectedParty))
                    {
                        partyId = Program.selectedParty;
                        return true;
                    }
                    else
                    {
                        Logger.Write("Can't find party " + Program.selectedParty, Logger.logType.error);
                        Program.selectedParty = -1;
                        return false;
                    }
                }
                else
                {
                    ConsoleWrite("Join a party before");
                    return false;
                }
            }
            else
            {
                if(Program.clients[soc].partyID != -1)
                {
                    if (Program.parties.ContainsKey(Program.clients[soc].partyID))
                    {
                        if (Program.parties[Program.clients[soc].partyID].IsOwner(GetName(soc)) || !needOwn)
                        {
                            partyId = Program.clients[soc].partyID;
                            return true;
                        }
                        else
                        {
                            Send(soc, "You are not owner", Common.dataType.message);
                            return false;
                        }
                    }
                    else
                    {
                        Send(soc, "Can't find party " + Program.clients[soc].partyID, Common.dataType.message);
                        Program.clients[soc].partyID = -1;
                        return false;
                    }
                }
                else
                {
                    Send(soc, "Join a party before", Common.dataType.message);
                    return false;
                }
            }
        }
    }
}
