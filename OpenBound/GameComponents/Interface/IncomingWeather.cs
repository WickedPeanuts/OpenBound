using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Interface.Text;
using OpenBound.GameComponents.Level;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.WeatherEffect;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBound.GameComponents.Interface
{
    public class IncomingWeatherPointer
    {
        public Flipbook flipbook;
        public SpriteText spriteText;
        public Vector2 position;

        private float fadeAnimation;

        public WeatherMetadata WeatherMetadata { get; private set; }

        public IncomingWeatherPointer(WeatherMetadata weatherMetadata)
        {
            WeatherMetadata = weatherMetadata;

            fadeAnimation = 0;

            position = Topography.FromNormalizedPositionToRelativePosition(weatherMetadata.Position);
            flipbook = new Flipbook(Vector2.Zero, new Vector2(9, -11), 19, 27, "Interface/InGame/HUD/Blue/IncomingWeather/Pointer", new AnimationInstance { EndingFrame = 6, TimePerFrame = 1/15f }, 0.95f, 0);

            string text;

            switch (weatherMetadata.Weather)
            {
                case WeatherType.Force:
                    text = Language.WeatherForce;
                    break;
                case WeatherType.Tornado:
                    text = Language.WeatherTornado;
                    break;
                case WeatherType.Electricity:
                    text = Language.WeatherElectricity;
                    break;
                case WeatherType.Weakness:
                    text = Language.WeatherWeakness;
                    break;
                case WeatherType.Mirror:
                    text = Language.WeatherMirror;
                    break;
                default:
                    text = Language.WeatherRandom;
                    break;
            }

            spriteText = new SpriteText(FontTextType.Consolas10Family, text, Color.White, Alignment.Center, layerDepth: 1);

            flipbook.Position = new Vector2(position.X, -(GameScene.Camera.CameraOffset + Parameter.ScreenResolution / 2).Y);
            spriteText.Position = flipbook.Position + new Vector2(0, 0);

            flipbook.Color = spriteText.Color = spriteText.OutlineColor = Color.Transparent;
        }

        public void Update(GameTime gameTime)
        {
            flipbook.Position = new Vector2(position.X, -(GameScene.Camera.CameraOffset + Parameter.ScreenResolution / 2).Y);
            spriteText.Position = flipbook.Position + new Vector2(0, 0);

            if (fadeAnimation != 1)
            {
                fadeAnimation = Math.Min(fadeAnimation + (float)gameTime.ElapsedGameTime.TotalSeconds / 2, 1);
                flipbook.SetTransparency(fadeAnimation);
                spriteText.SetTransparency(fadeAnimation);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            flipbook.Draw(gameTime, spriteBatch);
            spriteText.Draw(spriteBatch);
        }
    }
}