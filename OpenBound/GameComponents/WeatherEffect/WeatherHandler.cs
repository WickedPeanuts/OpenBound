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
using OpenBound_Network_Object_Library.Entity;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound.GameComponents.WeatherEffect
{
    public class WeatherHandler
    {
        List<Weather> toBeAddedWeatherList;

        /// <Summary>
        /// List of all instanced weathers on the scenario. Do not add elements in here. In order to add new elements and prevent
        /// access/overwriting problems use <cref see="Add"/>.
        /// </Summary>
        public List<Weather> WeatherList;

        //List of weathers that are going to be removed
        List<Weather> unusedWeatherList;

        public WeatherHandler()
        {
            toBeAddedWeatherList = new List<Weather>();
            WeatherList = new List<Weather>();
            unusedWeatherList = new List<Weather>();
        }

        public void Add(Weather weather)
        {
            toBeAddedWeatherList.Add(weather);
        }

        public Weather AddAbsoluteWeather(WeatherType weatherType, WeatherType extraWeatherType, Vector2 position, float scale = 1, bool shouldRender = true, float rotation = 0)
        {
            Weather weather = CreateWeather(weatherType, extraWeatherType, position, scale);
            weather.IsMergeable = false;
            weather.ShouldRender = shouldRender;
            toBeAddedWeatherList.Add(weather);
            return weather;
        }

        public void Add(WeatherType weatherType, WeatherType extraWeatherType, Vector2 position, float rotation = 0)
        {
            Weather weather = CreateWeather(weatherType, extraWeatherType, position);

            //Return if the given weather is not implemented yet
            if (weather == null) return;

            if (weather.IsMergeable && weatherType != WeatherType.Random)
                CheckAndMergeWeatherEffects(weather);
            else
                toBeAddedWeatherList.Add(weather);
        }

        public void Add(WeatherType weatherType, Vector2 position, float rotation = 0)
        {
            Add(weatherType, default, position, rotation);
        }

        public static Weather CreateWeather(WeatherType weatherType, WeatherType extraWeatherType, Vector2 position, float scale = 1)
        {
            switch (weatherType)
            {
                case WeatherType.Tornado:
                    return new Tornado(position, scale);
                case WeatherType.Force:
                    return new Force(position, scale);
                case WeatherType.Weakness:
                    return new Weakness(position, scale);
                case WeatherType.Mirror:
                    return new Mirror(position, scale);
                case WeatherType.Electricity:
                    return new Electricity(position, scale);
                case WeatherType.Random:
                    return new Random(position, extraWeatherType);
                case WeatherType.Thor:
                    return new Thor(position);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Check if there is an existing weather and if this weather can be merged together. The merging happen if
        /// their (existing and new weather) collider overlap.
        /// </summary>
        /// <param name="weather">The weather to be added on the list.</param>
        public void CheckAndMergeWeatherEffects(Weather weather)
        {
            bool hasMerged;

            do
            {
                hasMerged = false;
                foreach (Weather w in WeatherList.Except(unusedWeatherList))
                {
                    if (w.WeatherType == weather.WeatherType && w.Intersects(weather) && w != weather)
                    {
                        weather = w.Merge(weather);
                        unusedWeatherList.Add(w);
                        hasMerged = true;
                    }
                }
            } while (hasMerged);

            toBeAddedWeatherList.Add(weather);
        }

        /// <summary>
        /// Removes a weather from the list/scene.
        /// </summary>
        public void RemoveWeather(WeatherType weatherEffectType)
        {
            unusedWeatherList.AddRange(WeatherList.Where((x) => x.WeatherType != weatherEffectType && !x.Expires));
        }

        public void RemoveWeather(Weather weather)
        {
            unusedWeatherList.Add(weather);
        }

        public void Update(GameTime gameTime)
        {
            WeatherList.ForEach((x) => x.Update(gameTime));

            foreach(Weather w in unusedWeatherList)
            {
                if (toBeAddedWeatherList.Count() == 0)
                    w.OnBeingRemoved(null);
                else
                    w.OnBeingRemoved(toBeAddedWeatherList[0]);

                WeatherList.Remove(w);
            }

            unusedWeatherList.Clear();

            WeatherList.AddRange(toBeAddedWeatherList);
            toBeAddedWeatherList.Clear();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            WeatherList.ForEach((x) => x.Draw(gameTime, spriteBatch));
        }
    }
}
