using System;
using System.Linq;
using System.Text;

namespace Galactic_Colors_Control_Common
{
    public static class Binary
    {
        public static bool ToBool(ref byte[] bytes)
        {
            byte[] data = new byte[1];
            data = bytes.Take(1).ToArray();
            RemoveFirst(ref bytes, 1);
            return data[1] == 1 ? true : false;
        }

        public static byte[] FromBool(bool x)
        {
            return x ? new byte[1] { 1 } : new byte[1] { 0 };
        }

        public static string ToString(ref byte[] bytes)
        {
            int len = ToInt(ref bytes);
            string text = Encoding.ASCII.GetString(bytes.Take(len).ToArray());
            RemoveFirst(ref bytes, len);
            return text;
        }

        public static byte[] FromString(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            return AddBytes(FromInt(data.Length), data);
        }

        public static int ToInt(ref byte[] bytes)
        {
            if (bytes == null)
                return -1;

            if (bytes.Length < 4)
                return -1;

            byte[] data = new byte[4];
            data = bytes.Take(4).ToArray();
            data.Reverse();
            RemoveFirst(ref bytes, 4);
            return BitConverter.ToInt32(data, 0);
        }

        public static byte[] FromInt(int x)
        {
            byte[] data = new byte[4];
            data = BitConverter.GetBytes(x);
            return data;
        }

        public static string[] ToStringArray(ref byte[] bytes)
        {
            int len = ToInt(ref bytes);
            if (len < 1 || len > 10000)
                return new string[0];

            string[] data = new string[len];
            for (int i = 0; i < len; i++)
            {
                data[i] = ToString(ref bytes);
            }
            return data;
        }

        public static byte[] FromStringArray(string[] array)
        {
            if (array == null)
                return new byte[0];

            byte[] data = FromInt(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                data = AddBytes(data, FromString(array[i]));
            }
            return data;
        }

        public static int[] ToIntArray(ref byte[] bytes)
        {
            int len = ToInt(ref bytes);
            int[] data = new int[len];
            for (int i = 0; i < len; i++)
            {
                data[i] = ToInt(ref bytes);
            }
            return data;
        }

        public static byte[] FromIntArray(int[] array)
        {
            byte[] data = FromInt(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                data = AddBytes(data, FromInt(array[i]));
            }
            return data;
        }

        public static byte[] AddBytes(params byte[][] arguments)
        {
            byte[] res = new byte[arguments.Sum(a => a.Length)];
            int offset = 0;
            for (int i = 0; i < arguments.Length; i++)
            {
                Buffer.BlockCopy(arguments[i], 0, res, offset, arguments[i].Length);
                offset += arguments[i].Length;
            }
            return res;
        }

        public static void RemoveFirst(ref byte[] bytes, int count)
        {
            byte[] newbytes = new byte[bytes.Length - count];
            newbytes = bytes.Skip(count).ToArray();
            bytes = newbytes;
        }
    }
}