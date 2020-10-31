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
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Debug;
using OpenBound_Network_Object_Library.Entity;
using System.Collections.Generic;

namespace OpenBound.GameComponents.MobileAction
{
    public class Trace
    {
        private Projectile projectile;

        protected List<Flipbook> traceList;
        protected Flipbook leadTrace;

        protected Vector2 positionOffset, positionRotatedOffset, lastSpawn;
        public Color Color;

        //
        protected float rotationAngle;

#if DEBUG
        protected DebugCrosshair dc0, dc1;
#endif

        public Vector2 Position
        {
            get => leadTrace.Position;
            set => leadTrace.Position = value;
        }


        public Trace(MobileType mobileType, Color color, Projectile projectile)
        {
            this.projectile = projectile;
            traceList = new List<Flipbook>();
            Color = color;

#if DEBUG
            dc0 = new DebugCrosshair(Color.Yellow);
            dc1 = new DebugCrosshair(Color.Blue);

            DebugHandler.Instance.Add(dc0);
            DebugHandler.Instance.Add(dc1);
#endif
        }


        public Flipbook SpawnFlipbook(MobileType mobileType, ShotType shotType, AnimationInstance animationInstance = null)
        {
            if (animationInstance == null)
                animationInstance = new AnimationInstance() { StartingFrame = 0, EndingFrame = 72, TimePerFrame = 1 / 72f, AnimationType = AnimationType.FowardStop };

            switch (mobileType)
            {
                case MobileType.Mage:
                    return new Flipbook(Vector2.Zero, new Vector2(8, 8), 16, 16, $"Graphics/Tank/Mage/{shotType}T", animationInstance, projectile.FlipbookList[0].LayerDepth);
                case MobileType.Turtle:
                    return new Flipbook(Vector2.Zero, new Vector2(20, 8), 32, 16, $"Graphics/Tank/Turtle/S1T", animationInstance, projectile.FlipbookList[0].LayerDepth);
            }

            return null;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            traceList.ForEach((x) => x.Draw(gameTime, spriteBatch));
        }
    }
}
