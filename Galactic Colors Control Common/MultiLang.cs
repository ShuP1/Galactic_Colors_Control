using Galactic_Colors_Control_Common.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Galactic_Colors_Control_Common
{
    public class MultiLang
    {
        private Dictionary<string, List<string>> _multiDictionary = new Dictionary<string, List<string>>(); //List of phrases by key
        private List<string> _Langs = new List<string>(); //Readable langs list

        public Dictionary<string, List<string>> multiDictionary { get { return _multiDictionary; } }
        public List<string> Langs { get { return _Langs; } }

        public void Load()
        {
            _multiDictionary.Clear();
            _Langs.Clear();
            string[] lines = Properties.Resources.Lang.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries); //Load from .cvs ressources. //TODO add more langs
            _Langs = lines[0].Split(';').OfType<string>().ToList();
            _Langs.RemoveAt(0);
            foreach (string line in lines)
            {
                List<string> items = line.Split(';').OfType<string>().ToList();
                string key = items[0];
                items.RemoveAt(0);
                _multiDictionary.Add(key, items);
            }
        }

        public string GetEventText(EventData eve, int lang)
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
                    return data + " " + Get(eve.type.ToString(), lang);

                default:
                    return eve.ToSmallString();
            }
        }

        public string GetResultText(ResultData res, int lang)
        {
            string data = Common.ArrayToString(res.result);
            if (res.type == ResultTypes.Error)
                data = Get("Error", lang) + ": " + data;
            return data;
        }

        public string Get(string Key, int Lang)
        {
            string text = "";

            if (_multiDictionary.ContainsKey(Key))
            {
                if (_multiDictionary[Key].Count >= Lang)
                {
                    text = _multiDictionary[Key][Lang];
                }
                else
                {
                    text = "!!!UNKNOW LANG KEY!!!";
                }
            }
            else
            {
                text = "!!!UNKNOW WORD KEY!!!";
            }

            return text;
        }
    }
}