/* 
 * Copyright (C) 2020, Carlos H.M.S. <carlos_judo@hotmail.com>
 * This file is part of OpenBound.
 * OpenBound is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of the License, or(at your option) any later version.
 * 
 * OpenBound is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with OpenBound. If not, see http://www.gnu.org/licenses/.
 */

using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.TCP.Entity;
using System.Collections.Concurrent;

namespace OpenBound_Network_Object_Library.Extension
{
    public static class EConcurrentQueue
    {
        public static void Enqueue(this ExtendedConcurrentQueue<byte[]> queue, int service, int part, object serviceObject)
        {
            string serviceObjectJSON = ObjectWrapper.Serialize(serviceObject);
            byte[] req = ObjectWrapper.ConvertObjectToByteArray($"{service}|{part}|{serviceObjectJSON}");
            lock (queue) queue.Enqueue(req);
        }

        public static void Enqueue(this ExtendedConcurrentQueue<byte[]> queue, int service, object serviceObject)
        {
            string serviceObjectJSON = ObjectWrapper.Serialize(serviceObject);
            byte[] req = ObjectWrapper.ConvertObjectToByteArray($"{service}|{serviceObjectJSON}");
            lock (queue) queue.Enqueue(req);
        }

        public static T Dequeue<T>(this ExtendedConcurrentQueue<T> queue)
        {
            T tmpValue = default;
            lock (queue) queue.TryDequeue(out tmpValue);
            return tmpValue;
        }

        public static T Peek<T>(this ExtendedConcurrentQueue<T> queue)
        {
            T tmpValue = default;
            lock (queue) queue.TryPeek(out tmpValue);
            return tmpValue;
        }
    }
}
