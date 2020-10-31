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
using OpenBound.GameComponents.MobileAction;
using OpenBound_Network_Object_Library.Entity;
using System;
using System.Collections.Generic;

namespace OpenBound.GameComponents.WeatherEffect
{
    public class Force : Weather
    {
        List<WeatherProjectileParticleTimer> forceInteraction;

        /// <summary>
        /// This constructor must only be used by Mirror. If you intend to instantiate force, use the other one.
        /// </summary>
        protected Force(Vector2 position, WeatherType weatherType, float scale = 1, float rotation = 0) : base(position, new Vector2(64, 32), 8, new Vector2(20, 0), new Vector2(10, 10), weatherType, scale, rotation)
        {
            forceInteraction = new List<WeatherProjectileParticleTimer>();
        }

        public Force(Vector2 position, float scale = 1) : base(new Vector2(position.X, -Topography.MapHeight / 2), new Vector2(64, 32), 8, new Vector2(20, 0), new Vector2(10, 10), WeatherType.Force, scale, 0)
        {
            forceInteraction = new List<WeatherProjectileParticleTimer>();
            Initialize("Graphics/Special Effects/Weather/Force", StartingPosition, WeatherAnimationType.VariableAnimationFrame, 2);            
            SetTransparency(0);
        }

        public override void Update(GameTime gameTime)
        {
            FadeIn(gameTime, Parameter.WeatherEffectFadeTime);
            VerticalScrollingUpdate(gameTime);
            UpdateProjectiles(gameTime);
        }

        public virtual void UpdateProjectiles(GameTime gameTime)
        {
            //Foreach existing projectile that has interacted with the force
            for (int i = 0; i < forceInteraction.Count; i++)
            {
                //Spawn a new force particle special effect if its timer reaches zero
                if (forceInteraction[i].ParticleTimer <= 0)
                {
                    forceInteraction[i].ParticleTimer = (Parameter.WeatherEffectForceSpawnParticleStartingTime + (float)Parameter.Random.NextDouble()) / 32f;
                    SpecialEffectBuilder.ForceRandomParticle(forceInteraction[i].Projectile.Position + new Vector2(30 * (0.5f - (float)Parameter.Random.NextDouble()), 30 * (0.5f - (float)Parameter.Random.NextDouble())));
                }
                else
                {
                    forceInteraction[i].ParticleTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }

        public override Weather Merge(Weather weather)
        {
            return new Force((StartingPosition + weather.StartingPosition) / 2, Scale + weather.Scale);
        }

        public override void OnInteract(Projectile projectile)
        {
            //Checks if the projectile is already under influence of this weather 
            if (CheckWeatherInfluence(projectile)) return;

            //Passes this instance to any projectile's dependent projectile
            projectile.OnBeginForceInteraction(this);

            //If the project can't collide, it should not be taken into consideration for spawning/explosion effects
            if (!projectile.CanCollide) return;

            //Once added to the list the projectile starts spawning force particles around it's flipbook
            WeatherProjectileParticleTimer wppt = new WeatherProjectileParticleTimer() { Projectile = projectile };
            forceInteraction.Add(wppt);

            //Calculate new base damage for a projectile
            CalculateDamage(projectile);

            //Install itself on the projectile explosion event forcing every exploding projectile to be removed from the spawning list
            Action removeParticle = () =>
            {
                OnStopInteracting(projectile);
                forceInteraction.Remove(wppt);
            };
            projectile.OnExplodeAction += removeParticle;
            projectile.OnBeingDestroyedAction += removeParticle;

            //Install itself on the projectile ground destruction and dmg dealing
            Action<int> particleEffect = (p) =>
            {
                forceInteraction.Remove(forceInteraction.Find((x) => x.Projectile == projectile));
                ParticleBuilder.AsyncCreateForceCollapseParticleEffect(p, projectile.Position, projectile.CurrentFlipbookRotation);
            };

            projectile.OnDestroyGroundAction += particleEffect;
            projectile.OnDealDamageAction += particleEffect;
        }

        public virtual void CalculateDamage(Projectile projectile)
        {
            //If the projectile base damage = 0, it should not increase
            if (projectile.BaseDamage != 0)
                projectile.BaseDamage = (int)(projectile.BaseDamage * Parameter.WeatherEffectForceDamageIncreaseFactor + Parameter.WeatherEffectForceDamageIncreaseValue);
        }
    }
}
