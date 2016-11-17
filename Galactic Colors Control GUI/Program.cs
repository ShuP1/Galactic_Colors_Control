using Galactic_Colors_Control_Common;
using System;

namespace Galactic_Colors_Control_GUI
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        public static bool _dev = false;
        public static bool _debug = false;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "--debug":
                        _debug = true;                     
                        break;

                    case "--dev":
                        _dev = true;              
                        break;

                    default:
                        Common.ConsoleWrite("Use --debug or --dev");
                        break;
                }
            }
            using (var game = new Game())
                game.Run();
        }
    }
}