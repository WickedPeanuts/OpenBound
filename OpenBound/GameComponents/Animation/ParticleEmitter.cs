using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenBound.GameComponents.Animation
{
    public class ParticleEmitter
    {
        float elapsedTime;

        Action particleCreation;

        public float EmissionTime;
        public bool IsActive;

        public ParticleEmitter(Action particleCreation, float emissionTime, bool isActive = false)
        {
            EmissionTime = emissionTime;
            this.particleCreation = particleCreation;
            IsActive = isActive;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedTime > EmissionTime)
            {
                elapsedTime = 0;
                particleCreation.Invoke();
            }
        }
    }
}
