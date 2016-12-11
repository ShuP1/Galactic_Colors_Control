using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyCommon;
using MyMonoGame.GUI;
using System.Threading;

namespace Galactic_Colors_Control_GUI.States
{
    public struct Message
    {
        public string title;
        public string text;

        public Message(string Title, string Text = "")
        {
            title = Title;
            text = Text;
        }
    }

    public class ConnectState : State
    {
        private bool locked = false;
        private bool showLoading = false;
        private bool showOKMessage = false;
        private bool showYNMessage = false;

        private Message message;
        private string adress;

        public override void Draw(SpriteBatch spritebatch)
        {
            Game.singleton.background.Draw(spritebatch);
            Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 4), Game.singleton.multilang.GetWord("GCC", Game.singleton.config.lang), Game.singleton.GUI.content.GetFont("title"), new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter);
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
                    if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 140, Game.singleton.ScreenHeight / 4 + 150, 280, 40), Game.singleton.GUI.content.GetBox("Default"), Game.singleton.multilang.GetWord("OK", Game.singleton.config.lang), Game.singleton.GUI.content.GetFont("basic"))) { locked = false; Game.singleton.GUI.ResetFocus(); showOKMessage = false; }
                }
                else
                {
                    if (showYNMessage)
                    {
                        Game.singleton.GUI.Box(new Rectangle(Game.singleton.ScreenWidth / 2 - 150, Game.singleton.ScreenHeight / 4 + 50, 300, 100), Game.singleton.GUI.content.GetBox("Default"));
                        Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 4 + 60), message.title, Game.singleton.GUI.content.GetFont("basic"), null, Manager.textAlign.bottomCenter);
                        if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 140, Game.singleton.ScreenHeight / 4 + 100, 135, 40), Game.singleton.GUI.content.GetBox("Default"), Game.singleton.multilang.GetWord("Yes", Game.singleton.config.lang), Game.singleton.GUI.content.GetFont("basic")))
                        {
                            if (!locked)
                            {
                                locked = true;
                                showYNMessage = false;
                                Game.singleton.GUI.ResetFocus();
                                new Thread(ConnectHost).Start();
                            }
                        }
                        if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 + 5, Game.singleton.ScreenHeight / 4 + 100, 135, 40), Game.singleton.GUI.content.GetBox("Default"), Game.singleton.multilang.GetWord("No", Game.singleton.config.lang), Game.singleton.GUI.content.GetFont("basic")))
                        {
                            showYNMessage = false;
                            Game.singleton.client.ResetHost();
                            Game.singleton.GUI.ResetFocus();
                        }
                    }
                    else
                    {
                        if (Game.singleton.GUI.TextField(new Rectangle(Game.singleton.ScreenWidth / 2 - 75, Game.singleton.ScreenHeight / 2 - 30, 150, 40), ref adress, Game.singleton.GUI.content.GetFont("basic"), new MyMonoGame.Colors(Color.LightGray, Color.White), Manager.textAlign.centerCenter, Game.singleton.multilang.GetWord("EnterHostname", Game.singleton.config.lang)))
                        {
                            if (!locked)
                            {
                                locked = true;
                                new Thread(ValidateHost).Start();
                            }
                        }
                        if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 75, Game.singleton.ScreenHeight / 2 + 20, 150, 40), Game.singleton.GUI.content.GetBox("Default"), Game.singleton.multilang.GetWord("Connect", Game.singleton.config.lang), Game.singleton.GUI.content.GetFont("basic"), new MyMonoGame.Colors(Color.LightGray, Color.White)))
                        {
                            if (!locked)
                            {
                                locked = true;
                                new Thread(ValidateHost).Start();
                            }
                        }
                        if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 75, Game.singleton.ScreenHeight / 2 + 70, 150, 40), Game.singleton.GUI.content.GetBox("Default"), Game.singleton.multilang.GetWord("Back", Game.singleton.config.lang), Game.singleton.GUI.content.GetFont("basic"), new MyMonoGame.Colors(Color.LightGray, Color.White)))
                        {
                            if (!locked)
                            {
                                locked = true;
                                Game.singleton.GUI.ResetFocus();
                                new Thread(() =>
                                {
                                    while (!Utilities.DoubleTo(ref Game.singleton.background.speedX, 1, 0.1)) { Thread.Sleep(20); }
                                    Game.singleton.gameState = new MainMenuState();
                                }).Start();
                            }
                        }
                    }
                }
            }
        }

        public override void Update()
        {
            Game.singleton.background.Update();
        }

        private void ValidateHost()
        {
            showLoading = true;
            if (adress == null) { adress = ""; }
            string Host = Game.singleton.client.ValidateHost(adress);
            if (Host[0] == '*')
            {
                Host = Host.Substring(1);
                message.title = Game.singleton.multilang.GetWord("Error", Game.singleton.config.lang);
                message.text = Host;
                showOKMessage = true;
                Game.singleton.client.ResetHost();
                Game.singleton.logger.Write("Validate : " + Host, Logger.logType.info);
            }
            else
            {
                message.title = Game.singleton.multilang.GetWord("Use", Game.singleton.config.lang) + " " + Host + "?";
                showYNMessage = true;
            }
            showLoading = false;
            locked = false;
        }

        private void ConnectHost()
        {
            showLoading = true;
            if (Game.singleton.client.ConnectHost())
            {
                Game.singleton.logger.Write("Connected", Logger.logType.info);
                Game.singleton.gameState = new IndentificationState();
            }
            else
            {
                Game.singleton.logger.Write("Connect error", Logger.logType.error);
                message.title = Game.singleton.multilang.GetWord("Error", Game.singleton.config.lang);
                message.text = string.Empty;
                showOKMessage = true;
                Game.singleton.client.ResetHost();
            }
            showLoading = false;
        }
    }
}