using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Openbound_Asset_Tools.Extension
{
    public static class EQueue
    {
        public static int DequeueInt32(this Queue<byte> source)
        {
            int a = 0;

            a = a | source.Dequeue() << (8 * 0);
            a = a | source.Dequeue() << (8 * 1);

            a = a | source.Dequeue() << (8 * 2);
            a = a | source.Dequeue() << (8 * 3);

            return a;
        }

        public static int DequeueInt16(this Queue<byte> source)
        {
            int a = 0;

            a = a | source.Dequeue() << (8 * 0);
            a = a | source.Dequeue() << (8 * 1);

            return (short)a;
        }
    }
}
