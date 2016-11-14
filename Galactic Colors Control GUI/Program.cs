using System;

namespace Galactic_Colors_Control_GUI
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            //TODO add debug and more
            using (var game = new Game())
                game.Run();
        }
    }
}