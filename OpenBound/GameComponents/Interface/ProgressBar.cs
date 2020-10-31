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
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Level.Scene;

namespace OpenBound.GameComponents.Interface
{
    public class ProgressBar
    {
        public Sprite BarSprite;
        public Vector2 PositionOffset;

        public float Intensity { get; set; }
        private float stepSize;

        private bool beginsAtZero;

        public bool IsFull { get { return Intensity >= 100; } }
        public bool IsEmpty { get { return Intensity <= 0; } }

        public Vector2 MeasureCurrentSize => new Vector2((int)(Intensity * BarSprite.SpriteWidth / 100), BarSprite.SpriteHeight);

        public ProgressBar(string SpritePath, float LayerDepth,
            float StepSize = 0, Vector2 BarOffset = default, bool BeginsAtZero = false)
        {
            //Shot Strenght Metter
            BarSprite = new Sprite(SpritePath, layerDepth: LayerDepth);
            beginsAtZero = BeginsAtZero;
            stepSize = StepSize;
            PositionOffset = BarOffset;
            Reset();
        }

        public void UpdateAttatchmentPosition()
        {
            BarSprite.Position = PositionOffset - GameScene.Camera.CameraOffset;
        }

        public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            BarSprite.Draw(GameTime, SpriteBatch);
        }

        public virtual void Reset()
        {
            Intensity = (beginsAtZero) ? 0 : 100;
            UpdateBar();
        }

        public void UpdateBar()
        {
            BarSprite.SourceRectangle = new Rectangle(0, 0, (int)(Intensity * BarSprite.SpriteWidth / 100), BarSprite.SpriteHeight);
        }

        public void PerformStep()
        {
            Intensity = MathHelper.Clamp(Intensity + stepSize, 0, 100);
            UpdateBar();
        }
    }

    public class TimedProgressBar : ProgressBar
    {
        private float timer;
        private float timeLimit;

        public TimedProgressBar(string SpritePath, float LayerDepth, float TimeLimit,
            float StepSize = 0, Vector2 BarOffset = default, bool BeginsAtZero = false)
            : base(SpritePath, LayerDepth, StepSize, BarOffset, BeginsAtZero)
        {
            timer = 0;
            timeLimit = TimeLimit;
        }

        public override void Reset()
        {
            base.Reset();
            timer = 0;
        }

        public void PerformStep(GameTime GameTime)
        {
            timer += (float)GameTime.ElapsedGameTime.TotalSeconds;
            if (timer > timeLimit)
            {
                timer = 0;
                PerformStep();
                UpdateBar();
            }
        }
    }
}
