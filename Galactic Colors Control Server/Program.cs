using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace Galactic_Colors_Control_Server
{
    internal class Program
    {
        private const int BUFFER_SIZE = 2048;
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public static bool _debug = false;
        public static bool _dev = false;

        public static bool _run = true;
        public static bool _open = true;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];

        public static Dictionary<Socket, Client> clients { get; private set; } = new Dictionary<Socket, Client>();

        private static int partyID = 0;

        public static Dictionary<int, Party> parties { get; private set; } = new Dictionary<int, Party>();
        public static int selectedParty = -1;

        public static Config config = new Config();
        public static Thread CheckConnected = new Thread(CheckConnectedLoop);

        /// <summary>
        /// Server Main thread
        /// </summary>
        private static void Main(string[] args)
        {
            Console.Title = "Galactic Colors Control Server";
            Logger.Write("Galactic Colors Control Server " + Assembly.GetEntryAssembly().GetName().Version.ToString(), Logger.logType.fatal);
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "--debug":
                        _debug = true;
                        Logger.Write("SERVER IS IN DEBUG MODE !", Logger.logType.error, Logger.logConsole.show);
                        break;

                    case "--dev":
                        _dev = true;
                        Logger.Write("SERVER IS IN DEV MODE !", Logger.logType.error, Logger.logConsole.show);
                        break;

                    default:
                        Common.ConsoleWrite("Use --debug or --dev");
                        break;
                }
            }
            if (Type.GetType("Mono.Runtime") != null) { Logger.Write("Using Mono", Logger.logType.warm); }
            Console.Write(">");
            SetupServer();
            ConsoleLoop();
            CloseAllSockets();
        }

        /// <summary>
        /// Initialise server and start threads.
        /// </summary>
        private static void SetupServer()
        {
            config = config.Load();
            Logger.Initialise();
            Commands.Manager.Load();
            Logger.Write("Setting up server on *:" + config.port, Logger.logType.warm);
            Logger.Write("Size:" + config.size, Logger.logType.debug);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, config.port));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
            CheckConnected.Start();
            Logger.Write("Server setup complete", Logger.logType.info);
        }

        /// <summary>
        /// Wait console commands and execute them.
        /// </summary>
        private static void ConsoleLoop()
        {
            while (_run)
            {
                string ConsoleInput = Console.ReadLine();
                Console.Write(">");
                string[] args = Common.SplitArgs(ConsoleInput);
                Common.ConsoleWrite(new ResultData(-1, Commands.Manager.Execute(args, null, true)).ToSmallString());
                ConsoleInput = null;
            }
        }

        /// <summary>
        /// Close all connected client.
        /// </summary>
        private static void CloseAllSockets()
        {
            Logger.Write("Stoping server", Logger.logType.warm, Logger.logConsole.show);
            Utilities.Broadcast(new EventData(EventTypes.ServerKick, Common.Strings("Close")));
            config.Save();
            foreach (Socket socket in clients.Keys)
            {
                socket.Shutdown(SocketShutdown.Both);
                Logger.Write("Shutdown " + Utilities.GetName(socket), Logger.logType.debug);
            }
            serverSocket.Close();
            CheckConnected.Join(2000);
            Logger.Write("Server stoped", Logger.logType.info);
            Logger._run = false;
            Logger.Updater.Join();
        }

        /// <summary>
        /// Wait a client and check if is correct
        /// </summary>
        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;
            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            if (_open)
            {
                if (clients.Count < config.size)
                {
                    AddClient(socket);
                }
                else
                {
                    Logger.Write("Client can't join from " + ((IPEndPoint)socket.LocalEndPoint).Address.ToString() + " no more space", Logger.logType.warm);
                    Utilities.Send(socket, new EventData(EventTypes.ServerKick, Common.Strings("Space")));
                    socket.Close();
                }
            }
            else
            {
                Logger.Write("Client can't join from " + ((IPEndPoint)socket.LocalEndPoint).Address.ToString() + " server closed", Logger.logType.info);
                Utilities.Send(socket, new EventData(EventTypes.ServerKick, Common.Strings("Close")));
                socket.Close();
            }
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        /// <summary>
        /// Add client and initialise receive
        /// </summary>
        private static void AddClient(Socket socket)
        {
            Client client = new Client();
            clients.Add(socket, client);
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            Logger.Write("Client connection from " + Utilities.GetName(socket), Logger.logType.info);
            Logger.Write("Size: " + clients.Count + "/" + config.size, Logger.logType.dev);
            if (clients.Count >= config.size)
            {
                Logger.Write("Server full", Logger.logType.warm, Logger.logConsole.show);
            }
        }

        /// <summary>
        /// Wait a client commands and execute them
        /// </summary>
        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Logger.Write("Client forcefully disconnected from " + Utilities.GetName(current) + " : SocketException", Logger.logType.info);
                string username = Utilities.GetName(current);
                bool connected = clients[current].status != -1;
                Logger.Write("Size: " + clients.Count + "/" + config.size, Logger.logType.debug);
                current.Close(); // Don't shutdown because the socket may be disposed and its disconnected anyway.
                clients.Remove(current);
                if (connected) { Utilities.Broadcast(new EventData(EventTypes.ServerLeave, Common.Strings(username))); }
                return;
            }

            var data = new byte[received];
            Array.Copy(buffer, data, received);

            Data packet = Data.FromBytes(ref data);
            if (packet != null)
            {
                switch (packet.GetType().Name)
                {
                    case "RequestData":
                        RequestData req = (RequestData)packet;
                        Utilities.Send(current, new ResultData(req.id, Commands.Manager.Execute(req.args, current)));
                        break;

                    default:
                        Logger.Write("Wrong packet from " + Utilities.GetName(current), Logger.logType.error);
                        break;
                }
            }
            else
            {
                Logger.Write("Wrong packet from " + Utilities.GetName(current), Logger.logType.error);
            }

            if (clients.ContainsKey(current)) { current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current); }
        }

        private static void CheckConnectedLoop()
        {
            while (_run)
            {
                foreach (Socket current in clients.Keys.ToArray())
                {
                    if ((current.Poll(10, SelectMode.SelectRead) && current.Available == 0) || !current.Connected)
                    {
                        string username = Utilities.GetName(current);
                        Logger.Write("Client forcefully disconnected from " + username + " : NotConnected", Logger.logType.info);
                        bool connected = clients[current].status != -1;
                        Logger.Write("Size: " + clients.Count + "/" + config.size, Logger.logType.debug);
                        current.Close(); // Don't shutdown because the socket may be disposed and its disconnected anyway.
                        clients.Remove(current);
                        if (connected) { Utilities.Broadcast(new EventData(EventTypes.ServerLeave, Common.Strings(username))); }
                    }
                    Thread.Sleep(200);
                }
            }
        }

        /// <summary>
        /// Add new party with index
        /// </summary>
        /// <param name="party">Party to add</param>
        public static void AddParty(Party party)
        {
            parties.Add(GetPartyID(), party);
        }

        public static int GetPartyID(bool indent = true)
        {
            if (indent) { partyID++; }
            return partyID;
        }
    }
}