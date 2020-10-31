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
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Level;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.MobileAction;
using OpenBound_Network_Object_Library.Entity;
using System;
using System.Collections.Generic;

namespace OpenBound.GameComponents.WeatherEffect
{
    public class Random : Weather
    {
        bool isActive;
        WeatherType extraWeatherType;

        public Random(Vector2 position, WeatherType extraWeatherType) : base(new Vector2(position.X, -Topography.MapHeight / 2), new Vector2(64, 32), 8, new Vector2(40, 0), default, WeatherType.Random, 1)
        {
            isActive = false;
            this.extraWeatherType = extraWeatherType;
            
            Initialize("Graphics/Special Effects/Weather/Random", StartingPosition, WeatherAnimationType.FixedAnimationFrame, 2);

            SetTransparency(0);
        }

        public override void Update(GameTime gameTime)
        {
            VerticalScrollingUpdate(gameTime);

            if (isActive)
            {
                FadeOut(gameTime, Parameter.WeatherEffectFadeTime);
            }
            else
            {
                FadeIn(gameTime, Parameter.WeatherEffectFadeTime);
            }
        }

        //Ideally random weather shouldn't merge.
        public override Weather Merge(Weather weather) { return this; }

        public override void OnInteract(Projectile projectile)
        {
            projectile.OnBeginRandomInteraction(this);

            //If the project can't collide, it should not be taken into consideration for spawning/explosion effects
            if (!projectile.CanCollide) return;

            if (!isActive)
            {
                isActive = true;
                LevelScene.WeatherHandler.Add(extraWeatherType, StartingPosition);
            }
        }

        public override void OnStopInteracting(Projectile projectile) { }
    }
}
