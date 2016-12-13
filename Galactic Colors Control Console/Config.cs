﻿using MyCommon;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Galactic_Colors_Control_Console
{
    [XmlRoot("config")]
    public class Config
    {
        public string logPath = AppDomain.CurrentDomain.BaseDirectory + "Logs";
        public Logger.logType logLevel = Logger.logType.info;
        public char commandChar = '/';
        public ConsoleColor[] logForeColor = new ConsoleColor[6] { ConsoleColor.DarkGray, ConsoleColor.Gray, ConsoleColor.White, ConsoleColor.Yellow, ConsoleColor.Red, ConsoleColor.White };
        public ConsoleColor[] logBackColor = new ConsoleColor[6] { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Red };
        public int lang = 1;

        public void PreSave()
        {
            if (Console._debug || Console._dev) { logLevel = Logger.logType.info; }
        }

        public void PostSave()
        {
            if (Console._debug) { logLevel = Logger.logType.debug; }
            if (Console._dev) { logLevel = Logger.logType.dev; }
        }
    }
}