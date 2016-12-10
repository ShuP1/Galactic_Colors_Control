using Galactic_Colors_Control_Common;
using System;
using Console = MyConsole.ConsoleIO;
using MyConsole;

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
                        Console.Write(new ColorStrings(new ColorString("Use"), new ColorString(" --debug", System.ConsoleColor.Red), new ColorString(" or"), new ColorString(" --dev", System.ConsoleColor.White, System.ConsoleColor.Red)));
                        break;
                }
            }
            using (var game = new Game())
                game.Run();
        }
    }
}