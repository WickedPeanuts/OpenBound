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
using OpenBound.Extension;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Pawn;
using System;

namespace OpenBound.GameComponents.Interface
{
    public class HealthBar
    {
        Sprite sprite;
        Color[][] healthBarColorMatrix;

        float currentHealthPercentage;
        float currentSheildPercentage;
        float healthBarSize;
        float totalBar;

        //Object reference
        public Mobile Mobile;

        public HealthBar(Mobile Mobile)
        {
            this.Mobile = Mobile;

            sprite = new Sprite("Interface/InGame/HUD/HealthBarFrame",
                layerDepth: DepthParameter.HealthBar,
                shouldCopyAsset: true);

            healthBarSize = sprite.SpriteWidth - 2;

            ExtractTextureInformation();

            totalBar = this.Mobile.MobileMetadata.BaseHealth + this.Mobile.MobileMetadata.BaseShield;

            UpdateBarSprite();
        }

        public void ExtractTextureInformation()
        {
            Color[] color1D = new Color[sprite.Texture2D.Width * sprite.Texture2D.Height];
            sprite.Texture2D.GetData(color1D);
            healthBarColorMatrix = color1D.ConvertTo2D(sprite.Texture2D.Width, sprite.Texture2D.Height);
        }

        public void Update()
        {
            sprite.Position = Mobile.MobileFlipbook.Position + Parameter.HealthBarOffset;
            UpdateBarSprite();
        }

        public void UpdateBarSprite()
        {
            //Filling the bar with the colors
            currentHealthPercentage = (float)Math.Floor(healthBarSize * Mobile.MobileMetadata.CurrentHealth / totalBar);
            currentSheildPercentage = (float)Math.Ceiling(healthBarSize * Mobile.MobileMetadata.CurrentShield / totalBar);

            Color healthBarColor = Mobile.IsHealthCritical ? Parameter.HealthBarColorRed : Parameter.HealthBarColorGreen;

            for (int h = 1; h < healthBarColorMatrix.Length - 1; h++)
            {
                for (int w = 1; w < healthBarColorMatrix[0].Length - 1; w++)
                {
                    if (w <= currentHealthPercentage)
                        healthBarColorMatrix[h][w] = healthBarColor;
                    else if (w <= currentSheildPercentage + currentHealthPercentage)
                        healthBarColorMatrix[h][w] = Parameter.HealthBarColorBlue;
                    else
                        healthBarColorMatrix[h][w] = Parameter.HealthBarColorBlack;
                }
            }

            Color[] color1D = healthBarColorMatrix.ConvertTo1D();
            sprite.Texture2D.SetData(0, null, color1D, 0, color1D.Length);
        }

        public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            sprite.Draw(GameTime, SpriteBatch);
        }

        public void HideElement()
        {
            sprite.HideElement();
        }

        public void ShowElement()
        {
            sprite.ShowElement();
        }
    }
}
