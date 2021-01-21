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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace OpenBound_Network_Object_Library.Extension
{
    public static class ECollections
    {
        public static void SafeAdd<T>(this List<T> List, T Value)
        {
            lock (List)
            {
                List.Add(Value);
            }
        }

        public static T SafeRemoveAt<T>(this List<byte[]> List, int Index)
        {
            T variable;

            lock (List)
            {
                for (; List.Count <= Index;) Thread.Sleep(100);
                variable = ObjectWrapper.ConvertByteArrayToObject<T>(List[Index]);
                List.RemoveAt(Index);
            }

            return variable;
        }

        public static bool SafeRemove<T>(this List<T> List, T Value)
        {
            lock (List)
            {
                return List.Remove(Value);
            }
        }

        public static T SafeRemoveAt<T>(this List<T> List, int Index)
        {
            T variable;

            lock (List)
            {
                variable = List[Index];
                List.RemoveAt(Index);
            }

            return variable;
        }

        public static T SafeGet<T>(this List<T> List, int Index)
        {
            lock (List)
            {
                return List[Index];
            }
        }

        public static T SafeGet<T>(this List<byte[]> List, int Index)
        {
            T variable;

            lock (List)
            {
                variable = ObjectWrapper.ConvertByteArrayToObject<T>(List[Index]);
            }

            return variable;
        }

        public static T SafeDequeue<T>(this ConcurrentQueue<T> Queue)
        {
            T tmpValue;
            for (; !Queue.TryDequeue(out tmpValue);) { Thread.Sleep(100); }
            return tmpValue;
        }

        public static T SafeDequeue<T>(this ConcurrentQueue<byte[]> Queue)
        {
            byte[] tmpValue;
            for (; !Queue.TryDequeue(out tmpValue);) { Thread.Sleep(100); }
            return ObjectWrapper.ConvertByteArrayToObject<T>(tmpValue);
        }

        public static T SafePeek<T>(this ConcurrentQueue<T> Queue)
        {
            T tmpValue;
            for (; !Queue.TryPeek(out tmpValue);) { Thread.Sleep(100); }
            return tmpValue;
        }

        public static T SafePeek<T>(this ConcurrentQueue<byte[]> Queue)
        {
            byte[] tmpValue;
            for (; !Queue.TryPeek(out tmpValue);) { Thread.Sleep(100); }
            return ObjectWrapper.ConvertByteArrayToObject<T>(tmpValue);
        }

        public static void AddOrReplace<K, V>(this Dictionary<K, V> source, K key, V value)
        {
            if (source.ContainsKey(key))
                source.Remove(key);
            source.Add(key, value);
        }

        public static void ForEachValues<K, V>(this Dictionary<K, V> source, Action<V> action)
        {
            foreach (KeyValuePair<K, V> kvp in source)
            {
                action(kvp.Value);
            }
        }

        public static void ForEachKeys<K, V>(this Dictionary<K, V> source, Action<K> action)
        {
            foreach (KeyValuePair<K, V> kvp in source)
            {
                action(kvp.Key);
            }
        }

        public static void ForEach<K, V>(this Dictionary<K, V> source, Action<KeyValuePair<K, V>> action)
        {
            foreach (KeyValuePair<K, V> kvp in source)
            {
                action(kvp);
            }
        }
    }
}
