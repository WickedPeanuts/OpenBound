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
using OpenBound.GameComponents.Audio;
using OpenBound.GameComponents.Level;
using OpenBound.GameComponents.Pawn.UnitProjectiles;
using OpenBound.GameComponents.MobileAction;
using OpenBound_Network_Object_Library.Entity;
using System;

namespace OpenBound.GameComponents.WeatherEffect
{
    public class Electricity : Weather
    {
        public Electricity(Vector2 position, float scale = 1) : base(new Vector2(position.X, -Topography.MapHeight / 2), new Vector2(64, 32), 8, new Vector2(20, 0), new Vector2(10, 10), WeatherType.Electricity, scale, 0)
        {
            Initialize("Graphics/Special Effects/Weather/Electricity", StartingPosition, WeatherAnimationType.VariableAnimationFrame, 2);
            SetTransparency(0);
        }

        public override void Update(GameTime gameTime)
        {
            VerticalScrollingUpdate(gameTime);
            FadeIn(gameTime, Parameter.WeatherEffectFadeTime);
        }

        public override Weather Merge(Weather weather)
        {
            return new Electricity((StartingPosition + weather.StartingPosition) / 2, Scale + weather.Scale);
        }

        public override void OnInteract(Projectile projectile)
        {
            //Checks if the projectile is already under influence of this weather 
            if (CheckWeatherInfluence(projectile)) return;

            //Passes this instance to any projectile's dependent projectile
            projectile.OnBeginElectricityInteraction(this);

            //If the project can't collide, it should not be taken into consideration for spawning/explosion effects
            if (!projectile.CanCollide) return;

            //Once added to the list, the projectile starts spawning electrical particles around it's flipbook
            SpecialEffect se = SpecialEffectBuilder.ElectricityParticle(projectile.Position);
            projectile.OnAfterUpdateAction += () =>
            {
                se.Flipbook.Position = projectile.Position;
                se.Flipbook.Rotation = projectile.CurrentFlipbookRotation;
            };

            //Install itself on the projectile explosion event forcing every exploding projectile to be removed from the spawning list
            Action removeParticle = () => SpecialEffectHandler.Remove(se);
            projectile.OnExplodeAction += removeParticle;
            projectile.OnBeingDestroyedAction += removeParticle;

            projectile.OnExplodeAction += () =>
            {
                LightningBaseProjectile ep = new LightningBaseProjectile(projectile.Mobile, projectile.Position,
                    MathHelper.PiOver2,
                    Parameter.WeatherEffectElectricityExplosionRadius,
                    Parameter.WeatherEffectElectricityEExplosionRadius,
                    Parameter.WeatherEffectElectricityBaseDamage,
                    Parameter.WeatherEffectElectricityEBaseDamage);

                ep.Update();

                AudioHandler.PlaySoundEffect("Audio/SFX/Tank/Blast/LightningS1");
            };
        }
    }
}
