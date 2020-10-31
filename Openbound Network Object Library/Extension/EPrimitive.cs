using System;
using System.Collections.Generic;
using System.Text;

namespace OpenBound_Network_Object_Library.Extension
{
    public static class EPrimitive
    {
        public static int Normalize(this int value, int min, int max, int newMin, int newMax)
        {
            return (int)((newMax - newMin) / (float)(max - min) * (value - max) + max);
        }

        public static uint Normalize(this uint value, int min, int max, int newMin, int newMax)
        {
            return (uint)((newMax - newMin) / (float)(max - min) * ((int)value - max) + max);
        }
    }
}
