namespace Galactic_Colors_Control_Common.Protocol
{
    public class RequestData : Data
    {
        public int id;
        public string[] args;

        public RequestData(int p1, string[] p2)
        {
            id = p1;
            args = p2;
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