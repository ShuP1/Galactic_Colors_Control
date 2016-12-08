using System;

namespace Galactic_Colors_Control_GUI
{
    internal static class Utilities
    {
        public static bool DoubleTo(ref double value, double target, double speed)
        {
            speed = Math.Abs(speed);
            bool up = value < target;
            value += (up ? 1 : -1) * speed;
            if ((up && value >= target) || (!up && value <= target))
            {
                value = target;
                return true;
            }
            return false;
        }
    }
}