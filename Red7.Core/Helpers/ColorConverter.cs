using System;
using System.Collections.Generic;
using System.Text;

namespace Red7.Core.Helpers
{
    public static class ColorConverter
    {
        public static System.Drawing.Color GetConsoleColor(this Red7.Core.Enums.Color color)
        {
            switch (color)
            {
                case Enums.Color.Red:
                    return System.Drawing.Color.Red;
                case Enums.Color.Orange:
                    return System.Drawing.Color.Orange;
                case Enums.Color.Yellow:
                    return System.Drawing.Color.Yellow;
                case Enums.Color.Green:
                    return System.Drawing.Color.Green;
                case Enums.Color.Blue:
                    return System.Drawing.Color.LightBlue;
                case Enums.Color.Indigo:
                    return System.Drawing.Color.Blue;
                case Enums.Color.Violet:
                    return System.Drawing.Color.Purple;
                default:
                    throw new Exception("Invalid color to convert.");
            }
        }
    }
}
