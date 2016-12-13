using MyCommon;
using System;
using System.Xml.Serialization;

namespace Galactic_Colors_Control_Server
{
    [XmlRoot("config")]
    public class Config
    {
        public string logPath = AppDomain.CurrentDomain.BaseDirectory + "Logs";
        public Logger.logType logLevel = Logger.logType.info;
        public int port = 25001;
        public int size = 20;
        public ConsoleColor[] logForeColor = new ConsoleColor[6] { ConsoleColor.DarkGray, ConsoleColor.Gray, ConsoleColor.White, ConsoleColor.Yellow, ConsoleColor.Red, ConsoleColor.White };
        public ConsoleColor[] logBackColor = new ConsoleColor[6] { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Red };
        public int lang = 1;
        public int partysize = 10;

        public void PreSave()
        {
            if (Server._debug || Server._dev) { logLevel = Logger.logType.info; }
        }

        public void PostSave()
        {
            if (Server._debug) { logLevel = Logger.logType.debug; }
            if (Server._dev) { logLevel = Logger.logType.dev; }
        }
    }
}