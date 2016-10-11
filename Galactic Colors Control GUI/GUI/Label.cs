using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galactic_Colors_Control_GUI.GUI
{
    class Label : Element
    {
        protected string _text;
        protected SpriteFont _font;
        protected Color _color, _colorHover, _colorFocus;
        protected Vector _vector;
        protected bool _center;

        public Label() { }

        public Label(Vector vector, string text, SpriteFont font, Color color, bool center = false)
        {
            _vector = vector;
            _text = text;
            _font = font;
            _color = color;
            _colorHover = color;
            _colorFocus = color;
            _center = center;
            OnTextChange(text);
        }

        public Label(Vector vector, string text, SpriteFont font, Color color, Color colorHover, bool center = false)
        {
            _vector = vector;
            _text = text;
            _font = font;
            _color = color;
            _colorHover = colorHover;
            _colorFocus = colorHover;
            _center = center;
            OnTextChange(text);
        }

        public Label(Vector vector, string text, SpriteFont font, Color color, Color colorHover, Color colorFocus, bool center = false)
        {
            _vector = vector;
            _text = text;
            _font = font;
            _color = color;
            _colorHover = colorHover;
            _colorFocus = colorFocus;
            _center = center;
            OnTextChange(text);
        }

        protected void OnTextChange(string text)
        {
            if (_center)
            {
                _pos = new Rectangle(_vector.X - (int)_font.MeasureString(text).X / 2, _vector.Y - (int)_font.MeasureString(text).Y / 2, (int)_font.MeasureString(text).X, (int)_font.MeasureString(text).Y);
            }
            else
            {
                _pos = new Rectangle(_vector.X, _vector.Y, (int)_font.MeasureString(text).X, (int)_font.MeasureString(text).Y);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color color = _isFocus ? _colorFocus : (_isHover ? _colorHover : _color);
            spriteBatch.DrawString(_font, _text, new Vector2(_pos.X, _pos.Y), color);
        }
    }
}
