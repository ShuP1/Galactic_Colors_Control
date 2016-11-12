using Galactic_Colors_Control_Common.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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

        public List<string> Output = new List<string>();
        private int RequestId = 0;

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
                    PORT = 0;
                    Output.Add(e.Message);
                    return null;
                }
            }
            else
            {
                Output.Add("Incorrect Port");
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
            Send(new RequestData(GetRequestId(), new string[1] { "exit" }));// Tell the server we are exiting
            _run = false;
            RecieveThread.Join(2000);
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

                case "/clear":
                    Output.Add("/clear");
                    break;

                default:
                    //TODO add key and send error here
                    if (request.Length > 0)
                    {
                        if (request[0] == '/')
                        {
                            request = request.Substring(1);
                            string[] array = request.Split(new char[1] { ' ' }, 4, StringSplitOptions.RemoveEmptyEntries);
                            if (array.Length > 0)
                            {
                                Send(new RequestData(GetRequestId(), array));
                            }
                            else
                            {
                                Output.Add("Any Command");
                            }
                        }
                        else
                        {
                            Send(new RequestData(GetRequestId(), new string[2] { "say", request }));
                        }
                    }
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

        private void Send(Data packet)
        {
            try
            {
                ClientSocket.Send(packet.ToBytes());
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

                Data packet = Data.FromBytes(ref data);
                if (packet != null)
                {
                    switch (packet.GetType().Name)
                    {
                        case "EventData":
                            EventData eve = (EventData)packet;
                            Output.Add(eve.ToSmallString());
                            break;

                        case "ResultData":
                            ResultData res = (ResultData)packet;
                            Output.Add(res.ToSmallString());
                            break;

                        default:
                            Output.Add("Wrong packet");
                            break;
                    }
                }
                else
                {
                    Output.Add("Wrong packet");
                }
                Thread.Sleep(200);
            }
            Output.Add("/*exit*/");
        }

        public int GetRequestId(bool indent = true)
        {
            if (indent) { RequestId++; }
            return RequestId;
        }
    }
}