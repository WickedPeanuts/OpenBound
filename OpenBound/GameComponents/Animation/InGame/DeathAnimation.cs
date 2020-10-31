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
using OpenBound.Common;
using OpenBound.GameComponents.Level;
using OpenBound.GameComponents.Pawn;
using OpenBound.GameComponents.Pawn.Unit;
using OpenBound_Network_Object_Library.Entity;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Animation.InGame
{
    public class DeathAnimation
    {
        private static List<DeathAnimation> deathAnimationList;
        private static List<DeathAnimation> toBeDestroyedDeathAnimationList;

        private readonly Mobile mobile;

        private DeathAnimation(Mobile mobile)
        {
            this.mobile = new Random(mobile.Owner, mobile.Position);
            this.mobile.HideLobbyExclusiveAvatars();
        }

        public static void Initialize()
        {
            deathAnimationList = new List<DeathAnimation>();
            toBeDestroyedDeathAnimationList = new List<DeathAnimation>();
        }

        private void UpdateElement(GameTime gameTime)
        {
            mobile.Rider.Update();
            mobile.MobileFlipbook.Position += Parameter.AnimationInGameDeathAnimationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Topography.IsNotInsideMapBoundaries(mobile.MobileFlipbook.Position - new Vector2(0, 300)) && Topography.IsNotInsideMapBoundaries(mobile.MobileFlipbook.Position + new Vector2(0, 300)))
                toBeDestroyedDeathAnimationList.Add(this);
        }

        private void DrawElement(GameTime gameTime, SpriteBatch spriteBatch)
        {
            mobile.Draw(gameTime, spriteBatch);
        }

        public static void Add(Mobile mobile)
        {
            deathAnimationList.Add(new DeathAnimation(mobile));
        }

        public static void Update(GameTime gameTime)
        {
            toBeDestroyedDeathAnimationList.ForEach((x) => deathAnimationList.Remove(x));
            toBeDestroyedDeathAnimationList.Clear();

            deathAnimationList.ForEach((x) =>
            {
                x.UpdateElement(gameTime);
            });
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            deathAnimationList.ForEach((x) => x.DrawElement(gameTime, spriteBatch));
        }
    }
}
