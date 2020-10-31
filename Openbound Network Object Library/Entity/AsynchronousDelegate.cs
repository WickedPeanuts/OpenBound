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

using System;

namespace OpenBound_Network_Object_Library.Entity
{
    public class AsynchronousAction
    {
        #region Async Variable 
        private readonly object asyncLock;

        private Action value;
        public Action Value
        {
            get
            {
                lock (asyncLock)
                    return value;
            }
            set
            {
                lock (asyncLock)
                {
                    this.value = value;
                }
            }
        }
        #endregion

        #region Operators
        public static AsynchronousAction operator +(AsynchronousAction a, AsynchronousAction b)
        {
            lock (a.asyncLock)
                a.value += b.Value;

            return a;
        }

        public static AsynchronousAction operator +(AsynchronousAction a, Action b)
        {
            lock (a.asyncLock)
                a.value += b;

            return a;
        }

        public static implicit operator AsynchronousAction(Action a)
        {
            return new AsynchronousAction(a);
        }
        #endregion

        public AsynchronousAction(Action action = default)
        {
            asyncLock = new object();
            value = action;
        }

        public void AsynchronousInvoke()
        {
            lock (asyncLock)
                value?.Invoke();
        }

        public void AsynchronousInvokeAndDestroy()
        {
            lock (asyncLock)
            {
                value?.Invoke();
                value = default;
            }
        }
    }
}
