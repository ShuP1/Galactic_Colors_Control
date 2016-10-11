using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Galactic_Colors_Control_GUI
{
    static class Utilities
    {
        static public void SpriteFromPng(string path, ref Texture2D sprite, GraphicsDevice graphics )
        {
            if (File.Exists(path))
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    sprite = Texture2D.FromStream(graphics, fileStream);
                }
            }
        }

        static public void SoundFromMp3(string path, ref SoundEffect sound)
        {
            if (File.Exists(path))
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                    sound = SoundEffect.FromStream(fileStream);
                }
            }
        }
    }
}
