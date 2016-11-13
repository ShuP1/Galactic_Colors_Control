using Galactic_Colors_Control_Common;
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
    /// <summary>
    /// Client CrossPlatform Core
    /// </summary>
    public class Client
    {
        private Socket ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public string IP = null; //Server IP
        public int PORT = 0; //Server Port

        public struct CoreConfig
        {
            public int resultsBuffer; //Max amount of waiting results
            public int timeout; //Request timeout in ms
            public int refresh; //Threads sleep in ms

            public CoreConfig(int buffer, int time, int speed)
            {
                resultsBuffer = buffer;
                timeout = time;
                refresh = speed;
            }
        }

        public CoreConfig config = new CoreConfig(20, 2000, 200); //Set default config

        private int _errorCount = 0; //Leave if > 5

        private bool _run = true; //Thread Stop
        public bool isRunning { get { return _run; } }

        private int RequestId = 0;
        private List<ResultData> Results = new List<ResultData>();
        private Thread RecieveThread; //Main Thread
        public EventHandler OnEvent; //Execute on EventData reception (must be short or async)

        /// <summary>
        /// Soft Server Reset
        /// </summary>
        public void ResetHost()
        {
            IP = null;
            PORT = 0;
        }

        /// <summary>
        /// Test and Convert Hostname to Address
        /// </summary>
        /// <param name="text">Hostname</param>
        /// <returns>Address(IP:PORT) or Error(*'text')</returns>
        public string ValidateHost(string text)
        {
            if (text == null) { text = ""; } //Prevent NullException
            string[] parts = text.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries); //Split IP and Port
            if (parts.Length == 0) //Default config (localhost)
            {
                parts = new string[] { "" };
                PORT = 25001;
            }
            else
            {
                if (parts.Length > 1)
                {
                    if (!int.TryParse(parts[1], out PORT)) { PORT = 0; } //Check Port
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
                    IPHostEntry ipHostEntry = Dns.GetHostEntry(parts[0]);//Resolve Hostname
                    IPAddress host = ipHostEntry.AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork);//Get IPv4
                    IP = host.ToString();
                    return IP + ":" + PORT;
                }
                catch (Exception e)
                {
                    PORT = 0;
                    return "*" + e.Message;
                }
            }
            else
            {
                return "*Port Format";
            }
        }

        /// <summary>
        /// Start Server connection
        /// </summary>
        /// <remarks>Setup IP and Port before</remarks>
        /// <returns>connection success</returns>
        public bool ConnectHost()
        {
            int attempts = 0;

            while (!ClientSocket.Connected && attempts < 5)
            {
                try
                {
                    attempts++;
                    ClientSocket.Connect(IP, PORT);
                }
                catch (SocketException) { }
            }
            if (attempts < 5) //Connection success
            {
                _run = true;
                RecieveThread = new Thread(ReceiveLoop); //Starting Main Thread
                RecieveThread.Start();
                return true;
            }
            else
            {
                ResetSocket();
                return false;
            }
        }

        /// <summary>
        /// Hard Reset (unsafe)
        /// </summary>
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
            _run = false; //Stopping Thread
            RecieveThread.Join(2000);
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            ResetHost();
        }

        /// <summary>
        /// Send RequestData to server
        /// </summary>
        /// <param name="args">Request args</param>
        /// <returns>ResultData or Timeout</returns>
        public ResultData Request(string[] args)
        {
            // TODO filter Client Side Requests
            RequestData req = new RequestData(GetRequestId(), args);
            if (!Send(req))
                return new ResultData(req.id, ResultTypes.Error, Common.Strings("Send Exception"));

            DateTime timeoutDate = DateTime.Now.AddMilliseconds(config.timeout); //Create timeout DataTime
            while (timeoutDate > DateTime.Now)
            {
                foreach (ResultData res in Results.ToArray()) //Check all results
                {
                    if (res.id == req.id)
                    {
                        Results.Remove(res);
                        return res;
                    }
                }
                Thread.Sleep(config.refresh);
            }
            return new ResultData(req.id, ResultTypes.Error, Common.Strings("Timeout"));
        }

        /// <summary>
        /// Ping Current Server IP
        /// </summary>
        /// <returns>Time in ms or 'Timeout'</returns>
        private string PingHost()
        {
            Ping ping = new Ping();
            PingReply reply;

            reply = ping.Send(IP);
            if (reply.Status == IPStatus.Success)
                return reply.RoundtripTime.ToString();

            return "Timeout";
        }

        /// <summary>
        /// Send Data object to server
        /// </summary>
        /// <returns>Send success</returns>
        private bool Send(Data packet)
        {
            try
            {
                ClientSocket.Send(packet.ToBytes());
                return true;
            }
            catch //Can't contact server
            {
                _errorCount++;
            }
            return false;
        }

        /// <summary>
        /// Main Thread
        /// </summary>
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
                    _errorCount++;
                }
                if (_errorCount >= 5)
                {
                    _run = false;
                }
                if (received == 0) return;
                _errorCount = 0;
                var data = new byte[received];
                Array.Copy(buffer, data, received);

                Data packet = Data.FromBytes(ref data); //Create Data object from recieve bytes
                if (packet != null)
                {
                    switch (packet.GetType().Name)
                    {
                        case "EventData":
                            EventData eve = (EventData)packet;
                            if (OnEvent != null)
                                OnEvent.Invoke(this, new EventDataArgs(eve));
                            break;

                        case "ResultData":
                            ResultData res = (ResultData)packet;
                            ResultAdd(res);
                            break;

                        default: //Ignore others Packets
                            break;
                    }
                }
                Thread.Sleep(config.refresh);
            }
            //TODOOutput.Add("/*exit*/");
        }

        public int GetRequestId(bool indent = true)
        {
            if (indent) { RequestId++; }
            return RequestId;
        }

        /// <summary>
        /// Add to Results
        /// </summary>
        public void ResultAdd(ResultData res)
        {
            while (Results.Count + 1 > config.resultsBuffer) { Results.RemoveAt(0); } //Removes firsts
            Results.Add(res);
        }
    }
}