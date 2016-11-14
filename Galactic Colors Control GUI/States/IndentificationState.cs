using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyMonoGame.GUI;
using System.Threading;
using System;
using Galactic_Colors_Control_Common.Protocol;
using Galactic_Colors_Control_Common;

namespace Galactic_Colors_Control_GUI.States
{
    public class IndentificationState : State
    {
        private string username;
        private Message message;

        private bool locked = false;
        private bool showLoading = false;
        private bool showOKMessage = false;

        public override void Draw(SpriteBatch spritebatch)
        {
            Game.singleton.background.Draw(spritebatch);
            Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 4), "Galactic Colors Control", Game.singleton.fonts.title , new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter);
            if (showLoading)
            {
                Game.singleton.GUI.Box(new Rectangle(Game.singleton.ScreenWidth / 2 - 150, Game.singleton.ScreenHeight / 4 + 50, 300, 50), Game.singleton.buttonsSprites[0]);
                Game.singleton.GUI.Label(new Rectangle(Game.singleton.ScreenWidth / 2 - 150, Game.singleton.ScreenHeight / 4 + 50, 300, 50), "Loading", Game.singleton.fonts.basic);
            }
            else
            {
                if (showOKMessage)
                {
                    Game.singleton.GUI.Box(new Rectangle(Game.singleton.ScreenWidth / 2 - 150, Game.singleton.ScreenHeight / 4 + 50, 300, 150), Game.singleton.buttonsSprites[0]);
                    Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 4 + 60), message.title, Game.singleton.fonts.basic, null, Manager.textAlign.bottomCenter);
                    Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 4 + 100), message.text, Game.singleton.fonts.small, null, Manager.textAlign.bottomCenter);
                    if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 140, Game.singleton.ScreenHeight / 4 + 150, 280, 40), Game.singleton.buttonsSprites[0], "Ok", Game.singleton.fonts.basic)){ Game.singleton.GUI.ResetFocus(); showOKMessage = false; }
                }
                else
                {
                    if (Game.singleton.GUI.TextField(new Rectangle(Game.singleton.ScreenWidth / 2 - 75, Game.singleton.ScreenHeight / 2 - 30, 150, 40), ref username, Game.singleton.fonts.basic, new MyMonoGame.Colors(Color.LightGray, Color.White), Manager.textAlign.centerCenter, "Username")) {
                        if (!locked)
                        {
                            locked = true;
                            new Thread(IdentifiacateHost).Start();
                        }
                    }
                    if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 75, Game.singleton.ScreenHeight / 2 + 20, 150, 40), Game.singleton.buttonsSprites[0], "Validate", Game.singleton.fonts.basic, new MyMonoGame.Colors(Color.LightGray, Color.White))) {
                        if (!locked)
                        {
                            locked = true;
                            new Thread(IdentifiacateHost).Start();
                        }
                    }
                    if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 75, Game.singleton.ScreenHeight / 2 + 70, 150, 40), Game.singleton.buttonsSprites[0], "Back", Game.singleton.fonts.basic, new MyMonoGame.Colors(Color.LightGray, Color.White)))
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

        private void IdentifiacateHost()
        {
            showLoading = true;
            if (username != null)
            {
                if (username.Length > 3)
                {
                    ResultData res = Game.singleton.client.Request(new string[2] { "connect", username });
                    if (res.type == ResultTypes.OK)
                    {
                        Game.singleton.gameState = new GameState(username);
                    }
                    else
                    {
                        message.title = "Error";
                        message.text = Common.ArrayToString(res.result);
                        showOKMessage = true;
                    }
                }
            }
            showLoading = false;
            locked = false;
        }


        public override void Update()
        {
            Game.singleton.background.Update();
        }
    }
}