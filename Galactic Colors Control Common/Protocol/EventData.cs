namespace Galactic_Colors_Control_Common.Protocol
{
    public enum EventTypes { ChatMessage, ServerJoin, ServerLeave, ServerKick, PartyJoin, PartyLeave, PartyKick }

    public class EventData : Data
    {
        public EventTypes type;
        public string[] data;

        public EventData(EventTypes p1, string[] p2 = null)
        {
            type = p1;
            data = p2;
        }

        public EventData(ref byte[] bytes)
        {
            type = (EventTypes)Binary.ToInt(ref bytes);
            data = Binary.ToStringArray(ref bytes);
        }

        public override byte[] ToBytes()
        {
            return Binary.AddBytes(Binary.FromInt((int)DataType.Event), Binary.FromInt((int)type), Binary.FromStringArray(data));
        }

        public override string ToSmallString()
        {
            return type.ToString() + "|" + Common.ArrayToString(data);
        }

        public override string ToLongString()
        {
            return "Event : " + ToSmallString();
        }
    }
}