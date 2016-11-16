//TODO add party support
using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyMonoGame.GUI;
using System.Threading;
using System;
using Microsoft.Xna.Framework.Input;

namespace Galactic_Colors_Control_GUI.States
{
    public class GameState : State
    {
        private bool showChat = false;
        private string chatText;
        private string chatInput;

        private bool showLoading = false;
        private bool showOKMessage = false;
        private Message message;

        public GameState()
        {
            Game.singleton.client.OnEvent += new EventHandler(OnEvent); //Set OnEvent function
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            Game.singleton.background.Draw(spritebatch);
            Game.singleton.GUI.Texture(new Rectangle(0, 0, Game.singleton.ScreenWidth, 30), Game.nullSprite, new MyMonoGame.Colors(new Color(0.1f, 0.1f, 0.1f)));
            if (Game.singleton.GUI.Button(new Rectangle(5, 5, 50, 20), (showChat ? Game.singleton.multilang.Get("Hide", Game.singleton.config.lang) : Game.singleton.multilang.Get("Show", Game.singleton.config.lang)) + " " + Game.singleton.multilang.Get("Chat", Game.singleton.config.lang), Game.singleton.fonts.small, new MyMonoGame.Colors(Color.White, Color.LightGray, Color.Gray))) { Game.singleton.GUI.ResetFocus(); showChat = !showChat; }

            if (showChat)
            {
                Game.singleton.GUI.Box(new Rectangle(0, 30, 310, 310), Game.singleton.buttonsSprites[0]);
                if (Game.singleton.GUI.TextField(new Rectangle(5, 35, 305, 20), ref chatInput, Game.singleton.fonts.basic, null, Manager.textAlign.centerLeft, Game.singleton.multilang.Get("EnterMessage", Game.singleton.config.lang))) { if (chatInput != null) { new Thread(ChatEnter).Start(); } }
                Game.singleton.GUI.Label(new Rectangle(5, 60, 305, 245), chatText, Game.singleton.fonts.small, null, Manager.textAlign.topLeft, true);
            }

            if (showLoading)
            {
                Game.singleton.GUI.Box(new Rectangle(Game.singleton.ScreenWidth / 2 - 150, Game.singleton.ScreenHeight / 4 + 50, 300, 50), Game.singleton.buttonsSprites[0]);
                Game.singleton.GUI.Label(new Rectangle(Game.singleton.ScreenWidth / 2 - 150, Game.singleton.ScreenHeight / 4 + 50, 300, 50), Game.singleton.multilang.Get("Loading", Game.singleton.config.lang), Game.singleton.fonts.basic);
            }
            else
            {
                if (showOKMessage)
                {
                    Game.singleton.GUI.Box(new Rectangle(Game.singleton.ScreenWidth / 2 - 150, Game.singleton.ScreenHeight / 4 + 50, 300, 150), Game.singleton.buttonsSprites[0]);
                    Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 4 + 60), message.title, Game.singleton.fonts.basic, null, Manager.textAlign.bottomCenter);
                    Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 4 + 100), message.text, Game.singleton.fonts.small, null, Manager.textAlign.bottomCenter);
                    if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 140, Game.singleton.ScreenHeight / 4 + 150, 280, 40), Game.singleton.buttonsSprites[0], Game.singleton.multilang.Get("OK", Game.singleton.config.lang), Game.singleton.fonts.basic)) { Game.singleton.GUI.ResetFocus(); showOKMessage = false; Game.singleton.client.ExitHost(); }
                }
            }
        }

        public override void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || (!Game.singleton.client.isRunning)) {
                Game.singleton.client.ExitHost();
                Game.singleton.gameState = new MainMenuState();
            }
        }

        private void ChatEnter()
        {
            string request = chatInput;
            chatInput = null;

            if (request == null)
                return;

            if (request.Length == 0)
                return;

            ResultData res;
            if (request[0] == Game.singleton.config.commandChar)
            {
                request = request.Substring(1);
                res = Game.singleton.client.Request(Common.SplitArgs(request));
                ChatText(Game.singleton.multilang.GetResultText(res, Game.singleton.config.lang));
            }
            else
            {
                res = Game.singleton.client.Request(Common.Strings("say", request));
                if (res.type != ResultTypes.OK)
                {
                    ChatText(Game.singleton.multilang.GetResultText(res, Game.singleton.config.lang));
                }
            }
        }

        private void OnEvent(object sender, EventArgs e)
        {
            //TODO add PartyKick
            EventData eve = ((EventDataArgs)e).Data;
            if (eve.type == EventTypes.ServerKick)
            {
                message.title = Game.singleton.multilang.Get("ServerKick", Game.singleton.config.lang);
                message.text = Common.ArrayToString(eve.data);
                showOKMessage = true;
            }else
            {
                ChatText(Game.singleton.multilang.GetEventText(eve, Game.singleton.config.lang));
            }
        }

        public void ChatText(string text)
        {
            chatText += (text + Environment.NewLine);
        }

        /*
        private void PartyClick()
        {
            showLoading = true;
            GUI.ResetFocus();
            //TODO
            /*
            if (showParty)
            {
                client.SendRequest("/party leave");
                showParty = false;
                showLoading = false;
            }
            else
            {
                client.Output.Clear();
                client.SendRequest("/party list");
                int wait = 0;
                while (wait < 20)
                {
                    if (client.Output.Count > 0)
                    {
                        wait = 20;
                    }
                    else
                    {
                        wait++;
                        Thread.Sleep(200);
                    }
                }
                if (client.Output.Count > 0)
                {
                    Thread.Sleep(500);
                    if (client.Output.Count > 1)
                    {
                        messageTitle = "Party";
                        messageText = string.Empty;
                        foreach (string line in client.Output.ToArray()) { messageText += (line + Environment.NewLine); }
                        showOKMessage = true;
                        client.Output.Clear();
                    }
                    else
                    {
                        messageTitle = "Any party";
                        messageText = string.Empty;
                        foreach (string line in client.Output.ToArray()) { messageText += (line + Environment.NewLine); }
                        showOKMessage = true;
                        client.Output.Clear();
                    }
                }
                else
                {
                    messageTitle = "Timeout";
                    messageText = "";
                    showOKMessage = true;
                    showLoading = false;
                    client.Output.Clear();
                }
            }
    }*/
    }
}