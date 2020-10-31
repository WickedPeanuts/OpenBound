using Microsoft.Xna.Framework;
using OpenBound.Common;
using OpenBound.GameComponents.Level;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Physics;
using System;

namespace OpenBound.GameComponents.Animation
{
    public class ParticleUpdateConfiguration
    {
        public bool Rotate;
        public bool UseYAcceleration;
        public bool UseXAcceleration;
        public bool SetScale;
        public bool Fade;
    }

    public class Particle : SpecialEffect
    {
        float rotatingSpeed;
        float scalingFactor;
        float alpha, alphaFactor;

        AcceleratedMovement yMovement, xMovement;

        Vector2 initialPosition;

        float RandomValue => (float)(Parameter.Random.NextDouble() - 0.5f);

        public Particle(Flipbook flipbook, Vector2 trajectory, Vector2 windInfluence, ParticleUpdateConfiguration particleConfiguration) : base(flipbook)
        {
            alpha = 1;

            if (particleConfiguration.Rotate)
                rotatingSpeed = RandomValue * Parameter.GroundParticleMaximumRotatingSpeed;
            if (particleConfiguration.SetScale)
                scalingFactor = RandomValue * Parameter.GroundParticleMaximumScalingFactor;
            if (particleConfiguration.Fade)
                alphaFactor = Parameter.GroundParticleInitialAlphaFactor + (float)Parameter.Random.NextDouble() * 5;

            float randomValue = (float)Parameter.Random.NextDouble();

            float yAccFactor = (particleConfiguration.UseYAcceleration) ? Parameter.GroundParticleInitialYAcceleration - windInfluence.Y : 0;
            float xAccFactor = (particleConfiguration.UseXAcceleration) ? windInfluence.X : 0;

            yMovement = new AcceleratedMovement();
            yMovement.Preset(Parameter.GroundParticleInitialYSpeed + (randomValue * Parameter.GroundParticleMaximumYSpeed * trajectory.Y), yAccFactor);

            xMovement = new AcceleratedMovement();
            xMovement.Preset(randomValue * Parameter.GroundParticleMaximumXSpeed * trajectory.X, xAccFactor);

            initialPosition = flipbook.Position;
        }

        public override bool CeaseFunction()
        {
            return Topography.IsNotInsideMapBoundaries(Flipbook.Position) || Flipbook.Color == Color.Transparent;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Flipbook.Rotation += rotatingSpeed * elapsedTime;
            Flipbook.Scale += Vector2.One * scalingFactor * elapsedTime;
            Flipbook.SetTransparency(alpha -= alphaFactor * elapsedTime);

            xMovement.RefreshCurrentPosition(elapsedTime);
            yMovement.RefreshCurrentPosition(elapsedTime);

            Flipbook.Position = initialPosition + new Vector2(xMovement.CurrentPosition, yMovement.CurrentPosition);
        }
    }
}
