using System;
using Galactic_Colors_Control;
using System.Threading;
using System.Reflection;

namespace Galactic_Colors_Control_Console
{
    internal class Program
    {
        private static Client client;
        private static bool run = true;
        private static Thread Writer;

        private static void Main()
        {
            client = new Client();
            Console.Title = "Galactic Colors Control Client";
            Console.Write(">");
            Write("Galactic Colors Control Client");
            Write("Console " + Assembly.GetEntryAssembly().GetName().Version.ToString());
            bool hostSet = false;
            while(!hostSet)
            {
                Write("Enter server host:");
                string host = client.ValidateHost(Console.ReadLine());
                if(host == null)
                {
                    foreach (string output in client.Output.ToArray())
                    {
                        Write(output);
                    }
                    client.Output.Clear();
                    client.ResetHost();
                }
                else
                {
                    Write("Use " + host + "? y/n");
                    ConsoleKeyInfo c = new ConsoleKeyInfo();
                    while(c.Key != ConsoleKey.Y && c.Key != ConsoleKey.N)
                    {
                        c = Console.ReadKey();
                    }
                    if(c.Key == ConsoleKey.Y)
                    {
                        hostSet = true;
                    }
                    else
                    {
                        client.ResetHost();
                    }
                }
            }
            if (client.ConnectHost())
            {
                run = true;
                Writer = new Thread(OutputWriter);
                Writer.Start();
                while (run)
                {
                    client.SendRequest(Console.ReadLine());
                    if (!client.isRunning) { run = false; }
                }
                Writer.Join();
                Console.Read();
            }
            else
            {
                foreach (string output in client.Output.ToArray())
                {
                    Write(output);
                }
                client.Output.Clear();
                Console.Read();
            }
        }

        private static void OutputWriter()
        {
            while (run || client.Output.Count > 0)
            {
                if (client.Output.Count > 0)
                {
                    string text = client.Output[0];
                    Write(text);
                    client.Output.Remove(text);
                }
                Thread.Sleep(200);
            }
        }

        private static void Write( string text)
        {
            Console.Write("\b");
            Console.WriteLine(text);
            Console.Write(">");
        }
    }
}