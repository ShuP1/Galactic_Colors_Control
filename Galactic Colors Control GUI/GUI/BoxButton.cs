﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Galactic_Colors_Control_GUI.GUI
{
    class BoxButton : Box
    {
        protected Button _button;
        private int _unFocusTime;

        public BoxButton() { }

        public BoxButton(Rectangle pos, boxSprites backSprites, Colors colors, EventHandler click = null)
        {
            _pos = pos;
            _backSprites = backSprites;
            _colors = colors;
            _button = new Button(pos, click);
        }

        public override void Update(int x, int y, Mouse mouse, Keys key, bool isMaj, EventArgs e)
        {
            base.Update(x, y, mouse, key, isMaj, e);
            _button.Update(x, y, mouse, key, isMaj, e);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (_isFocus)
            {
                if (_unFocusTime < 10)
                {
                    _unFocusTime++;
                }
                else {
                    _isFocus = false;
                    _unFocusTime = 0;
                }
            }

            _button.Draw(spriteBatch);
        }
    }
}
