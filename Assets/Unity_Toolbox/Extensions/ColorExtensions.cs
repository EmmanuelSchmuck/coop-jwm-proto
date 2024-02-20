using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color c, float value)
        {
            return new Color(c.r, c.g, c.b, value);
        }

        public static Color WithAlphaMultiplied(this Color c, float value)
        {
            return new Color(c.r, c.g, c.b, c.a * value);
        }
    }
}