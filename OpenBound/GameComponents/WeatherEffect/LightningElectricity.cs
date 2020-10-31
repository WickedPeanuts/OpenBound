using Microsoft.Xna.Framework;
using OpenBound.Common;
using OpenBound.GameComponents.Level;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.MobileAction;

namespace OpenBound.GameComponents.WeatherEffect
{
    /// <summary>
    /// Lightning special effect. This "fake weather" is only used by lightning's projectiles and electricity weather.
    /// </summary>
    public class LightningElectricity : Weather
    {
        public LightningElectricity(Vector2 position, float angle) : base(new Vector2(position.X, -Topography.MapHeight / 2), new Vector2(31, 128), 3, default, default, default, 1, angle)
        {
            Initialize(@"Graphics/Special Effects/Tank/Lightning/Flame1", position, WeatherAnimationType.VariableAnimationFrame,
                layerDepth: DepthParameter.ProjectileSFX);
            Expires = true;
        }

        public override void OnInteract(Projectile projectile) { }

        public override void OnStopInteracting(Projectile projectile) { }

        public override void Update(GameTime gameTime)
        {
            FadeOut(gameTime, Parameter.ProjectileLightningElectricityFadeTime);

            if (fadeAnimationElapsedTime == Parameter.ProjectileLightningElectricityFadeTime)
                LevelScene.WeatherHandler.RemoveWeather(this);
        }

        public override Weather Merge(Weather weather) { return this; }
    }
}
