using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Galactic_Colors_Control_GUI.GUI
{
    class Button : Element
    {
        protected string _text;
        protected SpriteFont _font;
        protected Color _backColor, _backColorHover, _backColorFocus, _textColor, _textColorHover, _textColorFocus;
        protected event EventHandler _click;
        Texture2D _backSprite;

        public Button() { }

        public Button(Rectangle pos, Color backColor, EventHandler click = null)
        {
            _pos = pos;
            _backColor = backColor;
            _backColorHover = backColor;
            _backColorFocus = backColor;
            _backSprite = Game1.nullSprite;
            _click = click;
        }

        public Button(Rectangle pos, Color backColor, Color backColorHover, EventHandler click = null)
        {
            _pos = pos;
            _backColor = backColor;
            _backColorHover = backColorHover;
            _backColorFocus = backColorHover;
            _backSprite = Game1.nullSprite;
            _click = click;
        }

        public Button(Rectangle pos, Color backColor, Color backColorHover, Color backColorFocus, EventHandler click = null)
        {
            _pos = pos;
            _backColor = backColor;
            _backColorHover = backColorHover;
            _backColorFocus = backColorFocus;
            _backSprite = Game1.nullSprite;
            _click = click;
        }

        public Button(Rectangle pos, Color backColor, string text, SpriteFont font, Color textColor, EventHandler click = null)
        {
            _pos = pos;
            _font = font;
            _backColor = backColor;
            _backColorHover = backColor;
            _backColorFocus = backColor;
            _textColor = textColor;
            _textColorHover = textColor;
            _textColorFocus = textColor;
            _text = text;
            _backSprite = Game1.nullSprite;
            _click = click;
        }

        public Button(Rectangle pos, Color backColor, Color backColorHover, string text, SpriteFont font, Color textColor, Color textColorHover, EventHandler click = null)
        {
            _pos = pos;
            _font = font;
            _backColor = backColor;
            _backColorHover = backColorHover;
            _backColorFocus = backColorHover;
            _textColor = textColor;
            _textColorHover = textColorHover;
            _textColorFocus = textColorHover;
            _text = text;
            _backSprite = Game1.nullSprite;
            _click = click;
        }

        public Button(Rectangle pos, Color backColor, Color backColorHover, Color backColorFocus, string text, SpriteFont font, Color textColor, Color textColorHover, Color textColorFocus, EventHandler click = null)
        {
            _pos = pos;
            _backColor = backColor;
            _backColorHover = backColorHover;
            _backColorFocus = backColorFocus;
            _text = text;
            _font = font;
            _textColor = textColor;
            _textColorHover = textColorHover;
            _textColorFocus = textColorFocus;
            _backSprite = Game1.nullSprite;
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
            Color backColor = _isFocus ? _backColorFocus : (_isHover ? _backColorHover : _backColor);
            spriteBatch.Draw(_backSprite, _pos, backColor);
            if (_text != null)
            {
                Color textColor = _isFocus ? _textColorFocus : (_isHover ? _textColorHover : _textColor);
                spriteBatch.DrawString(_font, _text, new Vector2(_pos.X + (_pos.Width - _font.MeasureString(_text).X) / 2, _pos.Y + (_pos.Height - _font.MeasureString(_text).Y) / 2), textColor);
            }
            if (_isFocus) { _isFocus = false; }
        }
    }
}
