using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyMonoGame.GUI;
using System;
using System.Threading;

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
            Game.singleton.GUI.Texture(new Rectangle(0, 0, Game.singleton.ScreenWidth, 30), MyMonoGame.Utilities.Content.nullSprite, new MyMonoGame.Colors(new Color(0.1f, 0.1f, 0.1f)));
            if (Game.singleton.GUI.Button(new Rectangle(5, 5, 50, 20), (showChat ? Game.singleton.multilang.GetWord("Hide", Game.singleton.config.lang) : Game.singleton.multilang.GetWord("Show", Game.singleton.config.lang)) + " " + Game.singleton.multilang.GetWord("Chat", Game.singleton.config.lang), Game.singleton.GUI.content.GetFont("small"), new MyMonoGame.Colors(Color.White, Color.LightGray, Color.Gray))) { Game.singleton.GUI.ResetFocus(); showChat = !showChat; }

            if (showChat)
            {
                Game.singleton.GUI.Box(new Rectangle(0, 30, 310, 310), Game.singleton.GUI.content.GetBox("Default"));
                if (Game.singleton.GUI.TextField(new Rectangle(5, 35, 305, 20), ref chatInput, Game.singleton.GUI.content.GetFont("basic"), null, Manager.textAlign.centerLeft, Game.singleton.multilang.GetWord("EnterMessage", Game.singleton.config.lang))) { if (chatInput != null) { new Thread(ChatEnter).Start(); } }
                Game.singleton.GUI.Label(new Rectangle(5, 60, 305, 245), chatText, Game.singleton.GUI.content.GetFont("small"), null, Manager.textAlign.topLeft, true);
            }

            if (showLoading)
            {
                Game.singleton.GUI.Box(new Rectangle(Game.singleton.ScreenWidth / 2 - 150, Game.singleton.ScreenHeight / 4 + 50, 300, 50), Game.singleton.GUI.content.GetBox("Default"));
                Game.singleton.GUI.Label(new Rectangle(Game.singleton.ScreenWidth / 2 - 150, Game.singleton.ScreenHeight / 4 + 50, 300, 50), Game.singleton.multilang.GetWord("Loading", Game.singleton.config.lang), Game.singleton.GUI.content.GetFont("basic"));
            }
            else
            {
                if (showOKMessage)
                {
                    Game.singleton.GUI.Box(new Rectangle(Game.singleton.ScreenWidth / 2 - 150, Game.singleton.ScreenHeight / 4 + 50, 300, 150), Game.singleton.GUI.content.GetBox("Default"));
                    Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 4 + 60), message.title, Game.singleton.GUI.content.GetFont("basic"), null, Manager.textAlign.bottomCenter);
                    Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 4 + 100), message.text, Game.singleton.GUI.content.GetFont("small"), null, Manager.textAlign.bottomCenter);
                    if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 140, Game.singleton.ScreenHeight / 4 + 150, 280, 40), Game.singleton.GUI.content.GetBox("Default"), Game.singleton.multilang.GetWord("OK", Game.singleton.config.lang), Game.singleton.GUI.content.GetFont("basic"))) { Game.singleton.GUI.ResetFocus(); showOKMessage = false; Game.singleton.client.ExitHost(); }
                }
            }
        }

        public override void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || (!Game.singleton.client.isRunning))
            {
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
                ChatText(Parser.GetResultText(res, Game.singleton.config.lang, Game.singleton.multilang));
            }
            else
            {
                res = Game.singleton.client.Request(Common.Strings("say", request));
                if (res.type != ResultTypes.OK)
                {
                    ChatText(Parser.GetResultText(res, Game.singleton.config.lang, Game.singleton.multilang));
                }
            }
        }

        private void OnEvent(object sender, EventArgs e)
        {
            EventData eve = ((EventDataArgs)e).Data;
            if (eve.type == EventTypes.ServerKick)
            {
                Game.singleton.logger.Write("Server kick" + eve.data, Logger.logType.warm);
                message.title = Game.singleton.multilang.GetWord("ServerKick", Game.singleton.config.lang);
                message.text = Common.ArrayToString(eve.data);
                showOKMessage = true;
            }
            else
            {
                ChatText(Parser.GetEventText(eve, Game.singleton.config.lang, Game.singleton.multilang));
            }
        }

        public void ChatText(string text)
        {
            chatText += (text + Environment.NewLine);
        }
    }
}