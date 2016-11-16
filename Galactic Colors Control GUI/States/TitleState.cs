using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyMonoGame.GUI;
using System;

namespace Galactic_Colors_Control_GUI.States
{
    /// <summary>
    /// Only title in screen (and state change)
    /// </summary>
    public class TitleState : State
    {
        private DateTime _changeDate;
        private State _target;

        public TitleState()
        {
            _target = null;
        }

        public TitleState(State target, TimeSpan time)
        {
            _target = target;
            _changeDate = DateTime.Now.Add(time);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            Game.singleton.background.Draw(spritebatch);
            Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 2), Game.singleton.multilang.Get("GCC", Game.singleton.config.lang), Game.singleton.fonts.title, new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter);
        }

        public override void Update()
        {
            if (_target != null)
            {
                if (DateTime.Now > _changeDate) { Game.singleton.gameState = _target; }
            }
        }
    }
}
