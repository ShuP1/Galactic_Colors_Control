using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace Galactic_Colors_Control_Client
{
    internal class Program
    {
        private static readonly Socket ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static int PORT = 0;
        private static int _errorCount = 0;
        private static bool _run = true;
        private static string IP = null;

        private enum dataType { message, data };

        private static void Main()
        {
            Console.Title = "Galactic Colors Control Client";
            Console.Write(">");
            ConnectToServer();
            RequestLoop();
            Exit();
        }

        private static void ConnectToServer()
        {
            int attempts = 0;

            while (IP == null)
            {
                ConsoleWrite(Environment.NewLine + "Enter server host:");
                string text = Console.ReadLine();
                string[] parts = text.Split(new char[] {':'}, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                {
                    parts = new string[] { "" };
                    PORT = 25001;
                }
                else
                {
                    if (parts.Length > 1)
                    {
                        if (!int.TryParse(parts[1], out PORT)) { PORT = 0; }
                        if (PORT < 0 || PORT > 65535) { PORT = 0; }
                    }
                    else
                    {
                        PORT = 25001;
                    }
                }
                if (PORT != 0)
                {
                    try
                    {
                        IPHostEntry ipHostEntry = Dns.GetHostEntry(parts[0]);
                        IPAddress host = ipHostEntry.AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork);
                        ConsoleWrite("Use " + host.ToString() + ":" + PORT + "? y/n");
                        ConsoleKey key = ConsoleKey.NoName;
                        while (key != ConsoleKey.Y && key != ConsoleKey.N)
                        {
                            key = Console.ReadKey().Key;
                        }
                        if (key == ConsoleKey.Y)
                        {
                            IP = host.ToString();
                        }
                        else
                        {
                            PORT = 0;
                        }
                    }
                    catch (Exception e)
                    {
                        ConsoleWrite(e.Message);
                        PORT = 0;
                    }
                }
                else
                {
                    ConsoleWrite("Incorrect port");
                }
            }

            while (!ClientSocket.Connected && attempts < 5)
            {
                try
                {
                    attempts++;
                    ConsoleWrite("Connection attempt " + attempts);
                    ClientSocket.Connect(IP, PORT);
                }
                catch (SocketException)
                {
                    Console.Clear();
                }
            }
            if (attempts < 5)
            {
                Console.Clear();
                ConsoleWrite("Connected to " + IP.ToString());
            }
            else
            {
                Console.Clear();
                ConsoleWrite("Can't connected to " + IP.ToString());
                ClientSocket.Close();
                ConsoleWrite("Press Enter to quit");
                Console.Read();
                Environment.Exit(0);
            }
        }

        private static void RequestLoop()
        {
            Thread ReceiveThread = new Thread(new ThreadStart(ReceiveLoop));
            ReceiveThread.Start();
            while (_run)
            {
                SendRequest();
            }
            ReceiveThread.Join();
            if (_errorCount >= 5)
            {
                ConsoleWrite("Exit: Too much network errors");
            }
        }

        /// <summary>
        /// Close socket and exit program.
        /// </summary>
        private static void Exit()
        {
            Send("/exit", dataType.message); // Tell the server we are exiting
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            ConsoleWrite("Bye");
            Console.ReadLine();
            Environment.Exit(0);
        }

        private static void SendRequest()
        {
            string request = Console.ReadLine();
            switch (request.ToLower())
            {
                case "/exit":
                    Exit();
                    break;

                case "/ping":
                    Ping();
                    break;

                default:
                    Send(request, dataType.message);
                    break;
            }
        }

        private static void Ping()
        {
            Ping p = new Ping();
            PingReply r;

            r = p.Send(IP);

            if (r.Status == IPStatus.Success)
            {
                Console.WriteLine(r.RoundtripTime.ToString() + " ms.");
            }
            else
            {
                Console.WriteLine("Time out");
            }
        }

        private static void Send(object data, dataType dtype)
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
            }
            byte[] final = new byte[type.Length + bytes.Length];
            type.CopyTo(final, 0);
            bytes.CopyTo(final, type.Length);
            try
            {
                ClientSocket.Send(final);
            }
            catch
            {
                ConsoleWrite("Can't contact server : " + _errorCount);
                _errorCount++;
            }
            if (_errorCount >= 5)
            {
                _run = false;
            }
        }

        private static void ConsoleWrite(string v)
        {
            Console.Write("\b");
            Console.WriteLine(v);
            Console.Write(">");
        }

        private static void ReceiveLoop()
        {
            while (_run)
            {
                var buffer = new byte[2048];
                int received = 0;
                try
                {
                    received = ClientSocket.Receive(buffer, SocketFlags.None);
                }
                catch
                {
                    ConsoleWrite("Server timeout");
                }
                if (received == 0) return;
                _errorCount = 0;
                var data = new byte[received];
                Array.Copy(buffer, data, received);
                byte[] type = new byte[4];
                type = data.Take(4).ToArray();
                type.Reverse();
                dataType dtype = (dataType)BitConverter.ToInt32(type, 0);
                byte[] bytes = null;
                bytes = data.Skip(4).ToArray();
                switch (dtype)
                {
                    case dataType.message:
                        string text = Encoding.ASCII.GetString(bytes);
                        if (text[0] == '/')
                        {
                            text = text.Substring(1);
                            text = text.ToLower();
                            string[] array = text.Split(new char[1] { ' ' }, 4, StringSplitOptions.RemoveEmptyEntries);
                            switch (array[0])
                            {
                                case "kick":
                                    if (array.Length > 1)
                                    {
                                        ConsoleWrite("Kick : " + array[1]);
                                    }
                                    else
                                    {
                                        ConsoleWrite("Kick by server");
                                    }
                                    _run = false;
                                    break;

                                default:
                                    Console.WriteLine("Unknown action from server");
                                    break;
                            }
                        }
                        else
                        {
                            ConsoleWrite(text);
                        }
                        break;

                    case dataType.data:
                        Console.WriteLine("data");
                        break;
                }
                Thread.Sleep(200);
            }
        }
    }
}