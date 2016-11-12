namespace Galactic_Colors_Control_Common.Protocol
{
    public class Data
    {
        public enum DataType { Request, Result, Event };

        public static Data FromBytes(ref byte[] bytes)
        {
            switch ((DataType)Binary.ToInt(ref bytes))
            {
                case DataType.Request:
                    return new RequestData(ref bytes);

                case DataType.Result:
                    return new ResultData(ref bytes);

                case DataType.Event:
                    return new EventData(ref bytes);

                default:
                    return null;
            }
        }

        public virtual string ToSmallString()
        {
            return null;
        }

        public virtual string ToLongString()
        {
            return null;
        }

        public virtual byte[] ToBytes()
        {
            return new byte[0];
        }
    }
}