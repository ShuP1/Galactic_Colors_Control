using System;
using System.Linq;

namespace Galactic_Colors_Control_Common
{
    public static class Common
    {
        public static string dictionary = Properties.Resources.Lang;

        /// <summary>
        /// Simpler string array creation
        /// </summary>
        public static string[] Strings(params string[] args)
        {
            return args;
        }

        public static string ArrayToString(string[] array)
        {
            string text = "";
            if (array != null)
            {
                foreach (string str in array)
                {
                    text += ((text == "" ? "" : Environment.NewLine) + str);
                }
            }
            return text;
        }

        public static string ArrayToString(int[] array)
        {
            string text = "";
            foreach (int i in array)
            {
                text += ((text == "" ? "" : Environment.NewLine) + i.ToString());
            }
            return text;
        }

        public static string[] SplitArgs(string text)
        {
            return text.Split('"')
                     .Select((element, index) => index % 2 == 0
                                           ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                           : new string[] { element })
                     .SelectMany(element => element).ToArray();
        }
    }
}