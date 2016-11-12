namespace Galactic_Colors_Control_Common.Protocol
{
    public enum ResultTypes { Error, OK }

    public class ResultData : Data
    {
        public int id;
        public ResultTypes type;
        public string[] result;

        public ResultData(int p1, ResultTypes p2, string[] p3 = null)
        {
            id = p1;
            type = p2;
            result = p3;
        }

        public ResultData(int p1, RequestResult p2)
        {
            id = p1;
            type = p2.type;
            result = p2.result;
        }

        public ResultData(ref byte[] bytes)
        {
            id = Binary.ToInt(ref bytes);
            type = (ResultTypes)Binary.ToInt(ref bytes);
            result = Binary.ToStringArray(ref bytes);
        }

        public override byte[] ToBytes()
        {
            return Binary.AddBytes(Binary.FromInt((int)DataType.Result), Binary.FromInt(id), Binary.FromInt((int)type), Binary.FromStringArray(result));
        }

        public override string ToSmallString()
        {
            return type.ToString() + "|" + Common.ArrayToString(result);
        }

        public override string ToLongString()
        {
            return "Result : " + type.ToString() + "|" + Common.ArrayToString(result) + "|" + id;
        }
    }
}