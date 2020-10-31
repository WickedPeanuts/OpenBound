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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenBound.Extension;
using System;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Animation
{
    public class SpecialEffectHandler
    {
        private static HashSet<SpecialEffect> specialEffectSet;
        private static HashSet<SpecialEffect> toBeAddedSpecialEffect;
        private static HashSet<SpecialEffect> specialEffectToBeDestroyed;

        public static void Initialize()
        {
            toBeAddedSpecialEffect = new HashSet<SpecialEffect>();
            specialEffectSet = new HashSet<SpecialEffect>();
            specialEffectToBeDestroyed = new HashSet<SpecialEffect>();
        }

        public static void AddRange(List<SpecialEffect> specialEffectList)
        {
            lock (toBeAddedSpecialEffect)
            {
                toBeAddedSpecialEffect.AddRange(specialEffectList);
            }
        }

        public static void Add(SpecialEffect specialEffect)
        {
            lock (toBeAddedSpecialEffect)
            {
                toBeAddedSpecialEffect.Add(specialEffect);
            }
        }

        public static void Remove(SpecialEffect specialEffect)
        {
            specialEffectToBeDestroyed.Add(specialEffect);
        }

        public static void Remove(List<SpecialEffect> specialEffectList)
        {
            specialEffectToBeDestroyed.AddRange(specialEffectList);
        }

        public static void Update(GameTime gameTime)
        {
            lock (toBeAddedSpecialEffect)
            {
                specialEffectSet.AddRange(toBeAddedSpecialEffect);
                toBeAddedSpecialEffect.Clear();
            }

            specialEffectToBeDestroyed.ForEach((x) => specialEffectSet.Remove(x));
            specialEffectToBeDestroyed.Clear();

            specialEffectSet.ForEach((x) => x.Update(gameTime));
        }

        public static void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            specialEffectSet.ForEach((x) => x.Flipbook.Draw(GameTime, SpriteBatch));
        }
    }
}
