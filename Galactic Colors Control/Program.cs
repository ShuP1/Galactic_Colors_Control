using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace Galactic_Colors_Control
{
    public class Client
    {
        private Socket ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public int PORT = 0;
        private int _errorCount = 0;
        private bool _run = true;
        public string IP = null;

        public bool isRunning { get { return _run; } }

        private enum dataType { message, data };

        public List<string> Output = new List<string>();

        private Thread RecieveThread;

        public void ResetHost()
        {
            IP = null;
            PORT = 0;
        }

        public string ValidateHost(string text)
        {
            if (text == null) { text = ""; }
            string[] parts = text.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
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
                    IP = host.ToString();
                    return IP + ":" + PORT;
                }
                catch (Exception e)
                {
                    Output.Add(e.Message);
                    PORT = 0;
                    return null;
                }
            }
            else
            {
                Output.Add("Incorrect port");
                return null;
            }
        }

        /// <summary>
        /// Set IP and PORT before
        /// </summary>
        /// <returns>Connection succes</returns>
        public bool ConnectHost()
        {
            int attempts = 0;

            while (!ClientSocket.Connected && attempts < 5)
            {
                try
                {
                    attempts++;
                    Output.Add("Connection attempt " + attempts);
                    ClientSocket.Connect(IP, PORT);
                }
                catch (SocketException)
                {
                    Output.Clear();
                }
            }
            if (attempts < 5)
            {
                Output.Clear();
                Output.Add("Connected to " + IP.ToString());
                _run = true;
                RecieveThread = new Thread(ReceiveLoop);
                RecieveThread.Start();
                return true;
            }
            else
            {
                Output.Clear();
                Output.Add("Can't connected to " + IP.ToString());
                ResetSocket();
                return false;
            }
        }

        private void ResetSocket()
        {
            ClientSocket.Close();
            ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Close socket and exit program.
        /// </summary>
        public void ExitHost()
        {
            Send("/exit", dataType.message); // Tell the server we are exiting
            _run = false;
            RecieveThread.Join();
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            Output.Add("Bye");
            ResetHost();
        }

        public void SendRequest(string request)
        {
            switch (request.ToLower())
            {
                case "/exit":
                    ExitHost();
                    break;

                case "/ping":
                    PingHost();
                    break;

                default:
                    Send(request, dataType.message);
                    break;
            }
        }

        private void PingHost()
        {
            Ping p = new Ping();
            PingReply r;

            r = p.Send(IP);

            if (r.Status == IPStatus.Success)
            {
                Output.Add(r.RoundtripTime.ToString() + " ms.");
            }
            else
            {
                Output.Add("Time out");
            }
        }

        private void Send(object data, dataType dtype)
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
                Output.Add("Can't contact server : " + _errorCount);
                _errorCount++;
            }
            if (_errorCount >= 5)
            {
                Output.Add("Kick : too_much_errors");
                _run = false;
            }
        }

        private void ReceiveLoop()
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
                    Output.Add("Server timeout");
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
                                case "connected":
                                    Output.Add("Identifiaction succes");
                                    break;

                                case "allreadytaken":
                                    Output.Add("Username Allready Taken");
                                    break;

                                case "kick":
                                    if (array.Length > 1)
                                    {
                                        Output.Add("Kick : " + array[1]);
                                    }
                                    else
                                    {
                                        Output.Add("Kick by server");
                                    }
                                    _run = false;
                                    break;

                                default:
                                    Output.Add("Unknown action from server");
                                    break;
                            }
                        }
                        else
                        {
                            Output.Add(text);
                        }
                        break;

                    case dataType.data:
                        Console.WriteLine("data");
                        break;
                }
                Thread.Sleep(200);
            }
            Output.Add("/*exit*/");
        }
    }
}