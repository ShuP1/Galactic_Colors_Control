using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Galactic_Colors_Control_GUI
{
    /// <summary>
    /// Multilayer Background
    /// </summary>
    public class Background
    {
        private double[] backgroundX;
        private double[] backgroundY;
        private Texture2D[] backSprites;
        private double[] ratio;

        public double speedX = 0;
        public double speedY = 0;

        internal void Draw(object spriteBatch)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Background.lenght == Ratio.Lenght
        /// </summary>
        public Background(Texture2D[] BackSprites, double[] Ratio)
        {
            backSprites = BackSprites;
            ratio = Ratio;
            backgroundX = new double[backSprites.Length];
            backgroundY = new double[backSprites.Length];
        }

        /// <summary>
        /// Manual Move
        /// </summary>
        public void Move(double x, double y)
        {
            for (int index = 0; index < backSprites.Length; index++)
            {
                backgroundX[index] += (x * ratio[index]);
                backgroundY[index] += (y * ratio[index]);
                if (backgroundX[index] > backSprites[index].Width) { backgroundX[index] = 0; }
                if (backgroundY[index] > backSprites[index].Height) { backgroundY[index] = 0; }
                if (backgroundX[index] < 0) { backgroundX[index] = backSprites[index].Width; }
                if (backgroundY[index] < 0) { backgroundY[index] = backSprites[index].Height; }
            }
        }

        /// <summary>
        /// AutoMove for speedX and speedY
        /// </summary>
        public void Update()
        {
            Move(speedX, speedY);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < backSprites.Length; index++)
            {
                for (int X = -1; X < Game.singleton.ScreenWidth / backSprites[index].Width + 1; X++)
                {
                    for (int Y = -1; Y < Game.singleton.ScreenHeight / backSprites[index].Height + 1; Y++)
                    {
                        spriteBatch.Draw(backSprites[index], new Rectangle(X * backSprites[index].Width + (int)backgroundX[index], Y * backSprites[index].Height + (int)backgroundY[index], backSprites[index].Width, backSprites[index].Height), Color.White);
                    }
                }
            }
        }
    }
}