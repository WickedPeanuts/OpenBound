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
using Newtonsoft.Json;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity.Sync;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound_Network_Object_Library.Entity
{
    public class WeatherMetadata
    {
        public float[] Position;
        public WeatherType Weather, ExtraWeather;
    }

    public class MatchMetadata
    {
        //public SyncMobile CurrentTurnOwner;
        public List<SyncMobile> SyncMobileList;

        [JsonProperty("WF")] public int WindForce;
        [JsonProperty("WA")] public int WindAngleDegrees;

        public float WindAngleRadians => 0.01745f * WindAngleDegrees;

        [JsonProperty("CWL")] public List<WeatherMetadata> CurrentWeatherList;
        [JsonProperty("IWL")] public List<WeatherMetadata> IncomingWeatherList;

        [JsonProperty("TC")] public int TurnCount;
        [JsonProperty("RC")] public int RoundCount;

        public Dictionary<int, PlayerPerformanceMetadata> PlayerPerformanceDictionary;

        [JsonIgnore]
        Map map;

        public MatchMetadata() { }

        public MatchMetadata(Map map, List<SyncMobile> syncMobileList)
        {
            this.map = map;
            SyncMobileList = syncMobileList;

            IncomingWeatherList = new List<WeatherMetadata>();
            CurrentWeatherList = new List<WeatherMetadata>();
            PlayerPerformanceDictionary = new Dictionary<int, PlayerPerformanceMetadata>();

            foreach(SyncMobile sm in syncMobileList)
                PlayerPerformanceDictionary.Add(sm.Owner.ID, new PlayerPerformanceMetadata());

            for (int i = 0; i < 4; i++) EnqueueNextWeather();

            ProcessUpcomingWeather();
        }

        public float[] WindForceComponents()
        {
            float waR = WindAngleRadians;
            return new float[] { (float)Math.Cos(waR) * WindForce, (float)Math.Sin(waR) * WindForce };
        }

        internal void PassTurn(int RoomSize)
        {
            IncomingWeatherList.Clear();
            if (++TurnCount == RoomSize)
            {
                TurnCount = 0;
                RoundCount++;

                EnqueueNextWeather();
                ProcessUpcomingWeather();
            }
        }

        private void EnqueueNextWeather()
        {
            int val = NetworkObjectParameters.Random.Next(0, map.WeatherPreset.Sum());

            WeatherMetadata weatherMetadata = new WeatherMetadata();

            if ((val -= map.Force) < 0) weatherMetadata.Weather = WeatherType.Force;
            else if ((val -= map.Tornado) < 0) weatherMetadata.Weather = WeatherType.Tornado;
            else if ((val -= map.Electricity) < 0) weatherMetadata.Weather = WeatherType.Electricity;
            else if ((val -= map.Wind) < 0) weatherMetadata.Weather = WeatherType.Wind;
            else if ((val -= map.Weakness) < 0) weatherMetadata.Weather = WeatherType.Weakness;
            else if ((val -= map.Protection) < 0) weatherMetadata.Weather = WeatherType.Protection;
            else if ((val -= map.Ignorance) < 0) weatherMetadata.Weather = WeatherType.Ignorance;
            else if ((val -= map.Thor) < 0) weatherMetadata.Weather = WeatherType.Thor;
            else if ((val -= map.Mirror) < 0) weatherMetadata.Weather = WeatherType.Mirror;
            else weatherMetadata.Weather = WeatherType.Random;

            CurrentWeatherList.Add(weatherMetadata);
            IncomingWeatherList.Add(weatherMetadata);

            //Generate extra information (if necessary)
            if (NetworkObjectParameters.ActiveWeatherEffectList.Contains(weatherMetadata.Weather))
            {
                CalculateRandomPosition(weatherMetadata);

                if (weatherMetadata.Weather == WeatherType.Random)
                    RandomizeExtraWeather(weatherMetadata);

                if (weatherMetadata.Weather == WeatherType.Thor)
                {
                    //Force thor to spawn in the upper side of the screen from
                    //x ranging from 0.1f to 0.9f and
                    //y ranging from 0.3f to 0.9f
                    weatherMetadata.Position[0] = NetworkObjectParameters.WeatherThorMinimumOffsetX + weatherMetadata.Position[0] * NetworkObjectParameters.WeatherThorMaximumOffsetX;
                    weatherMetadata.Position[1] = 1 - (NetworkObjectParameters.WeatherThorMinimumOffsetY + weatherMetadata.Position[1] * NetworkObjectParameters.WeatherThorMaximumOffsetY);
                }
            }
        }

        private void ProcessUpcomingWeather()
        {
            WeatherMetadata wm = CurrentWeatherList[0];
            CurrentWeatherList.RemoveAt(0);

            DisturbWind();

            if (wm.Weather == WeatherType.Wind) ChangeWind();
            //else if (wm.Weather == WeatherType.Protection) ApplyProtection();
        }

        private void RandomizeExtraWeather(WeatherMetadata weatherMetadata)
        {
            weatherMetadata.ExtraWeather = NetworkObjectParameters.RandomizableWeatherEffectList[NetworkObjectParameters.Random.Next(0, NetworkObjectParameters.RandomizableWeatherEffectList.Count())];
        }

        private void CalculateRandomPosition(WeatherMetadata weatherMetadata)
        {
            weatherMetadata.Position = new float[] { (float)NetworkObjectParameters.Random.NextDouble(), (float)NetworkObjectParameters.Random.NextDouble() };
        }

        private void DisturbWind()
        {
            if (NetworkObjectParameters.Random.NextDouble() <= NetworkObjectParameters.WeatherWindAngleDisturbanceChance)
                return;

            WindForce = Math.Max(0, (WindForce + NetworkObjectParameters.Random.Next(0, NetworkObjectParameters.WeatherWindForceDisturbance) - NetworkObjectParameters.WeatherWindForceDisturbance / 2));
            WindAngleDegrees += NetworkObjectParameters.Random.Next(0, NetworkObjectParameters.WeatherWindAngleDisturbance) - NetworkObjectParameters.WeatherWindAngleDisturbance / 2;
            WindAngleDegrees = WindAngleDegrees % 360;
        }

        /*private void ApplyProtection()
        {
            foreach(SyncMobile sm in SyncMobileList)
            {
                sm.MobileMetadata.CurrentHealth += sm.MobileMetadata.HealthRegenerationProtection;
            }
        }*/

        private void ChangeWind()
        {
            WindForce = NetworkObjectParameters.Random.Next(0, NetworkObjectParameters.WeatherMaximumWindForce);
            WindAngleDegrees = NetworkObjectParameters.Random.Next(0, 360);
        }
    }
}
