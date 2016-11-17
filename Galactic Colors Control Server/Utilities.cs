using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System;
using System.Net;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server
{
    internal class Utilities
    {
        /// <summary>
        /// Check if socket is connect
        /// </summary>
        /// <param name="soc">Client socket</param>
        public static bool IsConnect(Socket soc)
        {
            if (soc == null)
                return true;

            if (Program.clients.ContainsKey(soc))
                return Program.clients[soc].status != -1;

            Program.logger.Write("IsConnect : Unknown client", Logger.logType.error);
            return false;
        }

        /// <summary>
        /// Return username from socket
        /// </summary>
        /// <param name="soc">Cleint socket</param>
        /// <returns>Name</returns>
        public static string GetName(Socket soc)
        {
            if (soc == null)
                return Program.multilang.Get("Server",Program.config.lang);

            if (!Program.clients.ContainsKey(soc))
                return "?";

            string res = Program.clients[soc].pseudo;
            if (res == "") { res = ((IPEndPoint)soc.LocalEndPoint).Address.ToString(); }
            return res;
        }

        public static int GetParty(Socket soc)
        {
            if (soc == null)
                return Program.selectedParty;

            return Program.clients[soc].partyID;
        }

        /// <summary>
        /// Send data to specified socket
        /// </summary>
        /// <param name="soc">Target socket</param>
        /// <param name="data">Data to send</param>
        public static void Send(Socket soc, Data packet)
        {
            if (soc.Connected)
            {
                try
                {
                    soc.Send(packet.ToBytes());
                    if (Program.config.logLevel == Logger.logType.dev)
                    {
                        Program.logger.Write("Send to " + GetName(soc) + " : " + packet.ToLongString(), Logger.logType.dev);
                    }
                }
                catch (Exception e)
                {
                    Program.logger.Write("Send exception to " + GetName(soc) + " : " + e.Message, Logger.logType.error);
                }
            }
        }

        /// <summary>
        /// Send data to all clients
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <param name="dtype">Type of data</param>
        /// <param name="message">Message to display for server</param>
        public static void Broadcast(Data packet)
        {
            foreach (Socket soc in Program.clients.Keys)
            {
                Send(soc, packet);
            }
            switch (packet.GetType().Name)
            {
                case "EventData":
                    Common.ConsoleWrite(Program.multilang.GetEventText((EventData)packet, Program.config.lang));
                    break;

                default:
                    Common.ConsoleWrite(packet.ToSmallString());
                    break;
            }
        }

        /// <summary>
        /// Send data to all client of the party
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <param name="dtype">Type of data</param>
        /// <param name="party">Id of the party</param>
        /// <param name="message">Message to display for server</param>
        public static void BroadcastParty(Data data, int party)
        {
            foreach (Socket soc in Program.clients.Keys)
            {
                if (Program.clients[soc].partyID == party)
                {
                    Send(soc, data);
                }
            }
            if (Program.selectedParty == party)
            {
                switch (data.GetType().Name)
                {
                    case "EventData":
                        Common.ConsoleWrite(Program.multilang.GetEventText((EventData)data, Program.config.lang));
                        break;

                    default:
                        Common.ConsoleWrite(data.ToSmallString());
                        break;
                }
            }
        }

        /// <summary>
        /// Send or display if server
        /// </summary>
        /// <param name="message">Text to display if server</param>
        /// <param name="data">Data to send if client</param>
        /// <param name="soc">Target socket</param>
        /// <param name="server">Is server?</param>
        public static void Return(Data data, Socket soc = null, bool server = false)
        {
            if (server)
            {
                Common.ConsoleWrite(data.ToSmallString());
            }
            else
            {
                Send(soc, data);
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
        public static bool AccessParty(ref int partyId, string[] args, bool needOwn, Socket soc = null, bool server = false)
        {
            if (server)
            {
                if (Program.selectedParty == -1)
                    return false;

                if (Program.parties.ContainsKey(Program.selectedParty))
                {
                    partyId = Program.selectedParty;
                    return true;
                }
                else
                {
                    Program.selectedParty = -1;
                    return false;
                }
            }
            else
            {
                if (Program.clients[soc].partyID == -1)
                    return false;

                if (!Program.parties.ContainsKey(Program.clients[soc].partyID))
                    return false;

                if (Program.parties[Program.clients[soc].partyID].IsOwner(GetName(soc)) || !needOwn)
                {
                    partyId = Program.clients[soc].partyID;
                    return true;
                }
                else { return false; }
            }
        }
    }
}