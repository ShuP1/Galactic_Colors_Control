using System;
using System.Linq;

namespace Galactic_Colors_Control_Common
{
    public static class Common
    {
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
                    text += (str + Environment.NewLine);
                }
            }
            return text;
        }

        public static string ArrayToString(int[] array)
        {
            string text = "";
            foreach (int i in array)
            {
                text += (i.ToString() + Environment.NewLine);
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