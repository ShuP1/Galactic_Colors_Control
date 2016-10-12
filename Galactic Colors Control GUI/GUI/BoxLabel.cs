﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galactic_Colors_Control_GUI.GUI
{
    class BoxLabel : Box
    {
        protected Label _label;

        public BoxLabel() { }

        public BoxLabel(Rectangle pos, boxSprites backSprites, Colors colors, string text, SpriteFont font, Colors textColors, Label.textAlign align = Label.textAlign.centerCenter)
        {
            _pos = pos;
            _backSprites = backSprites;
            _colors = colors;
            _label = new Label(pos, text, font, textColors, align);
        }

        public override void Update(int x, int y, Mouse mouse, Keys key, bool isMaj, EventArgs e)
        {
            base.Update(x, y, mouse, key, isMaj, e);
            _label.Update(x, y, mouse, key, isMaj, e);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            _label.Draw(spriteBatch);
        }
    }
}
