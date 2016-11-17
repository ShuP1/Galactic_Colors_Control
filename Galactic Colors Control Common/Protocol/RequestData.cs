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
            id = Binary.ToInt(ref bytes);
            args = Binary.ToStringArray(ref bytes);
        }

        public override byte[] ToBytes()
        {
            return Binary.AddBytes(Binary.FromInt((int)DataType.Request), Binary.FromInt(id), Binary.FromStringArray(args));
        }

        public override string ToSmallString()
        {
            return Common.ArrayToString(args);
        }

        public override string ToLongString()
        {
            return "Request : " + Common.ArrayToString(args) + "|" + id;
        }
    }
}