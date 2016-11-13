using Galactic_Colors_Control_Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Galactic_Colors_Control_Server
{
    public class Logger
    {
        public enum logType { dev, debug, info, warm, error, fatal }

        public enum logConsole { normal, show, hide }

        private static List<Log> toWriteLogs = new List<Log>();
        private static string logPath;
        public static Thread Updater;
        public static bool _run = true;

        public struct Log
        {
            public string text;
            public logType type;
            public logConsole console;

            public Log(string p1, logType p2, logConsole p3 = logConsole.normal)
            {
                text = p1;
                type = p2;
                console = p3;
            }
        }

        /// <summary>
        /// Create log file and start logger thread
        /// </summary>
        public static void Initialise()
        {
            if (!Directory.Exists(Program.config.logPath))
            {
                Directory.CreateDirectory(Program.config.logPath);
                Write("Log Directory Created", logType.info);
            }
            else
            {
                //Sort old logs
                string[] files = Directory.GetFiles(Program.config.logPath);
                foreach (string file in files)
                {
                    if (Path.GetExtension(file) == ".log")
                    {
                        string name = Path.GetFileName(file);
                        name = name.Substring(0, Math.Min(name.Length, 10));
                        if (name.Length == 10)
                        {
                            if (name != DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
                            {
                                int y;
                                int m;
                                int d;
                                if (int.TryParse(new string(name.Take(4).ToArray()), out y) && int.TryParse(new string(name.Skip(5).Take(2).ToArray()), out m) && int.TryParse(new string(name.Skip(8).Take(2).ToArray()), out d))
                                {
                                    if (!Directory.Exists(Program.config.logPath + "/" + y + "/" + m + "/" + d))
                                    {
                                        Directory.CreateDirectory(Program.config.logPath + "/" + y + "/" + m + "/" + d);
                                    }
                                    File.Move(file, Program.config.logPath + "/" + y + "/" + m + "/" + d + "/" + Path.GetFileName(file));
                                }
                            }
                        }
                    }
                }
            }
            int i = 0;
            while (File.Exists(Program.config.logPath + "/" + DateTime.UtcNow.ToString("yyyy-MM-dd-", CultureInfo.InvariantCulture) + i + ".log")) { i++; }
            logPath = Program.config.logPath + "/" + DateTime.UtcNow.ToString("yyyy-MM-dd-", CultureInfo.InvariantCulture) + i + ".log";
            Write("Log path:" + logPath, logType.debug);
            Updater = new Thread(new ThreadStart(UpdaterLoop));
            Updater.Start();
        }

        /// <summary>
        /// Add log to log pile
        /// </summary>
        /// <param name="text">Log text</param>
        /// <param name="type">Log status</param>
        /// <param name="console">Server display modifier</param>
        public static void Write(string text, logType type, logConsole console = logConsole.normal)
        {
            Write(new Log(text, type, console));
        }

        /// <summary>
        /// Add log to log pile
        /// </summary>
        /// <param name="log">Log struct</param>
        public static void Write(Log log)
        {
            if (Program.config.logLevel == logType.debug || Program.config.logLevel == logType.dev)
            {
                //Add Source Method
                log.text = "[" + new StackTrace().GetFrame(2).GetMethod().Name + "]: " + log.text;
            }
            toWriteLogs.Add(log);
        }

        /// <summary>
        /// Write log pile to logfile and console
        /// </summary>
        public static void UpdaterLoop()
        {
            while (_run || toWriteLogs.Count > 0)
            {
                while (toWriteLogs.Count > 0)
                {
                    Log log = toWriteLogs[0];
                    if (log.type >= Program.config.logLevel)
                    {
                        File.AppendAllText(logPath, DateTime.UtcNow.ToString("[yyyy-MM-dd]", CultureInfo.InvariantCulture) + " [" + log.type.ToString().ToUpper() + "]: " + log.text + Environment.NewLine);
                        if (log.console != logConsole.hide)
                        {
                            Console.BackgroundColor = Program.config.logBackColor[(int)log.type];
                            Console.ForegroundColor = Program.config.logForeColor[(int)log.type];
                            Console.Write("\b");
                            Console.WriteLine(DateTime.UtcNow.ToString("[yyyy-MM-dd]", CultureInfo.InvariantCulture) + ": " + log.text);
                            Common.ConsoleResetColor();
                            Console.Write(">");
                        }
                    }
                    //TODO reactive show logger
                    /*
                    else
                    {
                        if(log.console == logConsole.show)
                        {
                            Console.BackgroundColor = Program.config.logBackColor[(int)log.type];
                            Console.ForegroundColor = Program.config.logForeColor[(int)log.type];
                            Console.Write("\b");
                            Console.WriteLine(DateTime.UtcNow.ToString("[yyyy-MM-dd]", CultureInfo.InvariantCulture) + ": " + log.text);
                            Utilities.ConsoleResetColor();
                            Console.Write(">");
                        }
                    }*/
                    toWriteLogs.Remove(log);
                }
                Thread.Sleep(200);
            }
        }
    }
}