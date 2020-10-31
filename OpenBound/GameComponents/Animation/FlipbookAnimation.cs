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

namespace OpenBound.GameComponents.Animation
{
    public enum AnimationType
    {
        Foward,
        FowardStop,
        //Backward = 1,
        Cycle,
        Playlist,
    }

    /// <summary>
    /// Animation Instance is used when a simple flipbook
    /// </summary>
    public class AnimationInstance
    {
        public int StartingFrame;
        public int EndingFrame;
        public float TimePerFrame;
        public AnimationType AnimationType;
    }

    public abstract class FlipbookAnimation
    {
        public AnimationInstance AnimationInstance { get; set; }

        protected float timePast;

        public AnimationType AnimationType;

        public int CurrentFrameIndex;

        public bool AnimationEnded { get; set; }

        public bool IsLastFrame { get => CurrentFrameIndex + 1 > AnimationInstance.EndingFrame; }

        protected FlipbookAnimation(AnimationInstance FlipbookInstance)
        {
            AnimationInstance = FlipbookInstance;
            CurrentFrameIndex = FlipbookInstance.StartingFrame;
        }

        public static FlipbookAnimation GetInstance(AnimationInstance FlipbookInstance)
        {
            switch (FlipbookInstance.AnimationType)
            {
                case (AnimationType.Foward):
                    return new FlipbookAnimationFoward(FlipbookInstance);
                case (AnimationType.FowardStop):
                    return new FlipbookAnimationFowardStop(FlipbookInstance);
                case (AnimationType.Cycle):
                    return new FlipbookAnimationCycle(FlipbookInstance);
            }

            return null;
        }

        public void ChangeAnimation(AnimationInstance FlipbookInstance)
        {
            AnimationInstance = FlipbookInstance;
            Reset();
        }

        public abstract int GetNextAnimationIndex(GameTime GameTime);

        public void Reset()
        {
            timePast = 0;
            CurrentFrameIndex = AnimationInstance.StartingFrame;
        }
    }

    //The flipbook animation should reset after the last frame
    //or skip to the next animation if it was previously enqueued
    public class FlipbookAnimationFoward : FlipbookAnimation
    {
        public FlipbookAnimationFoward(AnimationInstance FlipbookInstance) : base(FlipbookInstance)
        { }

        public override int GetNextAnimationIndex(GameTime GameTime)
        {
            timePast += (float)GameTime.ElapsedGameTime.TotalSeconds;

            if (AnimationInstance.TimePerFrame == 0) return CurrentFrameIndex;

            if (timePast > AnimationInstance.TimePerFrame)
            {
                CurrentFrameIndex++;

                if (CurrentFrameIndex > AnimationInstance.EndingFrame)
                {
                    CurrentFrameIndex = AnimationInstance.StartingFrame;
                    AnimationEnded = true;
                }

                timePast = 0;
            }

            return CurrentFrameIndex;
        }
    }

    public class FlipbookAnimationFowardStop : FlipbookAnimation
    {
        public FlipbookAnimationFowardStop(AnimationInstance FlipbookInstance) : base(FlipbookInstance)
        { }

        public override int GetNextAnimationIndex(GameTime GameTime)
        {
            timePast += (float)GameTime.ElapsedGameTime.TotalSeconds;

            if (timePast > AnimationInstance.TimePerFrame)
            {
                CurrentFrameIndex++;
                if (CurrentFrameIndex > AnimationInstance.EndingFrame)
                {
                    CurrentFrameIndex = AnimationInstance.EndingFrame;
                    AnimationEnded = true;
                }

                timePast = 0;
            }

            return CurrentFrameIndex;
        }
    }

    //The animation here repeats itself, first cycle it goes foward, then backwards.
    public class FlipbookAnimationCycle : FlipbookAnimation
    {
        private bool isBackwards = false;
        public FlipbookAnimationCycle(AnimationInstance FlipbookInstance) : base(FlipbookInstance)
        { }

        public override int GetNextAnimationIndex(GameTime GameTime)
        {
            timePast += (float)GameTime.ElapsedGameTime.TotalSeconds;

            if (timePast > AnimationInstance.TimePerFrame)
            {
                if (isBackwards)
                {
                    CurrentFrameIndex--;
                    if (CurrentFrameIndex < AnimationInstance.StartingFrame)
                    {
                        isBackwards = !isBackwards;
                        CurrentFrameIndex = AnimationInstance.StartingFrame;
                    }
                }
                else
                {
                    CurrentFrameIndex++;
                    if (CurrentFrameIndex > AnimationInstance.EndingFrame)
                    {
                        isBackwards = !isBackwards;
                        CurrentFrameIndex = AnimationInstance.EndingFrame;
                        AnimationEnded = true;
                    }
                }

                timePast = 0;
            }

            return CurrentFrameIndex;
        }
    }
}
