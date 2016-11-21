namespace Galactic_Colors_Control_Common.Protocol
{
    /// <summary>
    /// Packet Master Class
    /// </summary>
    public class Data
    {
        public enum DataType { Request, Result, Event };

        /// <summary>
        /// Create Packet from bytes
        /// </summary>
        /// <param name="bytes">row bytes (remove used bytes)</param>
        public static Data FromBytes(ref byte[] bytes)
        {
            int type;
            if (!Binary.TryToInt(ref bytes, out type))
                return null;

            switch ((DataType)type)
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

        /// <summary>
        /// Small readable text
        /// </summary>
        public virtual string ToSmallString()
        {
            return null;
        }

        /// <summary>
        /// Long readble text
        /// </summary>
        public virtual string ToLongString()
        {
            return null;
        }

        /// <summary>
        /// Generate bytes to send
        /// </summary>
        public virtual byte[] ToBytes()
        {
            return new byte[0];
        }
    }
}