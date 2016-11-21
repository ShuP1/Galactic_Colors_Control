using Galactic_Colors_Control_Common;
using Galactic_Colors_Control_Common.Protocol;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyMonoGame.GUI;
using System.Threading;

namespace Galactic_Colors_Control_GUI.States
{
    public class PartyCreateState : State
    {
        private Message message;

        private bool locked = false;
        private bool showLoading = false;
        private bool showOKMessage = false;
        private string name;
        private string size;

        public override void Draw(SpriteBatch spritebatch)
        {
            Game.singleton.background.Draw(spritebatch);
            Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 4), Game.singleton.multilang.Get("GCC", Game.singleton.config.lang), Game.singleton.fonts.title, new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter);
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
                    if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 140, Game.singleton.ScreenHeight / 4 + 150, 280, 40), Game.singleton.buttonsSprites[0], Game.singleton.multilang.Get("OK", Game.singleton.config.lang), Game.singleton.fonts.basic)) { Game.singleton.GUI.ResetFocus(); showOKMessage = false; }
                }
                else
                {
                    Game.singleton.GUI.Box(new Rectangle(Game.singleton.ScreenWidth / 2 - 60, Game.singleton.ScreenHeight / 2 - 100, 120, 210), Game.singleton.buttonsSprites[0]);
                    Game.singleton.GUI.TextField(new Rectangle(Game.singleton.ScreenWidth / 2 - 50, Game.singleton.ScreenHeight / 2 - 90, 100, 40), ref name, Game.singleton.fonts.basic, null, Manager.textAlign.centerCenter, Game.singleton.multilang.Get("Name", Game.singleton.config.lang));
                    Game.singleton.GUI.TextField(new Rectangle(Game.singleton.ScreenWidth / 2 - 50, Game.singleton.ScreenHeight / 2 - 40, 100, 40), ref size, Game.singleton.fonts.basic, null, Manager.textAlign.centerCenter, Game.singleton.multilang.Get("Size", Game.singleton.config.lang));
                    if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 50, Game.singleton.ScreenHeight / 2 + 10, 100, 40), Game.singleton.buttonsSprites[0], Game.singleton.multilang.Get("Create", Game.singleton.config.lang), Game.singleton.fonts.basic, new MyMonoGame.Colors(Color.LightGray, Color.White)))
                    {
                        if (!locked)
                        {
                            locked = true;
                            new Thread(CreateParty).Start();
                        }
                    }

                    if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 50, Game.singleton.ScreenHeight / 2 + 60, 100, 40), Game.singleton.buttonsSprites[0], Game.singleton.multilang.Get("Back", Game.singleton.config.lang), Game.singleton.fonts.basic, new MyMonoGame.Colors(Color.LightGray, Color.White)))
                    {
                        if (!locked)
                        {
                            locked = true;
                            Game.singleton.GUI.ResetFocus();
                            Game.singleton.client.ExitHost();
                            Game.singleton.gameState = new PartyState();
                        }
                    }
                }
            }
        }

        private void CreateParty()
        {
            showLoading = true;
            if (name != null)
            {
                int count;
                string party = name;
                name = null;
                if (int.TryParse(size, out count))
                {
                    size = null;
                    ResultData res = Game.singleton.client.Request(new string[4] { "party", "create", party, count.ToString() });
                    if (res.type == ResultTypes.OK)
                    {
                        Game.singleton.logger.Write("Create party " + Common.ArrayToString(res.result), Logger.logType.info);
                        Game.singleton.gameState = new GameState();
                    }
                    else
                    {
                        Game.singleton.logger.Write("Create error " + Common.ArrayToString(res.result), Logger.logType.error);
                        message.title = Game.singleton.multilang.Get("Error", Game.singleton.config.lang);
                        message.text = Common.ArrayToString(res.result);
                        showOKMessage = true;
                    }
                }
            }
            locked = false;
            showLoading = false;
        }

        public override void Update()
        {
            Game.singleton.background.Update();
        }
    }
}