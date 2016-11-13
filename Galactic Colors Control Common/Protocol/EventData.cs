namespace Galactic_Colors_Control_Common.Protocol
{
    public enum EventTypes
    {
        ChatMessage, //To displat in chatbox
        ServerJoin, //A player join server
        ServerLeave, //A player leave server
        ServerKick, //You are kick from server
        PartyJoin, //A player join your party
        PartyLeave, //A player leave your party
        PartyKick //Tou are jick from your party
    }

    /// <summary>
    /// Server to Client Packet
    /// </summary>
    public class EventData : Data
    {
        public EventTypes type;
        public string[] data; //EventArgs like

        public EventData(EventTypes Type, string[] Data = null)
        {
            type = Type;
            data = Data;
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