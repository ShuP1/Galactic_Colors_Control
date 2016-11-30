using System;
using System.Collections.Generic;
using Cons = System.Console;

namespace Galactic_Colors_Control_Common
{
    public class Console
    {
        private static string inputBuffer = "";
        private static List<ColorStrings> outputBuffer = new List<ColorStrings>();
        public static string Title { get { return Cons.Title; } set { Cons.Title = value; } }

        public static void Write(ColorStrings Text)
        {
            outputBuffer.Add(Text);
            while (outputBuffer.Count > Cons.WindowHeight - 2) { outputBuffer.RemoveAt(0); }
            Update();
        }

        public static string Read()
        {
            ConsoleKeyInfo key = Cons.ReadKey();
            while (key.Key != ConsoleKey.Enter)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Backspace:
                        if (inputBuffer.Length == 0) { SetInputPos(); }
                        if (inputBuffer.Length == 1) { inputBuffer = ""; }
                        if (inputBuffer.Length > 1) { inputBuffer = inputBuffer.Substring(0, inputBuffer.Length - 1); }
                        break;

                    default:
                        inputBuffer += key.KeyChar;
                        break;
                }
                key = Cons.ReadKey();
            }
            Cons.WriteLine();
            string res = inputBuffer;
            inputBuffer = "";
            return res;
        }

        private static void Update()
        {
            Cons.Clear();
            Cons.SetCursorPosition(0, 0);
            foreach (ColorStrings output in outputBuffer) { output.Write(); }
            SetInputPos();
            Cons.ForegroundColor = ConsoleColor.White;
            Cons.BackgroundColor = ConsoleColor.Black;
            Cons.Write("> " + inputBuffer);
        }

        private static void SetInputPos()
        {
            Cons.SetCursorPosition(0, Math.Max(Cons.WindowHeight - 1, Cons.CursorTop + 1));
        }

        public static void ClearInput()
        {
            inputBuffer = ""; Update();
        }

        public static void ClearOutput()
        {
            outputBuffer.Clear(); Update();
        }
    }

    public class ColorStrings
    {
        public ColorString[] Text;

        public ColorStrings(params ColorString[] strings)
        {
            Text = strings;
        }

        public ColorStrings(string text)
        {
            Text = new ColorString[1] { new ColorString(text) };
        }

        public void Write()
        {
            foreach (ColorString cstring in Text)
            {
                Cons.BackgroundColor = cstring.Back;
                Cons.ForegroundColor = cstring.Fore;
                Cons.Write(cstring.Text);
            }
            Cons.WriteLine();
        }
    }

    public class ColorString
    {
        public string Text;
        public ConsoleColor Fore;
        public ConsoleColor Back;

        public ColorString(string text, ConsoleColor fore = ConsoleColor.White, ConsoleColor back = ConsoleColor.Black)
        {
            Text = text;
            Fore = fore;
            Back = back;
        }
    }
}