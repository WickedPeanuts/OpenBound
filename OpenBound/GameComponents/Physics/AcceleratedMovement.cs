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

using System;

namespace OpenBound.GameComponents.Physics
{
    /// <summary>
    /// This Class is used for movements that doesn't have to be time-dependent
    /// Currently its being used to calculate projectile next position after the update
    /// </summary>
    public class AcceleratedMovement
    {
        public float CurrentPosition { get; private set; }

        public float InitialSpeed { get; private set; }
        public float CurrentSpeed { get; private set; }

        public float Acceleration { get; private set; }

        public float CurrentTime { get; private set; }

        public void Preset(float InitialSpeed, float Acceleration)
        {
            CurrentSpeed = CurrentPosition = CurrentTime = 0;

            this.InitialSpeed = InitialSpeed;
            this.Acceleration = Acceleration;
        }

        public void RefreshCurrentPosition(float TimeElapsed)
        {
            CurrentTime += TimeElapsed;

            //v = v0 + a*t
            CurrentSpeed = (InitialSpeed + Acceleration * CurrentTime);

            //x = x0 + v*t
            CurrentPosition = CurrentSpeed * CurrentTime;
        }

        public float ProjectNextPosition(float TimeElapsed)
        {
            return (InitialSpeed + Acceleration * (CurrentTime + TimeElapsed)) * (CurrentTime + TimeElapsed);
        }

        public void InverseMovement()
        {
            InitialSpeed *= -1;
            Acceleration *= -1;
            CurrentTime = 0;
        }
    }
}
