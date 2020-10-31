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
    public class DefinedAcceleratedMovement
    {
        public float InitialPosition { get; private set; }
        public float CurrentPosition { get; private set; }
        public float FinalPosition { get; private set; }

        public float InitialSpeed { get; private set; }
        //public float CurrentSpeed { get; private set; }

        public float Acceleration { get; private set; }

        public float CurrentTime { get; private set; }
        public float TotalMotionTime { get; private set; }

        public bool IsMoving { get; private set; }

        //x = x0 + v0*t + a*t²/2. Since v0 = 0, we have:
        //x = x0 + a*t²/2, isolating the acceleration we got:
        //a = (x - x0)*2/t², where:

        //x = FinalPosition
        //x0 = InitialPosition
        //a = Acceleration wich we must determinate in order to create a perfect-timing movement
        //t = Total time spent during the movement
        public void Preset(float InitialPosition, float FinalPosition, float InitialSpeed, float TotalMotionTime)
        {
            this.InitialPosition = CurrentPosition = InitialPosition;
            this.FinalPosition = FinalPosition;
            this.InitialSpeed = InitialSpeed;
            this.TotalMotionTime = TotalMotionTime;
            IsMoving = true;

            CurrentTime = 0;

            Acceleration = ((FinalPosition - InitialPosition) - InitialSpeed * TotalMotionTime) / ((float)Math.Pow(TotalMotionTime, 2) / 2f);
        }

        public void RefreshCurrentPosition(float TimeElapsed)
        {
            //x = x0 + v0*t + a*t²/2
            if (CurrentTime + TimeElapsed < TotalMotionTime)
            {
                CurrentPosition = (float)(InitialPosition + InitialSpeed * CurrentTime + Acceleration * Math.Pow(CurrentTime, 2) / 2f);
                CurrentTime += TimeElapsed;
            }
            else
            {
                CurrentPosition = FinalPosition;
                IsMoving = false;
            }
        }
    }
}
