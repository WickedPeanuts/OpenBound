using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBound_Network_Object_Library.Extension
{
    public static class EBitwiseOperators
    {
        public static uint RotateLeft(this uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }

        public static uint RotateRight(this uint value, int count)
        {
            return (value >> count) | (value << (32 - count));
        }
    }
}
