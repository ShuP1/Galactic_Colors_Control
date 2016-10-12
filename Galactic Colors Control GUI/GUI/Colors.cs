using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galactic_Colors_Control_GUI.GUI
{
    public class Colors
    {
        public Color _normal;
        public Color _hover;
        public Color _focus;

        public Colors(Color color) {
            _normal = color;
            _hover = color;
            _focus = color;
        }

        public Colors(Color normal, Color hover)
        {
            _normal = normal;
            _hover = hover;
            _focus = hover;
        }

        public Colors(Color normal, Color hover, Color focus)
        {
            _normal = normal;
            _hover = hover;
            _focus = focus;
        }
    }
}
