using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openbound_Asset_Tools.Entity
{
    public class ColorBlending
    {
        /// <summary>
        /// Blending function with two distinct pixels on a single color channel
        /// </summary>
        /// <param name="colorA"></param>
        /// <param name="alphaA"></param>
        /// <param name="colorB"></param>
        /// <param name="alphaB"></param>
        /// <returns></returns>
        private static float SingleChannelAlphaBlending(float colorA, float alphaA, float colorB, float alphaB)
        {
            alphaA /= 255;
            alphaB /= 255;
            return (colorA * alphaA + colorB * alphaB * (1 - alphaA)) / (alphaA + alphaB * (1 - alphaA));
        }

        /// <summary>
        /// Returns a pixel that is the result of the blending between the given colors
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Color MultiChannelAlphaBlending(Color x, Color y)
        {
            return Color.FromArgb(Math.Max(x.A, y.A),
                (byte)SingleChannelAlphaBlending(x.R, x.A, y.R, y.A),
                (byte)SingleChannelAlphaBlending(x.G, x.A, y.G, y.A),
                (byte)SingleChannelAlphaBlending(x.B, x.A, y.B, y.A));
        }
    }
}
