﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Galactic_Colors_Control_GUI.GUI
{
    class Button : Element
    {
        protected event EventHandler _click;
        protected int _unFocusTime = 0;

        public Button() { }

        public Button(Rectangle pos, EventHandler click = null)
        {
            _pos = pos;
            _click = click;
        }

        public override void Click(object sender, EventArgs e)
        {
            if (_click != null)
            {
                _click.Invoke(sender, e);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_isFocus) {
                if (_unFocusTime < 10)
                {
                    _unFocusTime++;
                }
                else {
                    _isFocus = false;
                    _unFocusTime = 0;
                }
            }
        }
    }
}
