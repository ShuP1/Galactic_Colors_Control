using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Galactic_Colors_Control_GUI.GUI
{
    class Element
    {
        protected Rectangle _pos;
        protected bool _isHover;
        protected bool _isFocus;

        public bool Contain(int x, int y)
        {
            return _pos.Contains(x, y);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        public void Update(int x, int y, Mouse mouse, EventArgs e)
        {
            if (mouse.leftPress)
            {
                if (Contain(x, y))
                {
                    _isFocus = true;
                    Click(this, e);
                }
                else { _isFocus = false; }
            }
            else { _isHover = Contain(x, y); }
        }

        public virtual void Click(object sender, EventArgs e)
        {
            
        }
    }
}
