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
using Microsoft.Xna.Framework.Input;
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Debug;
using OpenBound.GameComponents.Input;
using OpenBound.GameComponents.Pawn;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Sync;
using System;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Interface
{
    public class CrosshairPreset
    {
        public float CannonOffsetDistance;
        public float CannonOffsetRotation;
        public float CrosshairPointerOffset;

        public CrosshairPreset Clone()
        {
            return new CrosshairPreset()
            {
                CannonOffsetDistance = CannonOffsetDistance,
                CannonOffsetRotation = CannonOffsetRotation,
                CrosshairPointerOffset = CrosshairPointerOffset
            };
        }
    }

    public enum CrosshairAnimationState
    {
        Start,
        Animating,
        End,
    }

    public class Crosshair
    {
        //Crosshair Presets, this variable contains all the information about all mobile crosshairs and supported angles,
        static readonly Dictionary<MobileType, Dictionary<ShotType, CrosshairPreset>> crosshairPresets
            = new Dictionary<MobileType, Dictionary<ShotType, CrosshairPreset>>()
            {
                #region Armor
                {
                    MobileType.Armor,
                    new Dictionary<ShotType, CrosshairPreset>()
                    {
                        {
                            ShotType.S1,
                            new CrosshairPreset() { CannonOffsetDistance = -24, CannonOffsetRotation = MathHelper.ToRadians(20), CrosshairPointerOffset = 29f }
                        },
                    }
                },
                #endregion
                #region Bigfoot
                {
                    MobileType.Bigfoot,
                    new Dictionary<ShotType, CrosshairPreset>()
                    {
                        {
                            ShotType.S1,
                            new CrosshairPreset() { CannonOffsetDistance = -28, CannonOffsetRotation = MathHelper.ToRadians(20), CrosshairPointerOffset = 29 }
                        }
                    }
                },
                #endregion
                #region Dragon
                {
                    MobileType.Dragon,
                    new Dictionary<ShotType, CrosshairPreset>()
                    {
                        {
                            ShotType.S1,
                            new CrosshairPreset() { CannonOffsetDistance = -30, CannonOffsetRotation = MathHelper.ToRadians(20), CrosshairPointerOffset = 29 }
                        }
                    }
                },
                #endregion
                #region Mage
                {
                    MobileType.Mage,
                    new Dictionary<ShotType, CrosshairPreset>()
                    {
                        {
                            ShotType.S1,
                            new CrosshairPreset() { CannonOffsetDistance = -24, CannonOffsetRotation = MathHelper.ToRadians(20), CrosshairPointerOffset = 29f }
                        },
                    }
                },
                #endregion
                #region Ice
                {
                    MobileType.Ice,
                    new Dictionary<ShotType, CrosshairPreset>()
                    {
                        {
                            ShotType.S1,
                            new CrosshairPreset() { CannonOffsetDistance = -30, CannonOffsetRotation = MathHelper.ToRadians(35), CrosshairPointerOffset = 29 }
                        },
                        {
                            ShotType.S2,
                            new CrosshairPreset() { CannonOffsetDistance = -28, CannonOffsetRotation = MathHelper.ToRadians(10), CrosshairPointerOffset = 29 }
                        },
                        {
                            ShotType.SS,
                            new CrosshairPreset() { CannonOffsetDistance = -34, CannonOffsetRotation = MathHelper.ToRadians(35), CrosshairPointerOffset = 29 }
                        },
                    }
                },
                #endregion
                #region Knight
                {
                    MobileType.Knight,
                    new Dictionary<ShotType, CrosshairPreset>()
                    {
                        {
                            ShotType.S1,
                            new CrosshairPreset() { CannonOffsetDistance = -26, CannonOffsetRotation = MathHelper.ToRadians(35), CrosshairPointerOffset = 29 }
                        }
                    }
                },
                #endregion
                #region Trico
                {
                    MobileType.Trico,
                    new Dictionary<ShotType, CrosshairPreset>()
                    {
                        {
                            ShotType.S1,
                            new CrosshairPreset() { CannonOffsetDistance = -28, CannonOffsetRotation = MathHelper.ToRadians(40), CrosshairPointerOffset = 29f }
                        },
                    }
                },
                #endregion
                #region Raon
                {
                    MobileType.RaonLauncher,
                    new Dictionary<ShotType, CrosshairPreset>()
                    {
                        {
                            ShotType.S1,
                            new CrosshairPreset() { CannonOffsetDistance = -29, CannonOffsetRotation = MathHelper.ToRadians(35), CrosshairPointerOffset = 29f }
                        },
                    }
                },
                #endregion
                #region Turtle
                {
                    MobileType.Turtle,
                    new Dictionary<ShotType, CrosshairPreset>()
                    {
                        {
                            ShotType.S1,
                            new CrosshairPreset() { CannonOffsetDistance = -24, CannonOffsetRotation = MathHelper.ToRadians(20), CrosshairPointerOffset = 29f }
                        },
                    }
                },
                #endregion
                #region Lightning
                {
                    MobileType.Lightning,
                    new Dictionary<ShotType, CrosshairPreset>()
                    {
                        {
                            ShotType.S1,
                            new CrosshairPreset() { CannonOffsetDistance = -25, CannonOffsetRotation = MathHelper.ToRadians(30), CrosshairPointerOffset = 29f }
                        },
                    }
                },
                #endregion
            };

        public Vector2 CannonPosition { get; private set; }

        //Crosshair
        private CrosshairPreset crosshairPreset;
        private AimPreset aimPreset;
        private Sprite crosshairFrame;

        private List<Sprite> crosshairRangeIndicatorList;
        private Sprite selectedCrosshairRangeIndicator;

        public Sprite CrosshairPointer;
        public List<NumericSpriteFont> crosshairAngleList;
        public NumericSpriteFont selectedCrosshairAngle;

        public int ShootingAngle { get; private set; }
        private float crosshairDesiredRotation;

        //Crosshair Turning Speed
        private float timePast;
        private float timeLimit;

        //Crosshair turn animation
        private CrosshairAnimationState crosshairAnimationState;
        private EventHandler animationEventHandler;
        private float animationElapsedTime;

        //Crosshair Selected Angle
        public int HUDSelectedAngle { get; private set; }


        //Mobile Reference
        protected Mobile mobile { get; }

#if DEBUG
        //DEBUG
        private DebugCrosshair debugCrosshair1;
        private DebugCrosshair debugCrosshair2;
#endif

        public bool IsTrueAngle => ShootingAngle >= aimPreset.AimTrueRotationMin && ShootingAngle <= aimPreset.AimTrueRotationMax;

        /// <summary>
        /// Crosshair must be initialized inside the Concrete Mobile class after its name is set.
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="isAlly"></param>
        public Crosshair(Mobile mobile)
        {
            //Save References
            this.mobile = mobile;

            //Initialize Crosshair/Pointer Sprites
            crosshairFrame = new Sprite($"Interface/InGame/HUD/Crosshair/CrosshairFrame",
                new Vector2(0, 0), DepthParameter.CrosshairFrame);

            crosshairRangeIndicatorList = new List<Sprite>()
            {
                new Sprite($"Interface/InGame/HUD/Crosshair/{mobile.MobileType}{ShotType.S1}" + (mobile.IsEnemy ? "Enemy":"Ally"),
                new Vector2(0, 0), DepthParameter.CrosshairAimRangeIndicator),
                new Sprite($"Interface/InGame/HUD/Crosshair/{mobile.MobileType}{ShotType.S2}" + (mobile.IsEnemy ? "Enemy":"Ally"),
                new Vector2(0, 0), DepthParameter.CrosshairAimRangeIndicator),
                new Sprite($"Interface/InGame/HUD/Crosshair/{mobile.MobileType}{ShotType.SS}" + (mobile.IsEnemy ? "Enemy":"Ally"),
                new Vector2(0, 0), DepthParameter.CrosshairAimRangeIndicator),
            };
            selectedCrosshairRangeIndicator = crosshairRangeIndicatorList[0];

            CrosshairPointer = new Sprite("Interface/InGame/HUD/Crosshair/Pointer",
               new Vector2(0, 0), DepthParameter.CrosshairPointer);

            //Initialize Crosshair Components
            crosshairPreset = crosshairPresets[mobile.MobileType][mobile.SelectedShotType].Clone();
            crosshairDesiredRotation = 0f;

            if (mobile.IsPlayable)
            {
                crosshairAngleList = new List<NumericSpriteFont>(){
                    new NumericSpriteFont(FontType.HUDBlueCrosshairTrueAngle, 3, Parameter.HUDCrosshairAngleIndicator, textAnchor: TextAnchor.Middle, attachToCamera: false),
                    new NumericSpriteFont(FontType.HUDBlueCrosshairFalseAngle, 3, Parameter.HUDCrosshairAngleIndicator, textAnchor: TextAnchor.Middle, attachToCamera: false)
                };

                selectedCrosshairAngle = crosshairAngleList[0];
            }

            //Initialize Shooting Angle
            aimPreset = mobile.MobileMetadata.MobileAimPreset[ShotType.S1];
            ShootingAngle = (aimPreset.AimTrueRotationMin + aimPreset.AimTrueRotationMax) / 2;

            CrosshairPointer.Rotation = crosshairFrame.Rotation + MathHelper.ToRadians(270 + ShootingAngle);

#if DEBUG
            //DEBUG
            debugCrosshair1 = new DebugCrosshair(Color.HotPink);
            debugCrosshair2 = new DebugCrosshair(Color.DarkTurquoise);
            DebugHandler.Instance.Add(debugCrosshair1);
            DebugHandler.Instance.Add(debugCrosshair2);
#endif
            if (mobile.IsPlayable)
                FadeElement();
            else
                HideElement();
        }

        public void HideElement()
        {
            crosshairFrame.HideElement();
            CrosshairPointer.HideElement();
            crosshairRangeIndicatorList.ForEach((x) => x.HideElement());
            crosshairAngleList?.ForEach((x) => x.HideElement());
            animationEventHandler -= Animation;
        }

        public void FadeElement()
        {
            crosshairFrame.Color = CrosshairPointer.Color = Parameter.HUDCrosshairFadeColor;
        }

        public void PlayAnimation()
        {
            crosshairAnimationState = CrosshairAnimationState.Start;
            animationEventHandler += Animation;
        }

        public void Animation(object sender, EventArgs e)
        {
            switch (crosshairAnimationState)
            {
                case CrosshairAnimationState.Start:
                    crosshairFrame.ShowElement();
                    CrosshairPointer.HideElement();
                    crosshairRangeIndicatorList.ForEach((x) => x.HideElement());
                    crosshairAngleList?.ForEach((x) => x.HideElement());
                    crosshairFrame.Scale = Parameter.HUDCrosshairAnimationStartingScale;
                    animationElapsedTime = 0;
                    crosshairAnimationState = CrosshairAnimationState.Animating;
                    break;
                case CrosshairAnimationState.Animating:
                    crosshairFrame.Scale = Parameter.HUDCrosshairAnimationStartingScale - (Parameter.HUDCrosshairAnimationStartingScale - new Vector2(1, 1)) * (animationElapsedTime / Parameter.HUDCrosshairAnimationTotalTime);
                    animationElapsedTime += (float)((GameTime)sender).ElapsedGameTime.TotalSeconds;
                    if (animationElapsedTime > Parameter.HUDCrosshairAnimationTotalTime)
                    {
                        crosshairFrame.Scale = CrosshairPointer.Scale = new Vector2(1f, 1f);
                        crosshairAnimationState = CrosshairAnimationState.End;
                    }
                    break;
                case CrosshairAnimationState.End:
                    animationEventHandler -= Animation;
                    CrosshairPointer.ShowElement();
                    crosshairRangeIndicatorList.ForEach((x) => x.ShowElement());
                    crosshairAngleList?.ForEach((x) => x.ShowElement());
                    break;
            }
        }

        public float GetProjectileTrajetoryAngle()
        {
            return MathHelper.ToRadians((int)MathHelper.ToDegrees(CrosshairPointer.Rotation) - 90);
        }

        public void UpdateShootingAngle(GameTime gameTime)
        {
            //Crosshair Pointer Rotator
            //For Turning Up
            if (InputHandler.IsBeingPressed(Keys.Up))
            {
                timeLimit = Parameter.HUDCrosshairInitialSensibility;
                ShootingAngle += 1;
            }
            else if (InputHandler.IsBeingHeldDown(Keys.Up))
            {
                if (timePast > timeLimit)
                {
                    timePast = 0;
                    ShootingAngle += 1;
                    timeLimit = Math.Max(timeLimit - Parameter.HUDCrosshairSensibilityReductionFactor, Parameter.HUDCrosshairMinimumSensibility);
                }
                else
                {
                    timePast += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
            else if (InputHandler.IsBeingReleased(Keys.Up))
            {
                timeLimit = Parameter.HUDCrosshairInitialSensibility;
                timePast = 0;
            }
            //For Turning Down
            else if (InputHandler.IsBeingPressed(Keys.Down))
            {
                timeLimit = Parameter.HUDCrosshairInitialSensibility;
                ShootingAngle -= 1;
            }
            else if (InputHandler.IsBeingHeldDown(Keys.Down))
            {
                if (timePast > timeLimit)
                {
                    timePast = 0;
                    ShootingAngle -= 1;
                    timeLimit = Math.Max(timeLimit - Parameter.HUDCrosshairSensibilityReductionFactor, Parameter.HUDCrosshairMinimumSensibility);
                }
                else
                {
                    timePast += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
            else if (InputHandler.IsBeingReleased(Keys.Down))
            {
                timeLimit = Parameter.HUDCrosshairInitialSensibility;
                timePast = 0;
            }
        }

        public void AutomaticUpdateShootingAngle(GameTime gameTime)
        {
            //Crosshair Pointer Rotator
            //For Turning Up
            if (mobile.SyncMobile == null) return;

            if (mobile.SyncMobile.CrosshairAngle == ShootingAngle)
            {
                timeLimit = Parameter.HUDCrosshairInitialSensibility;
                timePast = 0;
            }
            else
            {
                if (timePast > timeLimit)
                {
                    timePast = 0;
                    ShootingAngle += mobile.SyncMobile.CrosshairAngle - ShootingAngle > 0 ? 1 : -1;
                    timeLimit = Math.Max(timeLimit - Parameter.HUDCrosshairSensibilityReductionFactor, Parameter.HUDCrosshairMinimumSensibility);
                }
                else
                {
                    timePast += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }

        public void ChangeShot(ShotType shotType)
        {
            ChangeCrosshairPreset(shotType);

            if (shotType == ShotType.S1)
                selectedCrosshairRangeIndicator = crosshairRangeIndicatorList[0];
            else if (shotType == ShotType.S2)
                selectedCrosshairRangeIndicator = crosshairRangeIndicatorList[1];
            else if (shotType == ShotType.SS)
                selectedCrosshairRangeIndicator = crosshairRangeIndicatorList[2];

            aimPreset = mobile.MobileMetadata.MobileAimPreset[shotType];
        }

        public void Update(GameTime GameTime)
        {
#if DEBUG
            //DEBUG
            debugCrosshair1.Update(CannonPosition);
            debugCrosshair2.Update(CrosshairPointer.Position);
#endif

            //Calculating Cannon Position, Cannon Frame (Sprite) Position & Angle
            int newPosX = (int)(crosshairPreset.CannonOffsetDistance * Math.Cos(mobile.MobileFlipbook.Rotation + crosshairPreset.CannonOffsetRotation));
            int newPosY = (int)(crosshairPreset.CannonOffsetDistance * Math.Sin(mobile.MobileFlipbook.Rotation + crosshairPreset.CannonOffsetRotation));
            crosshairFrame.Position = CannonPosition = mobile.MobileFlipbook.Position + new Vector2(newPosX, newPosY);
            crosshairFrame.Rotation = mobile.MobileFlipbook.Rotation;

            //Update the shooting angle based on how much time up/down key has been pressed
            if (mobile.IsPlayable)
                UpdateShootingAngle(GameTime);
            else
                AutomaticUpdateShootingAngle(GameTime);

            // Force shooting angle to keep between the selected min and max
            ShootingAngle = MathHelper.Clamp(ShootingAngle, aimPreset.AimFalseRotationMin, aimPreset.AimFalseRotationMax);

            // Update the crosshair angle
            if (mobile.IsPlayable)
            {
                if (!IsTrueAngle)
                    selectedCrosshairAngle = crosshairAngleList[0];
                else
                    selectedCrosshairAngle = crosshairAngleList[1];
            }

            // Calculate new crosshair pointer depending on wich side the tank is facing
            if (mobile.Facing == Facing.Left)
            {
                CrosshairPointer.Rotation = crosshairFrame.Rotation + MathHelper.ToRadians(270 + ShootingAngle);
                newPosX = (int)(crosshairPreset.CrosshairPointerOffset * Math.Cos(crosshairDesiredRotation + MathHelper.ToRadians(ShootingAngle)));
                newPosY = (int)(crosshairPreset.CrosshairPointerOffset * Math.Sin(crosshairDesiredRotation + MathHelper.ToRadians(ShootingAngle)));

                //And calculate the Delay numeric field position
                if (mobile.IsPlayable)
                {
                    selectedCrosshairAngle.Position = crosshairFrame.Position + 30 * new Vector2(
                        (float)Math.Cos(crosshairFrame.Rotation + MathHelper.ToRadians(150)),
                        (float)Math.Sin(crosshairFrame.Rotation + MathHelper.ToRadians(150)));
                }
            }
            else
            {
                CrosshairPointer.Rotation = crosshairFrame.Rotation + MathHelper.ToRadians(90 - ShootingAngle);
                newPosX = (int)(crosshairPreset.CrosshairPointerOffset * Math.Cos(crosshairDesiredRotation - MathHelper.ToRadians(ShootingAngle)));
                newPosY = (int)(crosshairPreset.CrosshairPointerOffset * Math.Sin(crosshairDesiredRotation - MathHelper.ToRadians(ShootingAngle)));

                //And calculate the Delay numeric field position
                if (mobile.IsPlayable)
                {
                    selectedCrosshairAngle.Position = crosshairFrame.Position - 30 * new Vector2(
                        (float)Math.Cos(crosshairFrame.Rotation - MathHelper.ToRadians(150)),
                        (float)Math.Sin(crosshairFrame.Rotation - MathHelper.ToRadians(150)));
                }
            }

            CalculateHUDAngle();

            // Update the HUD angle text after defining its position
            selectedCrosshairAngle?.UpdateValue(HUDSelectedAngle);
            selectedCrosshairAngle?.Update();

            // Update the crosshair pointer location
            CrosshairPointer.Position = CannonPosition - new Vector2(newPosX, newPosY);

            // Update crosshair aim range indicators
            crosshairRangeIndicatorList.ForEach(
                (x) =>
                {
                    x.Position = crosshairFrame.Position;
                    x.Rotation = crosshairFrame.Rotation;
                });

            //Update Animations
            animationEventHandler?.Invoke(GameTime, null);
        }

        /// <summary>
        /// Updates the pointer angle rotation, this method needs to be called when the tank refreshes its rotation.
        /// newRotation must be the difference between the current mobile rotation and the new one.
        /// </summary>
        /// <param name="newRotation"></param>
        public void UpdateCrosshairPointerAngle(float newRotation)
        {
            crosshairDesiredRotation -= newRotation;
        }

        private void CalculateHUDAngle()
        {
            //Returns a more eye-friendly version of the shooting angle, this is used on the HUD
            HUDSelectedAngle = (int)MathHelper.ToDegrees(CrosshairPointer.Rotation);
            HUDSelectedAngle = (mobile.Facing == Facing.Left) ?
                    (HUDSelectedAngle - 270) : (90 - HUDSelectedAngle);
            if (HUDSelectedAngle > 90)
                HUDSelectedAngle = 90 - (HUDSelectedAngle % 90);
        }

        private void ChangeCrosshairPreset(ShotType shotType)
        {
            CrosshairPreset prevPreset = crosshairPreset;

            if (crosshairPresets[mobile.MobileType].ContainsKey(shotType))
                crosshairPreset = crosshairPresets[mobile.MobileType][shotType].Clone();
            else
                crosshairPreset = crosshairPresets[mobile.MobileType][ShotType.S1].Clone();

            //neg = neg * -1 * pos
            //neg = neg * -1 * neg
            //pos = pos * -1 * pos
            if (Math.Sign(prevPreset.CannonOffsetRotation) != Math.Sign(crosshairPreset.CannonOffsetRotation))
            {
                crosshairPreset.CannonOffsetDistance *= -1;
                crosshairPreset.CannonOffsetRotation *= -1;
            }
        }

        public void Flip()
        {
            crosshairFrame.Effect = (crosshairFrame.Effect == SpriteEffects.None) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            crosshairRangeIndicatorList.ForEach((x) => x.Effect = crosshairFrame.Effect);

            crosshairPreset.CannonOffsetRotation = -crosshairPreset.CannonOffsetRotation;
            crosshairPreset.CannonOffsetDistance = -crosshairPreset.CannonOffsetDistance;

            crosshairDesiredRotation += MathHelper.Pi;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            crosshairFrame.Draw(gameTime, spriteBatch);
            CrosshairPointer.Draw(gameTime, spriteBatch);
            selectedCrosshairRangeIndicator.Draw(gameTime, spriteBatch);
            selectedCrosshairAngle?.Draw(gameTime, spriteBatch);
        }
    }
}
