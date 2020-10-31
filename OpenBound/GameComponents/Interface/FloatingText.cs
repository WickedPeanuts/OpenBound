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
using OpenBound.GameComponents.Debug;
using OpenBound.GameComponents.Pawn;
using OpenBound.GameComponents.Physics;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Interface
{
    public enum FloatingTextType
    {
        Damage,
    }

    public abstract class FloatingText
    {
        public SpriteNumericField SpriteNumericField { get; set; }// protected set; }


        protected float timespan;
        protected float initialTimeSpan;

        public bool IsExpired { get { return timespan <= 0; } }

        public FloatingText(float Timespan)
        {
            timespan = initialTimeSpan = Timespan;
        }

        public virtual void Update(GameTime GameTime)
        {
            SpriteNumericField.Update(GameTime);
        }
    }

    public class DamageFloatingText : FloatingText
    {
        public Actor Owner { get; private set; }

        DefinedAcceleratedMovement yMovement;
        float elapsedTime;

        public DamageFloatingText(Actor Owner) : base(3f)
        {
            this.Owner = Owner;

            SpriteNumericField = new CurrencySpriteFont(FontType.HUDBlueDamage, 5, 1f,
                position: ((Mobile)Owner).MobileFlipbook.Position - new Vector2(0, 15),
                positionOffset: ((Mobile)Owner).MobileFlipbook.Position - new Vector2(0, 15),
                textAnchor: TextAnchor.Middle,
                attachToCamera: false);

            yMovement = new DefinedAcceleratedMovement();
            yMovement.Preset(SpriteNumericField.PositionOffset.Y, SpriteNumericField.PositionOffset.Y - 40, -40, 0.6f);
        }

        public void AddValue(int Value)
        {
            timespan = initialTimeSpan;

            ((CurrencySpriteFont)SpriteNumericField).AddValue(Value);
        }

        public override void Update(GameTime GameTime)
        {
            elapsedTime = (float)GameTime.ElapsedGameTime.TotalSeconds;
            timespan -= elapsedTime;
            if (yMovement.IsMoving)
            {
                yMovement.RefreshCurrentPosition(elapsedTime);
                SpriteNumericField.PositionOffset = new Vector2(SpriteNumericField.Position.X, yMovement.CurrentPosition);
            }

            base.Update(GameTime);
        }
    }

    public class FloatingTextHandler
    {
        public List<DamageFloatingText> DamageFloatingTextList { get; set; }
        public List<DamageFloatingText> UnusedFloatingTextList { get; set; }

        public FloatingTextHandler()
        {
            DamageFloatingTextList = new List<DamageFloatingText>();
            UnusedFloatingTextList = new List<DamageFloatingText>();
        }

        public void Update(GameTime GameTime)
        {
            DamageFloatingTextList.ForEach((x) =>
            {
                x.Update(GameTime);

                if (x.IsExpired)
                    UnusedFloatingTextList.Add(x);
            });

            UnusedFloatingTextList.ForEach((x) => DamageFloatingTextList.Remove(x));
            UnusedFloatingTextList.Clear();
        }

        public void AddDamage(Actor Owner, int Value)
        {
            for (int i = 0; i < DamageFloatingTextList.Count; i++)
            {
                if (DamageFloatingTextList[i].Owner == Owner)
                {
                    DamageFloatingTextList[i].AddValue(Value);
                    return;
                }
            }

            DamageFloatingText floatingText = new DamageFloatingText(Owner);
            floatingText.AddValue(Value);
            DamageFloatingTextList.Add(floatingText);
        }

        public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            DamageFloatingTextList.ForEach((x) =>
            {
                x.SpriteNumericField.Draw(GameTime, SpriteBatch);
#if DEBUG
                DebugLine dL = new DebugLine(Color.Red);
                dL.Update(Cursor.Instance.CurrentFlipbook.Position, x.SpriteNumericField.Position);
                dL.Draw(SpriteBatch);
#endif
            });
        }
    }
}
