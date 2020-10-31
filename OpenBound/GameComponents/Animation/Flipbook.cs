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
using OpenBound.GameComponents.Asset;
using System;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Animation
{
    public class Flipbook : Renderable
    {
        private List<AnimationInstance> AnimationCycle { get; set; }

        private int currentAnimatedInstanceIndex;
        private bool repeatAnimationCycle;
        private int framesPerLine;

        public List<FlipbookAnimation> FlipbookAnimationList { get; private set; }

        public AnimationInstance CurrentAnimationInstance => FlipbookAnimationList[currentAnimatedInstanceIndex].AnimationInstance;

        public Flipbook(Vector2 position, Vector2 pivot,
            int spriteWidth, int SpriteHeight, string texture2DPath,
            List<AnimationInstance> animationCycle,
            float layerDepth, float rotation = 0) : base()
        {
            CreateFlipbook(this, position, pivot,
            spriteWidth, SpriteHeight, texture2DPath,
            animationCycle, layerDepth, rotation);
        }

        public Flipbook(Vector2 position, Vector2 pivot,
            int spriteWidth, int spriteHeight, string texture2DPath,
            AnimationInstance animationInstance,
            float layerDepth, float rotation = 0) : base()
        {
            List<AnimationInstance> aiL = new List<AnimationInstance>();
            aiL.Add(animationInstance);

            CreateFlipbook(this, position, pivot,
            spriteWidth, spriteHeight, texture2DPath,
            aiL, layerDepth, rotation);
        }

        private static void CreateFlipbook(Flipbook flipbook,
            Vector2 Position, Vector2 Pivot,
            int SpriteWidth, int SpriteHeight, string Texture2DPath,
            List<AnimationInstance> AnimationCycle,
            float LayerDepth, float Rotation = 0)
        {
            flipbook.SpriteWidth = SpriteWidth;
            flipbook.SpriteHeight = SpriteHeight;
            flipbook.Texture2DPath = Texture2DPath;
            flipbook.Rotation = Rotation;
            flipbook.AnimationCycle = AnimationCycle;

            flipbook.Position = Position;
            flipbook.Pivot = Pivot;

            flipbook.currentAnimatedInstanceIndex = 0;
            flipbook.repeatAnimationCycle = false;

            flipbook.LayerDepth = LayerDepth;
            flipbook.Effect = SpriteEffects.None;

            flipbook.Texture2D = AssetHandler.Instance.RequestTexture(Texture2DPath);

            flipbook.FlipbookAnimationList = new List<FlipbookAnimation>();

            flipbook.CreateFlipbookInstances();

            flipbook.framesPerLine = flipbook.Texture2D.Width / SpriteWidth;
        }

        public void JumpToRandomAnimationFrame()
        {
            int nextFrameIndex = Parameter.Random.Next(CurrentAnimationInstance.StartingFrame, CurrentAnimationInstance.EndingFrame);
            SetCurrentFrame(nextFrameIndex);
        }

        public void Flip()
        {
            Effect = (Effect == SpriteEffects.None) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Pivot = new Vector2(SpriteWidth - Pivot.X, Pivot.Y);
        }

        private void CreateFlipbookInstances()
        {
            FlipbookAnimationList.Clear();
            foreach (AnimationInstance fbI in AnimationCycle)
                FlipbookAnimationList.Add(FlipbookAnimation.GetInstance(fbI));
        }

        public void AppendAnimationIntoCycle(AnimationInstance newAnimation, bool clearAnimationCycle = false)
        {
            if (clearAnimationCycle) AnimationCycle.Clear();
            AnimationCycle.Add(newAnimation);
            CreateFlipbookInstances();
        }

        public void AppendAnimationIntoCycle(List<AnimationInstance> newAnimationList, bool clearAnimationCycle = false)
        {
            if (clearAnimationCycle) AnimationCycle.Clear();
            AnimationCycle.AddRange(newAnimationList);
            CreateFlipbookInstances();
        }

        public void SetCurrentFrame(int newFrame) =>
            FlipbookAnimationList[currentAnimatedInstanceIndex].CurrentFrameIndex = newFrame;

        public int GetCurrentFrame() =>
            FlipbookAnimationList[currentAnimatedInstanceIndex].CurrentFrameIndex;

        public void ResetCurrentAnimation()
        {
            FlipbookAnimationList[currentAnimatedInstanceIndex].Reset();
        }

        public override void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            FlipbookAnimation fbA = FlipbookAnimationList[currentAnimatedInstanceIndex];

            int currentFrameIndex = fbA.GetNextAnimationIndex(GameTime);

            if (fbA.IsLastFrame && AnimationCycle != null && AnimationCycle.Count > 1)
            {
                if (!repeatAnimationCycle)
                {
                    FlipbookAnimationList.Remove(fbA);
                    AnimationCycle.Remove(fbA.AnimationInstance);
                }

                currentAnimatedInstanceIndex = 0;
            }

            SourceRectangle = new Rectangle(currentFrameIndex % framesPerLine * SpriteWidth, currentFrameIndex / framesPerLine * SpriteHeight, SpriteWidth, SpriteHeight);
            base.Draw(GameTime, SpriteBatch);
        }
    }
}
