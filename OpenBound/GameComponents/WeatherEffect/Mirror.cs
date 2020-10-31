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
using OpenBound.GameComponents.Audio;
using OpenBound.GameComponents.Level;
using OpenBound.GameComponents.MobileAction;
using OpenBound_Network_Object_Library.Entity;

namespace OpenBound.GameComponents.WeatherEffect
{
    public class Mirror : Force
    {
        public Mirror(Vector2 position, float scale = 1) : base(new Vector2(position.X, -(Topography.MapHeight / 2) * Parameter.WeatherEffectMirrorDistanceFromTopReduction), WeatherType.Mirror, scale)
        {
            Initialize("Graphics/Special Effects/Weather/Mirror", StartingPosition, WeatherAnimationType.VariableAnimationFrame, 2);
            SetTransparency(0);
        }

        public override Weather Merge(Weather weather)
        {
            return new Mirror((StartingPosition + weather.StartingPosition) / 2, Scale + weather.Scale);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateProjectiles(gameTime);
            FadeIn(gameTime, Parameter.WeatherEffectFadeTime);
        }

        public override void OnInteract(Projectile projectile)
        {
            //Sound Effect
            AudioHandler.PlaySoundEffect("Audio/SFX/Weather/Mirror");

            //Force interactions
            base.OnInteract(projectile);

            //Passes the command to reflect to any projectile's dependent projectile
            projectile.OnBeginMirrorInteraction();

            //Register if the project has interacted with this weather during its existence
            projectile.InteractedWeatherSet.Add(WeatherType);
        }

        public override void CalculateDamage(Projectile projectile)
        {
            //If the projectile base damage = 0, it should not increase
            if (projectile.BaseDamage != 0)
                projectile.BaseDamage = (int)(projectile.BaseDamage * Parameter.WeatherEffectMirrorDamageIncreaseFactor + Parameter.WeatherEffectMirrorDamageIncreaseValue);
        }
    }
}
