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
using OpenBound_Network_Object_Library.Common;
using System;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Interface
{
    public class WindCompass
    {
        private readonly Sprite compassBackground, compassForeground;
        private readonly Sprite whiteArrow, yellowArrow, redArrow;
        private readonly List<Sprite> spriteList;

        //Y-Rotation Animation
        private float currentArrowYRotation;

        //Z-Rotation Animation
        private float _currentArrowZRotation;
        private float currentArrowZRotation { get => _currentArrowZRotation; set => _currentArrowZRotation = value % MathHelper.TwoPi; }

        private float desiredArrowZRotation;
        private float maximumDesiredRotation;
        private int rotationDirection;

        //Wind Strenght Variable
        private int windForce;

        //Arrow coloring
        private float currentYellowArrowAlpha, currentRedArrowAlpha;
        private float desiredYellowArrowAlpha, desiredRedArrowAlpha;
        private float yellowArrowSign, redArrowSign;

        //Fonts
        NumericSpriteFont numericSpriteFont;

        public WindCompass(Vector2 position)
        {
            compassBackground = new Sprite("Interface/InGame/HUD/Blue/WindCompass/Background", position, DepthParameter.HUDBackground);
            compassForeground = new Sprite("Interface/InGame/HUD/Blue/WindCompass/Foreground", position, DepthParameter.HUDForeground);

            whiteArrow = new Sprite("Interface/InGame/HUD/Blue/WindCompass/WhiteArrow", position, DepthParameter.HUDL1);
            yellowArrow = new Sprite("Interface/InGame/HUD/Blue/WindCompass/YellowArrow", position, DepthParameter.HUDL1 + 0.01f);
            redArrow = new Sprite("Interface/InGame/HUD/Blue/WindCompass/RedArrow", position, DepthParameter.HUDL1 + 0.02f);

            spriteList = new List<Sprite>() { compassBackground, compassForeground, whiteArrow, yellowArrow, redArrow };

            compassForeground.SetTransparency(0.8f);
            compassForeground.Rotation = whiteArrow.Rotation = redArrow.Rotation = yellowArrow.Rotation = desiredArrowZRotation = currentArrowZRotation = MathHelper.PiOver2 + MathHelper.Pi;

            yellowArrow.SetTransparency(0);
            redArrow.SetTransparency(0);

            numericSpriteFont = new NumericSpriteFont(FontType.HUDBlueWindCompass, 2, DepthParameter.HUDL1 + 0.03f, PositionOffset: position - new Vector2(0, 10),
                textAnchor: TextAnchor.Middle, StartingValue: 0, attachToCamera: true);
        }

        public void ChangeWind(int degrees, int windForce)
        {
            //Wind Strenght
            if (this.windForce == windForce && MathHelper.ToRadians(degrees) == desiredArrowZRotation) return;

            this.windForce = windForce;

            //Numeric List
            numericSpriteFont.UpdateValue(windForce);

            //Calculating Movement
            if (windForce > 0)
            {
                desiredArrowZRotation = MathHelper.ToRadians(degrees);

                float lR = (desiredArrowZRotation - currentArrowZRotation) % MathHelper.TwoPi;
                float rR = 0;

                if (lR < 0)
                    rR = lR + MathHelper.TwoPi;
                else if (lR > 0)
                    rR = lR - MathHelper.TwoPi;

                maximumDesiredRotation = Math.Abs(lR) < Math.Abs(rR) ? lR : rR;
                rotationDirection = Math.Sign(maximumDesiredRotation);
            }

            //Wind Strenght Compass Color

            //Yellow Arrow Alpha Scaling

            //Math.Abs(windStrenght - 17.5) = x
            //                         17.5 = 100;
            //Math.Abs(windStrenght - 17.5) * 100 = x * 17.5
            //Replacing 17.5 for NetworkObjectParameters.WeatherMaximumWindForce/2

            float halfWF = NetworkObjectParameters.WeatherMaximumWindForce / 2f;
            desiredYellowArrowAlpha = 1 - Math.Abs(windForce - halfWF) / halfWF;
            yellowArrowSign = Math.Sign(desiredYellowArrowAlpha - currentYellowArrowAlpha);

            //Red Arrow Alpha Scaling
            if (windForce > NetworkObjectParameters.WeatherMaximumWindForce / 2f)
                desiredRedArrowAlpha = 1 - desiredYellowArrowAlpha;
            else
                desiredRedArrowAlpha = 0;

            redArrowSign = Math.Sign(desiredRedArrowAlpha - currentRedArrowAlpha);
        }

        //Z-Rotating Animation whenever the wind changes
        public void UpdateCompassRotationAnimation(GameTime gameTime)
        {
            //Z-Rotating Animation
            if ((rotationDirection >= 0 && maximumDesiredRotation <= 0) || (rotationDirection <= 0 && maximumDesiredRotation >= 0))
            {
                currentArrowZRotation = desiredArrowZRotation;
            }
            else
            {
                float rotationFactor = rotationDirection * (float)gameTime.ElapsedGameTime.TotalSeconds * (1 + windForce * Parameter.HUDAnimationCompassWindForceFactor);

                maximumDesiredRotation -= rotationFactor;
                currentArrowZRotation += rotationFactor;
            }

            //Color Scaling Animation
            //Yellow Arrow

            if ((yellowArrowSign > 0 && desiredYellowArrowAlpha <= currentYellowArrowAlpha) || (yellowArrowSign < 0 && desiredYellowArrowAlpha >= currentYellowArrowAlpha))
                currentYellowArrowAlpha = desiredYellowArrowAlpha;
            else
                currentYellowArrowAlpha += yellowArrowSign * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if ((redArrowSign > 0 && desiredRedArrowAlpha <= currentRedArrowAlpha) || (redArrowSign < 0 && desiredRedArrowAlpha >= currentRedArrowAlpha))
                currentRedArrowAlpha = desiredRedArrowAlpha;
            else
                currentRedArrowAlpha += redArrowSign * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        //Y-Rotating Animation that simulates a 3D Object on the screen
        public void UpdateYRotatingAnimation(GameTime gameTime)
        {
            currentArrowYRotation += (float)(gameTime.ElapsedGameTime.TotalSeconds * (0.5 + windForce * Parameter.HUDAnimationCompassWindForceFactor)) % MathHelper.TwoPi;
            whiteArrow.Scale = yellowArrow.Scale = redArrow.Scale = new Vector2(1, (float)Math.Abs(Math.Cos(currentArrowYRotation)));
        }

        public void Update(GameTime gameTime)
        {
            whiteArrow.Rotation = yellowArrow.Rotation = redArrow.Rotation = compassForeground.Rotation = currentArrowZRotation;

            yellowArrow.SetTransparency(currentYellowArrowAlpha);
            redArrow.SetTransparency(currentRedArrowAlpha);

            UpdateYRotatingAnimation(gameTime);
            UpdateCompassRotationAnimation(gameTime);

            spriteList.ForEach((x) => x.UpdateAttatchmentPosition());

            numericSpriteFont.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteList.ForEach((x) => x.Draw(null, spriteBatch));
            numericSpriteFont.Draw(null, spriteBatch);
        }
    }
}
