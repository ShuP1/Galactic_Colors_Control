using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Galactic_Colors_Control_GUI.GUI
{
    class TexturedButton : Button
    {
        buttonSprites _backSprites;

        public TexturedButton(Rectangle pos, buttonSprites backSprites, Color backColor, EventHandler click = null)
        {
            _pos = pos;
            _backSprites = backSprites;
            _backColor = backColor;
            _backColorHover = backColor;
            _backColorFocus = backColor;
            _click += click;
        }

        public TexturedButton(Rectangle pos, buttonSprites backSprites, Color backColor, Color backColorHover, EventHandler click = null)
        {
            _pos = pos;
            _backSprites = backSprites;
            _backColor = backColor;
            _backColorHover = backColorHover;
            _backColorFocus = backColorHover;
            _click += click;
        }

        public TexturedButton(Rectangle pos, buttonSprites backSprites, Color backColor, Color backColorHover, Color backColorFocus, EventHandler click = null)
        {
            _pos = pos;
            _backSprites = backSprites;
            _backColor = backColor;
            _backColorHover = backColorHover;
            _backColorFocus = backColorFocus;
            _click += click;
        }

        public TexturedButton(Rectangle pos, buttonSprites backSprites, Color backColor, string text, SpriteFont font, Color textColor, EventHandler click = null)
        {
            _pos = pos;
            _font = font;
            _backSprites = backSprites;
            _backColor = backColor;
            _textColor = textColor;
            _textColorHover = textColor;
            _textColorFocus = textColor;
            _text = text;
            _click += click;
        }

        public TexturedButton(Rectangle pos, buttonSprites backSprites, Color backColor, Color backColorHover, string text, SpriteFont font, Color textColor, Color textColorHover, EventHandler click = null)
        {
            _pos = pos;
            _font = font;
            _backSprites = backSprites;
            _backColor = backColor;
            _backColorHover = backColorHover;
            _backColorFocus = backColorHover;
            _textColor = textColor;
            _textColorHover = textColorHover;
            _textColorFocus = textColorHover;
            _text = text;
            _click += click;
        }

        public TexturedButton(Rectangle pos, buttonSprites backSprites, Color backColor, Color backColorHover, Color backColorFocus, string text, SpriteFont font, Color textColor, Color textColorHover, Color textColorFocus, EventHandler click = null)
        {
            _pos = pos;
            _backSprites = backSprites;
            _backColor = backColor;
            _backColorHover = backColorHover;
            _backColorFocus = backColorFocus;
            _text = text;
            _font = font;
            _textColor = textColor;
            _textColorHover = textColorHover;
            _textColorFocus = textColorFocus;
            _click += click;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color backColor = _isFocus ? _backColorFocus : (_isHover ? _backColorHover : _backColor);

            int leftWidth = _backSprites.topLeft.Width;
            int rightWidth = _backSprites.topRight.Width;
            int centerWidth = _pos.Width - leftWidth - rightWidth;

            int topHeight = _backSprites.topLeft.Height;
            int bottomHeight = _backSprites.bottomLeft.Height;
            int centerHeight = _pos.Height - topHeight - bottomHeight;

            spriteBatch.Draw(_backSprites.topLeft, new Rectangle(_pos.X, _pos.Y, leftWidth, topHeight), backColor);
            spriteBatch.Draw(_backSprites.topCenter, new Rectangle(_pos.X + leftWidth, _pos.Y, centerWidth, topHeight), backColor);
            spriteBatch.Draw(_backSprites.topRight, new Rectangle(_pos.X + _pos.Width - rightWidth, _pos.Y, rightWidth, topHeight), backColor);
            spriteBatch.Draw(_backSprites.centerLeft, new Rectangle(_pos.X, _pos.Y + topHeight, leftWidth, centerHeight), backColor);
            spriteBatch.Draw(_backSprites.centerCenter, new Rectangle(_pos.X + leftWidth, _pos.Y + topHeight, centerWidth, centerHeight), backColor);
            spriteBatch.Draw(_backSprites.centerRight, new Rectangle(_pos.X + _pos.Width - rightWidth, _pos.Y + topHeight, rightWidth, centerHeight), backColor);
            spriteBatch.Draw(_backSprites.bottomLeft, new Rectangle(_pos.X, _pos.Y + _pos.Height - bottomHeight, leftWidth, bottomHeight), backColor);
            spriteBatch.Draw(_backSprites.bottomCenter, new Rectangle(_pos.X + leftWidth, _pos.Y + _pos.Height - bottomHeight, centerWidth, bottomHeight), backColor);
            spriteBatch.Draw(_backSprites.bottomRight, new Rectangle(_pos.X + _pos.Width - rightWidth, _pos.Y + _pos.Height - bottomHeight, rightWidth, bottomHeight), backColor);
            if (_text != null)
            {
                Color textColor = _isFocus ? _textColorFocus : (_isHover ? _textColorHover : _textColor);
                spriteBatch.DrawString(_font, _text, new Vector2(_pos.X + (_pos.Width - _font.MeasureString(_text).X) / 2, _pos.Y + (_pos.Height - _font.MeasureString(_text).Y) / 2), textColor);
            }
            if (_isFocus) { _isFocus = false; }
        }
    }
}
