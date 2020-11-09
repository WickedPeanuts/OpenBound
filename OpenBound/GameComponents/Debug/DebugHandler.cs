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


using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Debug
{
#if DEBUG
    public class DebugHandler
    {
        private List<DebugElement> debugElementList;
        private List<DebugElement> toBeRemovedElements;
        private List<DebugElement> toBeAddedElements;

        private static DebugHandler instance;
        public static DebugHandler Instance
        {
            get
            {
                if (instance == null)
                    instance = new DebugHandler();

                return instance;
            }
        }

        private DebugHandler()
        {
            debugElementList = new List<DebugElement>();
            toBeRemovedElements = new List<DebugElement>();
            toBeAddedElements = new List<DebugElement>();
        }

        public void Clear()
        {
            lock (debugElementList)
            {
                debugElementList.ForEach((x) => toBeRemovedElements.Add(x));
            }
        }

        /*
        public void Add(DebugElement de) { }
        public void AddRange(List<DebugElement> deList) { }
        public void AddRange(List<DebugCrosshair> deList) { }
        public void AddRange(DebugCrosshair[] deList) { }*/

        public void Add(DebugElement de)
        {
            lock (debugElementList)
                toBeAddedElements.Add(de);
        }
        public void AddRange(List<DebugElement> deList)
        {
            foreach (DebugElement de in deList)
                lock (debugElementList)
                    toBeAddedElements.Add(de);
        }
        public void AddRange(List<DebugCrosshair> deList)
        {
            foreach (DebugElement de in deList)
                lock (debugElementList)
                    toBeAddedElements.Add(de);
        }
        public void AddRange(DebugCrosshair[] deList)
        {
            foreach (DebugElement de in deList)
                lock (debugElementList)
                    toBeAddedElements.Add(de);
        }

        public void Update()
        {
            lock (debugElementList)
            {
                toBeAddedElements.ForEach((x) => debugElementList.Add(x));
                toBeRemovedElements.ForEach((x) => debugElementList.Remove(x));

                toBeAddedElements.Clear();
                toBeRemovedElements.Clear();
            }
        }

        public void Draw(SpriteBatch SpriteBatch)
        {
            lock (debugElementList) debugElementList.ForEach((x) => x.Draw(SpriteBatch));
        }
    }
#endif
}
