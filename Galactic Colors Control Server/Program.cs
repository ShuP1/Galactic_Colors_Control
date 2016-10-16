using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace Galactic_Colors_Control_Server
{
	internal class Program
	{
		private const int BUFFER_SIZE = 2048;
		private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		public static bool _debug = false;
		public static bool _run = true;
		public static bool _open = true;
		private static readonly byte[] buffer = new byte[BUFFER_SIZE];
		public static Dictionary<Socket, Data> clients { get; private set; } = new Dictionary<Socket, Data>();
		public static Config config = new Config();

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
						Logger.Write("SERVER IS IN DEBUG MODE !", Logger.logType.error);
						break;

					default:
						Utilities.ConsoleWrite("Use --debug or any argument");
						break;
				}
			}
			if(Type.GetType("Mono.Runtime") != null) { Logger.Write("Using Mono", Logger.logType.warm); }
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
				ExecuteMessage(ConsoleInput, null, true);
				ConsoleInput = null;
			}
		}

		/// <summary>
		/// Close all connected client.
		/// </summary>
		private static void CloseAllSockets()
		{
			Logger.Write("Stoping server", Logger.logType.warm);
			config.Save();
			foreach (Socket socket in clients.Keys)
			{
				socket.Shutdown(SocketShutdown.Both);
				Logger.Write("Shutdown " + Utilities.GetName(socket),Logger.logType.debug);
			}
			serverSocket.Close();
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
			catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
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
					Utilities.Send(socket, "/kick can't_join_no_more_space", Utilities.dataType.message);
					socket.Close();
				}
			}
			else
			{
				Logger.Write("Client can't join from " + ((IPEndPoint)socket.LocalEndPoint).Address.ToString() + " server closed", Logger.logType.info);
				Utilities.Send(socket, "/kick can't_join_server_closed", Utilities.dataType.message);
				socket.Close();
			}
			serverSocket.BeginAccept(AcceptCallback, null);
		}

		/// <summary>
		/// Add client and initialise receive
		/// </summary>
		private static void AddClient(Socket socket)
		{
			clients.Add(socket, new Data());
			socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
			Logger.Write("Client connection from " + Utilities.GetName(socket), Logger.logType.info);
			Logger.Write("Size: " + clients.Count + "/" + config.size, Logger.logType.debug);
			if (clients.Count >= config.size)
			{
				Logger.Write("Server full", Logger.logType.warm);
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
				Logger.Write("Client forcefully disconnected from " + Utilities.GetName(current), Logger.logType.info);
				Logger.Write("Size: " + clients.Count + "/" + config.size, Logger.logType.debug);
				current.Close(); // Don't shutdown because the socket may be disposed and its disconnected anyway.
				clients.Remove(current);
				return;
			}

			var data = new byte[received];
			Array.Copy(buffer, data, received);

			try {
				byte[] type = new byte[4];
				type = data.Take(4).ToArray();
				type.Reverse();
				Utilities.dataType dtype = (Utilities.dataType)BitConverter.ToInt32(type, 0);
				byte[] bytes = null;
				bytes = data.Skip(4).ToArray();
				switch (dtype)
				{
					case Utilities.dataType.message:
						string text = Encoding.ASCII.GetString(bytes);
						ExecuteMessage(text, current);
						break;

					case Utilities.dataType.data:
						Console.WriteLine("data");
						break;

					default:
						Logger.Write("Unknow type data form" + Utilities.GetName(current), Logger.logType.error);
						break;
				}

				if (clients.ContainsKey(current)) { current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current); }
			}
			catch (Exception) { }
		}

		/// <summary>
		/// Execute send command
		/// <param name="text">Command</param>
		/// <param name="soc">Sender socket</param>
		/// <param name="server">Is sender server?</param>
		/// </summary>
		private static void ExecuteMessage(string text, Socket soc = null, bool server = false)
		{
			if (text.Length > 0)
			{
				if (text[0] == '/')
				{
					text = text.Substring(1);
					text = text.ToLower();
					string[] array = text.Split(new char[1] { ' ' }, 4, StringSplitOptions.RemoveEmptyEntries);
					if (array.Length > 0)
					{
						Commands.Manager.Execute(array, soc, server);
					}
					else
					{
						Utilities.Return("Any command",soc , server);
					}
				}
				else
				{
					if (!Utilities.IsConnect(soc))
					{
						Utilities.Send(soc, "Only identified clients can talk.", Utilities.dataType.message);
					}
					else
					{
						Logger.Write(Utilities.GetName(soc) + " : " + text, Logger.logType.info);
						Utilities.Broadcast(Utilities.GetName(soc) + " : " + text, Utilities.dataType.message);
					}
				}
			}
		}
	}
}