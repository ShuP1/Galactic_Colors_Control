using MyCommon;

namespace Galactic_Colors_Control_Common.Protocol
{
    public enum ResultTypes { Error, OK }

    /// <summary>
    /// Server to Client Result from RequestData
    /// </summary>
    public class ResultData : Data
    {
        public int id; //Client Side Autoindent
        public ResultTypes type;
        public string[] result;

        public ResultData(int Id, ResultTypes Type, string[] Result = null)
        {
            id = Id;
            type = Type;
            result = Result;
        }

        public ResultData(int Id, RequestResult Result)
        {
            id = Id;
            type = Result.type;
            result = Result.result;
        }

        public ResultData(ref byte[] bytes)
        {
            if (!Binary.TryToInt(ref bytes, out id))
                return;

            int ntype;
            if (!Binary.TryToInt(ref bytes, out ntype))
                return;

            type = (ResultTypes)ntype;
            if (!Binary.TryToStringArray(ref bytes, out result))
                return;
        }

        public override byte[] ToBytes()
        {
            return Binary.AddBytes(Binary.FromInt((int)DataType.Result), Binary.FromInt(id), Binary.FromInt((int)type), Binary.FromStringArray(result));
        }

        public override string ToSmallString()
        {
            return type.ToString() + "|" + Strings.ArrayToString(result);
        }

        public override string ToLongString()
        {
            return "Result : " + type.ToString() + "|" + Strings.ArrayToString(result) + "|" + id;
        }
    }
}