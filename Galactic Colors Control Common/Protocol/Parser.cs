using MyCommon;

namespace Galactic_Colors_Control_Common.Protocol
{
    public static class Parser
    {
        public static string GetEventText(EventData eve, int lang, MultiLang dico)
        {
            string data = Common.ArrayToString(eve.data);
            switch (eve.type)
            {
                case EventTypes.ChatMessage:
                    return data;

                case EventTypes.PartyJoin:
                case EventTypes.PartyLeave:
                case EventTypes.ServerJoin:
                case EventTypes.ServerLeave:
                    return data + " " + dico.GetWord(eve.type.ToString(), lang);

                default:
                    return eve.ToSmallString();
            }
        }

        public static string GetResultText(ResultData res, int lang, MultiLang dico)
        {
            string data = Common.ArrayToString(res.result);
            if (res.type == ResultTypes.Error)
                data = dico.GetWord("Error", lang) + ": " + data;
            return data;
        }
    }
}
