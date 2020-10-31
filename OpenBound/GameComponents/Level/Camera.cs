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
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Debug;
using OpenBound.GameComponents.Interface;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Pawn;
using OpenBound.GameComponents.MobileAction;
using System;
using System.Collections.Generic;
using OpenBound.GameComponents.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace OpenBound.GameComponents.Level
{
    public enum CameraOperationMode
    {
        InGame,
        Menus,
    }

    public enum CameraSpecialEffect
    {
        FadeIn,
        FadeOut,
    }

    public class Camera
    {
        public Matrix Transform, BackgroundTransform;
        public Vector2 Zoom;
        public List<Renderable> TargetList { get; set; }

        private Vector2 cameraOffset;
        public Vector2 CameraOffset
        {
            get => cameraOffset;
            set => cameraOffset = new Vector2(
                MathHelper.Clamp(value.X, cameraMinPosition.X, cameraMaxPosition.X),
                MathHelper.Clamp(value.Y, cameraMinPosition.Y, cameraMaxPosition.Y));
        }

        private Cursor cursor;

        private Vector2 cameraMinPosition;
        private Vector2 cameraMaxPosition;

//        public Vector2 ScreenCenterPosition { get; private set; }
        public Vector2 CameraReachableRange { get; private set; }

        public Vector2 TransformedScreenCenterPosition => -new Vector2(Transform.M41, Transform.M42) + Parameter.ScreenCenter;

        public object TrackedObject { get; private set; }

#if DEBUG
        DebugLine debugLine = new DebugLine(Color.Black);
#endif

        #region Camera Special Effects
        Action<float> cameraSpecialEffectUpdateAction;
        float currentCameraFade;

        Sprite fadeSprite;

        HashSet<Sprite> spriteSet;
        #endregion

        public Camera(GameScene gameScene)
        {
            cursor = Cursor.Instance;
            TargetList = new List<Renderable>();
            Transform = Matrix.CreateTranslation(0, 0, 0);
            CameraOffset = new Vector2(0, 0);
            Zoom = new Vector2(1, 1);

#if DEBUG
            //Debug
            DebugCrosshair[] debugCrosshair = new DebugCrosshair[8];

            for (int i = 0; i < debugCrosshair.Length; i++)
                debugCrosshair[i] = new DebugCrosshair(Color.Red);

            debugCrosshair[0].Update(new Vector2(-900, -900));
            debugCrosshair[1].Update(new Vector2(-900, 900 - Parameter.ScreenResolution.Y));
            debugCrosshair[2].Update(new Vector2(900 - Parameter.ScreenResolution.X, -900));
            debugCrosshair[3].Update(new Vector2(900 - Parameter.ScreenResolution.X, 900 - Parameter.ScreenResolution.Y));

            debugCrosshair[4].Update(new Vector2(0, 0) - new Vector2(900, 900));
            debugCrosshair[5].Update(new Vector2(0, 1800) - new Vector2(900, 900));
            debugCrosshair[6].Update(new Vector2(1800, 0) - new Vector2(900, 900));
            debugCrosshair[7].Update(new Vector2(1800, 1800) - new Vector2(900, 900));

            DebugHandler.Instance.AddRange(debugCrosshair);

            DebugHandler.Instance.Add(debugLine);
#endif
            AdjustAttackParameters(gameScene);

            #region Camera Special Effects
            spriteSet = new HashSet<Sprite>();

            fadeSprite = new Sprite("Misc/Dummy", Color.Black,
                sourceRectangle: new Rectangle(0, 0, 1, 1),
                layerDepth: DepthParameter.CameraFade)
            {
                Pivot = new Vector2(0.5f, 0.5f),
                Scale = Parameter.ScreenResolution * 2,
                Color = Color.Black,
                BaseColor = Color.Black,
            };
            #endregion
        }

        public void AdjustAttackParameters(GameScene scene)
        {
            //If Camera is InGame, the Min & Max positions should be set using the foreground
            float newMinX, newMinY, newMaxX, newMaxY;

            if (GameInformation.Instance.GameState == GameState.InGame)
            {
                int maxSupportedResWidth  = (int)Parameter.InGameSupportedResolution.X;
                int maxSupportedResHeight = (int)Parameter.InGameSupportedResolution.Y;

                int fgSpriteWidth  = ((LevelScene)scene).Foreground.SpriteWidth;
                int fgSpriteHeight = ((LevelScene)scene).Foreground.SpriteHeight;
                
                float gameScaleFactor = Math.Max(
                    Parameter.ScreenResolution.X / maxSupportedResWidth,
                    Parameter.ScreenResolution.Y / maxSupportedResHeight);

                Zoom = Vector2.One * gameScaleFactor;

                newMinX = -Math.Abs(Parameter.ScreenCenter.X / Zoom.X - fgSpriteWidth / 2);
                newMinY = -Math.Abs(Parameter.ScreenCenter.Y / Zoom.Y - fgSpriteHeight / 2);

                if (maxSupportedResWidth > fgSpriteWidth)
                    newMinX = 0;

                newMaxX = -newMinX;
                newMaxY = -newMinY;

                //If the resolution is bigger than BG, create another scale only for BG drawings
                int bgSpriteWidth = ((LevelScene)scene).Background.SpriteWidth;
                int bgSpriteHeight = ((LevelScene)scene).Background.SpriteHeight;

                if (Parameter.ShouldChangeBGScale = 
                    !(bgSpriteWidth > Parameter.ScreenResolution.X &&
                    bgSpriteHeight > Parameter.ScreenResolution.Y))
                {

                    float bgScaleFactor = Math.Max(
                        Parameter.ScreenResolution.X / bgSpriteWidth,
                        Parameter.ScreenResolution.Y / bgSpriteHeight);

                    Vector2 BackgroundZoom = Vector2.One * bgScaleFactor;

                    BackgroundTransform = Matrix.CreateTranslation(Vector3.Zero) *
                        Matrix.CreateScale(new Vector3(BackgroundZoom.X, BackgroundZoom.Y, 1)) *
                        Matrix.CreateTranslation(new Vector3(Parameter.ScreenCenter.X, Parameter.ScreenCenter.Y, 0));
                }

                //Starting camera offset
                cameraOffset = Vector2.Zero;
            }
            else
            {
                newMinX = newMaxX = -Parameter.ScreenCenter.X;
                newMinY = newMaxY = -Parameter.ScreenCenter.Y;

                Zoom = new Vector2(
                    Parameter.ScreenResolution.X /
                    Parameter.MenuSupportedResolution.X,
                    Parameter.ScreenResolution.Y /
                    Parameter.MenuSupportedResolution.Y);

                Zoom = Vector2.One * Math.Min(Zoom.X, Zoom.Y);
            }

            cameraMinPosition = new Vector2(newMinX, newMinY);
            cameraMaxPosition = new Vector2(newMaxX, newMaxY);

            //This is used to calculate the stage parallax
            CameraReachableRange = cameraMaxPosition - cameraMinPosition;
        }

        public void Update(GameTime gameTime)
        {
            //Update changes CameraOffset, must be called before the matrix calculation
            cursor.Update(this);

            UpdateTrackedObject();

            Transform =
                Matrix.CreateTranslation(new Vector3((int)CameraOffset.X, (int)CameraOffset.Y, 0)) *
                Matrix.CreateScale(new Vector3(Zoom.X, Zoom.Y, 1)) *
                Matrix.CreateTranslation(new Vector3(Parameter.ScreenCenter.X, Parameter.ScreenCenter.Y, 0));

            cameraSpecialEffectUpdateAction?.Invoke((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Sprite s in spriteSet) { s.Draw(null, spriteBatch); }
        }

        public void TrackObject(object trackedObject)
        {
            TrackedObject = trackedObject;
#if DEBUG
            debugLine.Sprite.ShowElement();
#endif
        }

        public void UpdateTrackedObject()
        {
            if (TrackedObject == null) return;

            Vector2 objectPosition = Vector2.Zero;

            if (TrackedObject is Projectile)
                objectPosition = ((Projectile)TrackedObject).Position;

            if (TrackedObject is Mobile)
                objectPosition = ((Mobile)TrackedObject).Position;

            Vector2 screenCenter = TransformedScreenCenterPosition;

            float cameraSpeed = (float)Helper.EuclideanDistance(screenCenter, objectPosition);
            double ang = Helper.AngleBetween(TransformedScreenCenterPosition, objectPosition);

#if DEBUG
            debugLine.Update(TransformedScreenCenterPosition, objectPosition);
#endif

            float factor = cameraSpeed / Parameter.CameraTrackingDampeningFactor;
            CameraOffset += new Vector2((int)(Math.Cos(ang) * factor), (int)(Math.Sin(ang) * factor));
        }

        public void CancelTracking()
        {
            TrackedObject = null;

#if DEBUG
            debugLine.Sprite.HideElement();
#endif
        }

        /// <summary>
        /// Returns a tuple with opposite screen effects that, by definition, can not overlap. The first item is the special effect
        /// that should be removed from the updating action. The second item is the one that will be inserted into.
        /// <see cref="cameraSpecialEffectUpdateAction"/>.
        /// </summary>
        private (Action<float>, Action<float>) BuildCameraEffectAction(CameraSpecialEffect cameraSpecialEffect) =>
            cameraSpecialEffect switch
            {
                CameraSpecialEffect.FadeIn  => (FadeOut, FadeIn),
                CameraSpecialEffect.FadeOut => (FadeIn, FadeOut),
                _ => default
            };

        public void ApplyCameraEffect(CameraSpecialEffect cameraSpecialEffect)
        {
            cameraSpecialEffectUpdateAction = default;
            (Action<float>, Action<float>) oppositeActions = BuildCameraEffectAction(cameraSpecialEffect);
            cameraSpecialEffectUpdateAction -= oppositeActions.Item1;
            cameraSpecialEffectUpdateAction += oppositeActions.Item2;
        }
        
        private void FadeOut(float elapsedTime)
        {
            if (currentCameraFade == 0)
            {
                spriteSet.Add(fadeSprite);
            }

            currentCameraFade = MathHelper.Clamp(
                currentCameraFade + elapsedTime * Parameter.CameraFadeSpeedFactor,  0,
                Parameter.CameraMinimumFadeOutTransparency);

            fadeSprite.SetTransparency(currentCameraFade);

            fadeSprite.Position = -CameraOffset;
        }

        private void FadeIn(float elapsedTime)
        {
            currentCameraFade = MathHelper.Clamp(currentCameraFade - elapsedTime * Parameter.CameraFadeSpeedFactor,
                0, Parameter.CameraMinimumFadeOutTransparency);

            fadeSprite.SetTransparency(currentCameraFade);

            fadeSprite.Position = -CameraOffset;

            //Self destruction
            if (currentCameraFade == 0)
            {
                spriteSet.Add(fadeSprite);
                cameraSpecialEffectUpdateAction -= FadeIn;
            }
        }
    }
}
