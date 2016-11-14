using Microsoft.Xna.Framework.Graphics;

namespace Galactic_Colors_Control_GUI.States
{
    public class OptionsState : State
    {
        public override void Draw(SpriteBatch spritebatch)
        {
            Game.singleton.background.Draw(spritebatch);
        }

        public override void Update()
        {
            Game.singleton.background.Update();
        }
    }
}