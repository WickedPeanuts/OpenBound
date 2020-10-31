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
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Interface;
using OpenBound.GameComponents.Pawn.UnitProjectiles;
using OpenBound.GameComponents.MobileAction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound.GameComponents.Pawn
{
    public class ThorSatellite
    {
        static Dictionary<ActorFlipbookState, AnimationInstance> thorStatePresets = new Dictionary<ActorFlipbookState, AnimationInstance>()
        {
            { ActorFlipbookState.Stand,      new AnimationInstance(){ StartingFrame = 00, EndingFrame = 19, TimePerFrame = 1/15f } },
            { ActorFlipbookState.Active,     new AnimationInstance(){ StartingFrame = 20, EndingFrame = 39, TimePerFrame = 1/15f } },
            { ActorFlipbookState.ShootingS1, new AnimationInstance(){ StartingFrame = 40, EndingFrame = 55, TimePerFrame = 1/15f } },
        };

        Flipbook flipbook;
        Vector2 cannonOffset;

        //Movement Animation
        public Vector2 Position => position + positionMovement;

        Vector2 position, positionMovement;
        float movementElapsedTime;

        //Cannon position
        Vector2 cannonPosition;

        //Rotation Movement

        /// <summary>
        /// Thor's target list. The very first element is focused by thor rotating animation.
        /// If this list is empty, thor faces <see cref="lastFocusedPosition"/> position.
        /// </summary>
        List<Projectile> targetList;
        Vector2 lastFocusedPosition;

        //Oscilation movement
        Vector2 oscilatingPositionOffset;
        float oscilatingElapsedTime1, oscilatingElapsedTime2;

        //Numeric fields
        Sprite levelText;
        NumericSpriteFont levelSpriteFont;
        CurrencySpriteFont experienceSpriteFont;

        //Cannon Projectile interpolated color
        Color beamColor;

        //Thor status
        public int Level => (int)levelSpriteFont.CurrentValue;
        public int CurrentExperience => experienceSpriteFont.FinalValue;

        public ThorSatellite()
        {
            targetList = new List<Projectile>();

            beamColor = Parameter.NeonGreen;

            oscilatingPositionOffset = new Vector2(3, 0);
            cannonOffset = new Vector2(50, 0);

            position = new Vector2(0, -500);

            flipbook = new Flipbook(position, new Vector2(118, 111), 197, 190, "Graphics/Entity/Thor/Spritesheet", thorStatePresets[ActorFlipbookState.Stand], DepthParameter.Mobile, MathHelper.PiOver2);

            levelSpriteFont = new NumericSpriteFont(FontType.HUDBlueThorLevelIndicator, 1, DepthParameter.MobileSatellite, textAnchor: TextAnchor.Right, attachToCamera: false, StartingValue: 1);
            experienceSpriteFont = new CurrencySpriteFont(FontType.HUDBlueThorExperienceIndicator, 5, DepthParameter.MobileSatellite, textAnchor: TextAnchor.Right, attachToCamera: false);

            levelText = new Sprite("Interface/Spritefont/HUD/Blue/ThorLevelLV", Vector2.Zero, layerDepth: DepthParameter.MobileSatellite);
            levelText.Pivot = Vector2.Zero;

            lastFocusedPosition = Vector2.Zero;
        }

        /// <summary>
        /// Play the Shot animation and spawn a beam on the targetting position.
        /// </summary>
        /// <param name="position"></param>
        public void Shot(Vector2 position)
        {
            flipbook.AppendAnimationIntoCycle(thorStatePresets[ActorFlipbookState.ShootingS1], true);
            flipbook.AppendAnimationIntoCycle(thorStatePresets[ActorFlipbookState.Active]);

            SpecialEffectBuilder.ThorShot((cannonPosition + position) / 2, beamColor, (float)Helper.EuclideanDistance(cannonPosition, position) / 256, (float)Helper.AngleBetween(cannonPosition, position) - MathHelper.PiOver2);
        }

        /// <summary>
        /// Change thor's flipbook to it's activated animation.
        /// </summary>
        public void Activate()
        {
            flipbook.AppendAnimationIntoCycle(thorStatePresets[ActorFlipbookState.Active], true);
        }

        /// <summary>
        /// Change thor's flipbook to it's deactivated animation.
        /// </summary>
        public void Deactivate()
        {
            flipbook.AppendAnimationIntoCycle(thorStatePresets[ActorFlipbookState.Stand], true);
        }

        public void Update(GameTime gameTime)
        {
            UpdatePosition(gameTime);
        }

        public void SetPosition(Vector2 newPosition)
        {
            positionMovement = newPosition - position;
            movementElapsedTime = 0;
        }

        public void GainExperience(int damageDealt)
        {
            if (damageDealt == 0) return;

            experienceSpriteFont.AddValue(damageDealt);

            for (int i = 0; i < Parameter.ThorExperienceTable.Count(); i++)
            {
                if (experienceSpriteFont.FinalValue > Parameter.ThorExperienceTable[i] && i + 1 != Parameter.ThorExperienceTable.Count()) continue;

                if (experienceSpriteFont.FinalValue >= Parameter.ThorExperienceTable.Last())
                    levelSpriteFont.UpdateValue(i + 2);
                else
                    levelSpriteFont.UpdateValue(i + 1);

                int extraColor = i + 1;

                if (extraColor == Parameter.ThorExperienceTable.Count())
                    extraColor = i;

                //Interpolating color values
                float ciV1 = experienceSpriteFont.FinalValue - damageDealt;
                float ciV2 = Parameter.ThorExperienceTable[i];

                if (i != 0)
                {
                    ciV1 = Math.Max(0, ciV1 - Parameter.ThorExperienceTable[i - 1]);
                    ciV2 -= Parameter.ThorExperienceTable[i - 1];
                }

                beamColor = Color.Lerp(
                    Parameter.ColorGradient[i],
                    Parameter.ColorGradient[extraColor],
                    ciV1 / ciV2);
                break;
            }
        }

        /// <summary>
        /// Attatches thor effects to a projectile.
        /// </summary>
        /// <param name="projectile"></param>
        public void Attatch(Projectile projectile)
        {
            targetList.Add(projectile);

            projectile.OnExplodeAction += () =>
            {
                targetList.Remove(projectile);

                ThorProjectile tp = new ThorProjectile(
                    projectile.Mobile,
                    this,
                    projectile.Position,
                    -(float)Helper.AngleBetween(Position, projectile.Position));

                tp.Update();

                lastFocusedPosition = projectile.Position;
            };
        }

        private void UpdatePosition(GameTime gameTime)
        {
            //Updating oscilating movement
            oscilatingElapsedTime1 += (float)gameTime.ElapsedGameTime.TotalSeconds * MathHelper.Pi / 4;
            oscilatingElapsedTime2 = oscilatingElapsedTime1 / 2;

            MathHelper.WrapAngle(oscilatingElapsedTime1);

            Vector2 tmpOscilatingPosition = Vector2.Transform(oscilatingPositionOffset, Matrix.CreateRotationZ(oscilatingElapsedTime1 / MathHelper.Pi)) * new Vector2((float)Math.Sin(oscilatingElapsedTime1), (float)Math.Cos(oscilatingElapsedTime2));

            //Update movement position
            Vector2 tmpPositionOffset = Vector2.Zero;
            if (positionMovement != Vector2.Zero)
            {
                movementElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds * MathHelper.PiOver2;

                tmpPositionOffset = positionMovement * (float)Math.Sin(movementElapsedTime);

                if (movementElapsedTime >= MathHelper.PiOver2)
                {
                    position += positionMovement;
                    positionMovement = Vector2.Zero;
                    flipbook.Position = position + tmpOscilatingPosition;
                    return;
                }
            }

            flipbook.Position = position + tmpOscilatingPosition + tmpPositionOffset;

            //Update rotation
            Vector2 facingPosition = targetList.Count == 0 ? lastFocusedPosition : targetList[0].Position;
            flipbook.Rotation = (float)Helper.AngleBetween(facingPosition, flipbook.Position);

            cannonPosition = Position +
                Vector2.Transform(
                   cannonOffset,
                   Matrix.CreateRotationZ(
                       (float)Helper.AngleBetween(
                           facingPosition,
                           Position)));

            //Update texts
            levelSpriteFont.Position = flipbook.Position - tmpOscilatingPosition + new Vector2(70, 40);
            experienceSpriteFont.Position = levelSpriteFont.Position + new Vector2(0, 15);
            levelText.Position = levelSpriteFont.Position - new Vector2(30, -1);
            levelSpriteFont.Update(gameTime);
            experienceSpriteFont.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            flipbook.Draw(gameTime, spriteBatch);
            levelSpriteFont.Draw(gameTime, spriteBatch);
            experienceSpriteFont.Draw(gameTime, spriteBatch);
            levelText.Draw(null, spriteBatch);
        }
    }
}
