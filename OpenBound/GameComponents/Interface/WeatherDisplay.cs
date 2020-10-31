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
using OpenBound.Common;
using OpenBound.Extension;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Audio;
using OpenBound.GameComponents.Level;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Asset;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using System;
using System.Collections.Generic;
using OpenBound.GameComponents.Pawn;

namespace OpenBound.GameComponents.Interface
{
    public enum WeatherDisplayAnimationState
    {
        ColorFadeIn,
        Moving,
        ColorFadeOut,
        Idle,
    }

    public class WeatherDisplay
    {
        public static Dictionary<WeatherType, Rectangle> WeatherIconPresets = new Dictionary<WeatherType, Rectangle>()
        {
            { WeatherType.Force,        new Rectangle(22 * 00, 00, 22, 22) },
            { WeatherType.Tornado,      new Rectangle(22 * 01, 00, 22, 22) },
            { WeatherType.Electricity,  new Rectangle(22 * 02, 00, 22, 22) },
            { WeatherType.Wind,         new Rectangle(22 * 03, 00, 22, 22) },
            { WeatherType.Thor,         new Rectangle(22 * 04, 00, 22, 22) },
            { WeatherType.Protection,   new Rectangle(22 * 05, 00, 22, 22) },
            { WeatherType.Ignorance,    new Rectangle(22 * 06, 00, 22, 22) },
            { WeatherType.Weakness,     new Rectangle(22 * 07, 00, 22, 22) },
            { WeatherType.Mirror,       new Rectangle(22 * 08, 00, 22, 22) },
            { WeatherType.Random,       new Rectangle(22 * 09, 00, 22, 22) },
            { WeatherType.DWeather,     new Rectangle(22 * 10, 00, 22, 22) },
            { WeatherType.FWeather,     new Rectangle(22 * 11, 00, 22, 22) },
            { WeatherType.TWeather,     new Rectangle(22 * 12, 00, 22, 22) },
            { WeatherType.BWeather,     new Rectangle(22 * 13, 00, 22, 22) },
            { WeatherType.GWeather,     new Rectangle(22 * 14, 00, 22, 22) },

            { WeatherType.GForce,       new Rectangle(22 * 00, 22, 22, 22) },
            { WeatherType.GTornado,   new Rectangle(22 * 01, 22, 22, 22) },
            { WeatherType.GElectricity, new Rectangle(22 * 02, 22, 22, 22) },
            { WeatherType.GWind,        new Rectangle(22 * 03, 22, 22, 22) },
            { WeatherType.GThor,        new Rectangle(22 * 04, 22, 22, 22) },
            { WeatherType.GProtection,  new Rectangle(22 * 05, 22, 22, 22) },
            { WeatherType.GIgnorance,   new Rectangle(22 * 06, 22, 22, 22) },
            { WeatherType.GWeakness,    new Rectangle(22 * 07, 22, 22, 22) },
            { WeatherType.GMirror,      new Rectangle(22 * 08, 22, 22, 22) },
            { WeatherType.GRandom,      new Rectangle(22 * 09, 22, 22, 22) },
            { WeatherType.GDWeather,    new Rectangle(22 * 10, 22, 22, 22) },
            { WeatherType.GFWeather,    new Rectangle(22 * 11, 22, 22, 22) },
            { WeatherType.GTWeather,    new Rectangle(22 * 12, 22, 22, 22) },
            { WeatherType.GBWeather,    new Rectangle(22 * 13, 22, 22, 22) },
            { WeatherType.GGWeather,    new Rectangle(22 * 14, 22, 22, 22) },
        };

        Sprite currentWeatherFrame, currentWeatherFrameContent;
        Sprite weatherRouletteColored;
        Sprite weatherRouletteGrayscale;

        Texture2D weatherTexture;

        //Movement Animation
        List<WeatherMetadata> weatherList, displayingWeatherList;
        Rectangle weatherRouletteRectangle;
        float currentOffset;
        float movingAnimationTime;

        //Color animation
        float colorFadeAnimationTime;

        //Animation Instances
        WeatherDisplayAnimationState animationState;

        //Incoming Weather pointer
        Queue<IncomingWeatherPointer> incomingWeatherPointers;

#if !DEBUGSCENE
        public WeatherMetadata ActiveWeather => displayingWeatherList[displayingWeatherList.Count > 4 ? 1 : 0];
#else
        public WeatherMetadata ActiveWeather => (displayingWeatherList.Count == 0) ? null : displayingWeatherList[displayingWeatherList.Count > 4 ? 1 : 0];
#endif
        private WeatherMetadata previousWeather;

        public WeatherDisplay(Vector2 position)
        {
            weatherList = new List<WeatherMetadata>();
            displayingWeatherList = new List<WeatherMetadata>();
            incomingWeatherPointers = new Queue<IncomingWeatherPointer>();

            currentWeatherFrame = new Sprite("Interface/InGame/HUD/Blue/WeatherDisplay/Frame", position: position, layerDepth: DepthParameter.HUDForeground);
            currentWeatherFrameContent = new Sprite("Interface/InGame/HUD/Blue/WeatherDisplay/Weather", position: position, layerDepth: 0.99f)
            {
                Pivot = new Vector2(12, 11),
            };
            currentWeatherFrameContent.SetTransparency(0);

            weatherRouletteColored = new Sprite(24 * 7, 22, position: position, layerDepth: 0.99f);
            weatherRouletteGrayscale = new Sprite(24 * 7, 22, position: position, layerDepth: 0.98f);

            weatherRouletteColored.SourceRectangle = weatherRouletteGrayscale.SourceRectangle = weatherRouletteRectangle = new Rectangle(0, 0, 24 * 5 - 2, 22);
            weatherRouletteColored.Pivot = weatherRouletteGrayscale.Pivot = new Vector2(36, 11);

            weatherTexture = AssetHandler.Instance.RequestTexture("Interface/InGame/HUD/Blue/WeatherDisplay/Weather");
            animationState = WeatherDisplayAnimationState.Idle;
        }

        private void AppendWeather()
        {
            WeatherMetadata weather = weatherList[0];
            weatherList.RemoveAt(0);

            Rectangle rS1 = WeatherIconPresets[weather.Weather];
            Rectangle rS2 = WeatherIconPresets[weather.Weather + (int)WeatherType.GForce];

            Point offset = new Point(rS1.Width, rS1.Height);
            Point originS1 = new Point(rS1.X, rS1.Y);
            Point originS2 = new Point(rS2.X, rS2.Y);

            weatherRouletteColored.Texture2D.AppendTexture(originS1, originS1 + offset, weatherTexture, new Point(24 * 6, 0));
            weatherRouletteGrayscale.Texture2D.AppendTexture(originS2, originS2 + offset, weatherTexture, new Point(24 * 6, 0));

            weatherRouletteColored.Texture2D.ShiftLeft(24, Color.Transparent);
            weatherRouletteGrayscale.Texture2D.ShiftLeft(24, Color.Transparent);

            currentOffset = 24;
            movingAnimationTime = 0;
            colorFadeAnimationTime = 0;

            weatherRouletteColored.SourceRectangle = weatherRouletteGrayscale.SourceRectangle = weatherRouletteRectangle;

            if (displayingWeatherList.Count > 0)
                previousWeather = ActiveWeather;

            displayingWeatherList.Add(weather);

            if (weather.Weather != WeatherType.Thor && NetworkObjectParameters.ActiveWeatherEffectList.Contains(weather.Weather))
                incomingWeatherPointers.Enqueue(new IncomingWeatherPointer(weather));

            if (displayingWeatherList.Count > 5)
                displayingWeatherList.RemoveAt(0);

            if (displayingWeatherList.Count >= 4)
            {
                ApplyCurrentWeatherEffect();
            }
        }

        public void ApplyCurrentWeatherEffect()
        {
            //Wipe any other weather if it has changed
            if (previousWeather.Weather != ActiveWeather.Weather)
                LevelScene.WeatherHandler.RemoveWeather(ActiveWeather.Weather);

            //Active Weathers
            if (NetworkObjectParameters.ActiveWeatherEffectList.Contains(ActiveWeather.Weather))
            {
                LevelScene.WeatherHandler.Add(ActiveWeather.Weather, ActiveWeather.ExtraWeather, Topography.FromNormalizedPositionToRelativePosition(ActiveWeather.Position));

                if (incomingWeatherPointers.Count > 0 && 
                    incomingWeatherPointers.Peek().WeatherMetadata == ActiveWeather)
                    incomingWeatherPointers.Dequeue();
            }

            //Unlock items (if necessary)
            if (ActiveWeather.Weather != WeatherType.Ignorance)
                LevelScene.HUD.MatchStatusClosed.SetTransparency(0);

            switch (ActiveWeather.Weather)
            {
                case WeatherType.Wind:
                    AudioHandler.PlaySoundEffect(SoundEffectParameter.InGameWeatherWindTransition);
                    break;
                case WeatherType.Protection:
                    foreach(Mobile mob in LevelScene.MobileList)
                        mob.Regenerate(true);
                    break;
                case WeatherType.Ignorance:
                    LevelScene.HUD.LockItem();
                    break;
            }
        }

        public void AppendWeatherToList(WeatherMetadata weather)
        {
            weatherList.Add(weather);
        }

        public void AppendWeatherToList(List<WeatherMetadata> weatherList)
        {
            this.weatherList.AddRange(weatherList);
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            switch (animationState)
            {
                //First step, change the color of the grayscale layer from 0 - 1
                case WeatherDisplayAnimationState.ColorFadeIn:
                    colorFadeAnimationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    weatherRouletteColored.SetTransparency(colorFadeAnimationTime);
                    if (colorFadeAnimationTime > 1)
                    {
                        animationState = WeatherDisplayAnimationState.Moving;
                        currentWeatherFrameContent.SetTransparency(0);
                    }
                    break;
                //Second step, while there is weather, shift to the left
                case WeatherDisplayAnimationState.Moving:
                    float tmpOffset = currentOffset;

                    movingAnimationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    currentOffset = (float)(24 * (1 + Math.Cos(movingAnimationTime * 2 * MathHelper.Pi)) / 2);

                    if (tmpOffset < currentOffset)
                    {
                        currentOffset = 0;
                        movingAnimationTime = 0;

                        if (weatherList.Count != 0)
                            animationState = WeatherDisplayAnimationState.Idle;
                        else
                        {
                            animationState = WeatherDisplayAnimationState.ColorFadeOut;
                            colorFadeAnimationTime = 1;
                        }
                    }

                    weatherRouletteColored.SourceRectangle =
                        weatherRouletteGrayscale.SourceRectangle =
                            new Rectangle((int)(weatherRouletteRectangle.X + 24 - currentOffset), weatherRouletteRectangle.Y, weatherRouletteRectangle.Width, weatherRouletteRectangle.Height);
                    break;
                //Third step, change back the color of the grayscale mask from 1 to 0.
                case WeatherDisplayAnimationState.ColorFadeOut:
                    if (displayingWeatherList.Count >= 4)
                        currentWeatherFrameContent.SourceRectangle = WeatherIconPresets[ActiveWeather.Weather];

                    currentWeatherFrameContent.SetTransparency(1);
                    colorFadeAnimationTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    weatherRouletteColored.SetTransparency(colorFadeAnimationTime);
                    if (colorFadeAnimationTime < 0) animationState = WeatherDisplayAnimationState.Idle;
                    break;
                //Forth and last step. Keep waiting for a new element on WeatherList to re-start the animation sequence
                default:
                    if (weatherList.Count == 0) return;

                    AppendWeather();

                    if (weatherRouletteColored.Color.A > 0)
                        animationState = WeatherDisplayAnimationState.Moving;
                    else
                        animationState = WeatherDisplayAnimationState.ColorFadeIn;
                    return;
            }
        }

        public void Update(GameTime gameTime)
        {
            currentWeatherFrame.UpdateAttatchmentPosition();
            weatherRouletteColored.UpdateAttatchmentPosition();
            weatherRouletteGrayscale.UpdateAttatchmentPosition();
            currentWeatherFrameContent.UpdateAttatchmentPosition();

            incomingWeatherPointers.ForEach((x) => x.Update(gameTime));

            UpdateAnimation(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            currentWeatherFrame.Draw(gameTime, spriteBatch);
            weatherRouletteColored.Draw(gameTime, spriteBatch);
            weatherRouletteGrayscale.Draw(gameTime, spriteBatch);
            currentWeatherFrameContent.Draw(gameTime, spriteBatch);

            incomingWeatherPointers.ForEach((x) => x.Draw(gameTime, spriteBatch));
        }
    }
}
