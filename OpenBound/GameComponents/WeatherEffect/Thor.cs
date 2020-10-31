using Microsoft.Xna.Framework;
using OpenBound.GameComponents.MobileAction;
using OpenBound_Network_Object_Library.Entity;
using OpenBound.GameComponents.Pawn;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Interface;

namespace OpenBound.GameComponents.WeatherEffect
{
    public class Thor : Weather
    {
        public Thor(Vector2 position) : base(default, default, 0, default, default, WeatherType.Thor, 0)
        {
            LevelScene.ThorSatellite.SetPosition(position);
            LevelScene.ThorSatellite.Activate();

            SetTransparency(0);
        }

        public override bool CheckProjectileInteraction(Projectile projectile) {

            if (ModifiedProjectileList.Contains(projectile)) return false;
            
            OnInteract(projectile);
            ModifiedProjectileList.Add(projectile);

            projectile.OnExplodeAction += () =>
            {
                OnStopInteracting(projectile);
            };

            return true;
        }
        
        public override void OnBeingRemoved(Weather incomingWeather)
        {
            if (incomingWeather == null || incomingWeather.WeatherType != WeatherType.Thor)
                LevelScene.ThorSatellite.Deactivate();
        }

        public override bool Intersects(Projectile projectile) => true;

        public override bool Intersects(Weather weather) => true;

        public override Weather Merge(Weather weather) { return this; }

        public override void OnInteract(Projectile projectile) { projectile.OnBeginThorInteraction(this); }

        public override void Update(GameTime gameTime) { }
    }
}
