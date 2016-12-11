using MyCommon;

namespace Galactic_Colors_Control_Common.Protocol
{
    /// <summary>
    /// Client to Server Data request packet 'allways' return ResultData
    /// </summary>
    public class RequestData : Data
    {
        public int id; //Client Size autoindent id
        public string[] args;

        public RequestData(int Id, string[] Args)
        {
            id = Id;
            args = Args;
        }

        public RequestData(ref byte[] bytes)
        {
            if (!Binary.TryToInt(ref bytes, out id))
                return;

            if (!Binary.TryToStringArray(ref bytes, out args))
                return;
        }

        public override byte[] ToBytes()
        {
            return Binary.AddBytes(Binary.FromInt((int)DataType.Request), Binary.FromInt(id), Binary.FromStringArray(args));
        }

        public override string ToSmallString()
        {
            return Strings.ArrayToString(args);
        }

        public override string ToLongString()
        {
            return "Request : " + Strings.ArrayToString(args) + "|" + id;
        }
    }
}