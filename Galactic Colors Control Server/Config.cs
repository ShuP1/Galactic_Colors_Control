using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
        public ConsoleColor[] logForeColor = new ConsoleColor[5] {ConsoleColor.Gray , ConsoleColor.White, ConsoleColor.Yellow, ConsoleColor.Red, ConsoleColor.White};
        public ConsoleColor[] logBackColor = new ConsoleColor[5] { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Red };

        /// <summary>
        /// Load config from xml file
        /// App.config is too easy
        /// </summary>
        /// <returns>Loaded config</returns>
        public Config Load()
        {
            Logger.Write("Loading config", Logger.logType.info);
            Config config = new Config();
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Config.xml"))
            {
                if (CorrectConfig())
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Config));
                    using (StreamReader re = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "Config.xml"))
                    {
                        config = xs.Deserialize(re) as Config;
                    };
                }
                else
                {
                    Logger.Write("Old config in Config.xml.old", Logger.logType.warm);
                    File.Move(AppDomain.CurrentDomain.BaseDirectory + "Config.xml", AppDomain.CurrentDomain.BaseDirectory + "Config.xml.old");
                    config.Save();
                }
            }
            else
            {
                Logger.Write("Any config file", Logger.logType.error);
                config.Save();
            }
            if (Program._debug) { config.logLevel = Logger.logType.debug; }
            return config;
        }

        /// <summary>
        /// Write actual config in xml file
        /// </summary>
        public void Save()
        {
            XmlSerializer xs = new XmlSerializer(typeof(Config));
            if (Program._debug) { logLevel = Logger.logType.info; }
            using (StreamWriter st = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Config.xml"))
            {
                xs.Serialize(st,this);
            };
            if (Program._debug) { logLevel = Logger.logType.debug; }
        }

        /// <summary>
        /// Check config format using Schema
        /// </summary>
        public bool CorrectConfig()
        {
            bool isCorrect = false;

            using (Stream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "Config.xml", FileMode.Open))
            {
                XmlReader re = new XmlTextReader(fs);
                XmlSerializer xs = new XmlSerializer(typeof(Config));
                try
                {
                    isCorrect = xs.CanDeserialize(re);
                }
                catch (XmlException e)
                {
                    isCorrect = false;
                    Logger.Write("Error: " + e.Message, Logger.logType.error);
                }
            }

            if (isCorrect)
            {
                try
                {
                    XmlDocument d = new XmlDocument();
                    d.Load(AppDomain.CurrentDomain.BaseDirectory + "Config.xml");
                    d.Schemas.Add("", XmlReader.Create("ConfigSchema.xsd"));

                    d.Validate((o, e) =>
                    {
                        Logger.Write("Error: " + e.Message, Logger.logType.error);
                        isCorrect = false;
                    });
                }
                catch (XmlException e)
                {
                    isCorrect = false;
                    Logger.Write("Error: " + e.Message, Logger.logType.error);
                }
            }

            return isCorrect;
        }
    }
}
