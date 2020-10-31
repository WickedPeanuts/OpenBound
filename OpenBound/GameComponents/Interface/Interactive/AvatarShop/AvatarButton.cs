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
using OpenBound.GameComponents.Interface.Text;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.Models;
using System;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Interface.Interactive.AvatarShop
{
    public class AvatarButton : Button
    {
        List<Sprite> spriteList;
        List<SpriteText> spriteTextList;

        Sprite typeIcon;
        Sprite thumb;
        Sprite newMarker;
        Sprite equippedIndicator;
        Sprite ownedCheck;
        Sprite goldIcon;
        Sprite cashIcon;

        //Texts
        SpriteText itemNameSpriteText;
        SpriteText goldSpriteText;
        SpriteText cashSpriteText;

        float newMarkerElapsedTime = 0;

        public AvatarMetadata AvatarMetadata { get; private set; }

        public AvatarButton(Vector2 position, AvatarMetadata avatarMetadata, Action<object> onClick, float layerDepth)
            : base(ButtonType.AvatarButton, layerDepth, onClick, position)
        {
            AvatarMetadata = avatarMetadata;

            spriteList = new List<Sprite>();
            spriteTextList = new List<SpriteText>();

            thumb = new Sprite($"Graphics/Avatar/{avatarMetadata.Gender}/Portrait/{avatarMetadata.AvatarCategory}/{avatarMetadata.Name}",
                position + new Vector2(1, 6f), layerDepth + 0.001f);
            Vector2 scaleFactor = new Vector2(87, 48) / new Vector2(thumb.SpriteWidth, thumb.SpriteHeight);
            float newScale = Math.Min(scaleFactor.X, scaleFactor.Y);
            thumb.Scale = new Vector2(newScale, newScale);

            spriteList.Add(thumb);

            //New Marker
            if (DateTime.Now.AddDays(Parameter.InterfaceAvatarShopAvatarButtonNewStampDay) > avatarMetadata.Date)
            {
                newMarker = new Sprite("Interface/StaticButtons/AvatarShop/AvatarButton/NewItem",
                    position + new Vector2(40, -30), layerDepth + 0.003f);
                newMarker.Scale /= 2;

                spriteList.Add(newMarker);
            }

            //Equipped Indicator
            equippedIndicator = new Sprite("Interface/StaticButtons/AvatarShop/AvatarButton/EquipedIndicator",
                position + new Vector2(-47, 0), layerDepth + 0.002f);

            spriteList.Add(equippedIndicator);

            if (GameInformation.Instance.PlayerInformation.GetEquippedAvatar(avatarMetadata.AvatarCategory) != avatarMetadata.ID)
                HideEquippedIndicator();

            //Owned check
            if (GameInformation.Instance.PlayerInformation.OwnedAvatar[avatarMetadata.AvatarCategory].Contains(avatarMetadata.ID))
            {
                ownedCheck = new Sprite("Interface/StaticButtons/AvatarShop/AvatarButton/OwnedCheck",
                    position + new Vector2(38, -14), layerDepth + 0.002f);
                ownedCheck.Scale /= 2;

                spriteList.Add(ownedCheck);
            }

            //Selecting button icon
            int index = GetCorrespondingIconIndex(avatarMetadata.AvatarCategory, GameInformation.Instance.PlayerInformation.Gender);
            typeIcon = new Sprite("Interface/StaticButtons/AvatarShop/AvatarButton/ButtonIcons",
                position + new Vector2(-32, -12), layerDepth + 0.002f,
                new Rectangle((index % 6) * 26, (index / 6) * 17, 26, 17));
            typeIcon.Pivot = new Vector2(13, 8.5f);

            spriteList.Add(typeIcon);

            //Texts
            string name = avatarMetadata.Name;
            if (name.Length > 13)
                name = name.Substring(0, 12) + ".";

            itemNameSpriteText = new SpriteText(FontTextType.Consolas10, name, Color.White, Alignment.Left, layerDepth + 0.002f, position, Color.Black);
            itemNameSpriteText.PositionOffset = position + new Vector2(-44, -36);
            spriteTextList.Add(itemNameSpriteText);

            int pricePos = 0;
            if (avatarMetadata.GoldPrice != 0)
            {
                goldSpriteText = new SpriteText(FontTextType.Consolas10, string.Format("{0:N0}", avatarMetadata.GoldPrice), Parameter.InterfaceAvatarShopButtonGoldColor, Alignment.Right, layerDepth + 0.003f, position, Parameter.InterfaceAvatarShopButtonGoldOutlineColor);
                goldSpriteText.PositionOffset = position + new Vector2(30, 20 - 10 * pricePos);
                goldIcon = new Sprite("Interface/StaticButtons/AvatarShop/AvatarButton/GoldIcon",
                    position + new Vector2(38, 26 - 10 * pricePos++), layerDepth + 0.002f, new Rectangle(0, 0, 26, 21));
                goldIcon.Pivot = new Vector2(13, 10.5f);
                goldIcon.Scale /= 2;

                spriteTextList.Add(goldSpriteText);
                spriteList.Add(goldIcon);
            }

            if (avatarMetadata.CashPrice != 0)
            {
                cashSpriteText = new SpriteText(FontTextType.Consolas10, string.Format("{0:N0}", avatarMetadata.CashPrice), Parameter.InterfaceAvatarShopButtonCashColor, Alignment.Right, layerDepth + 0.003f, position, Parameter.InterfaceAvatarShopButtonCashOutlineColor);
                cashSpriteText.PositionOffset = position + new Vector2(30, 20 - 10 * pricePos);
                cashIcon = new Sprite("Interface/StaticButtons/AvatarShop/AvatarButton/CashIcon",
                    position + new Vector2(38, 26 - 10 * pricePos++), layerDepth + 0.002f, new Rectangle(0, 0, 26, 21));
                cashIcon.Pivot = new Vector2(13, 10.5f);
                cashIcon.Scale /= 2;

                spriteTextList.Add(cashSpriteText);
                spriteList.Add(cashIcon);
            }
        }

        public void ShowEquippedIndicator() => equippedIndicator.ShowElement();

        public void HideEquippedIndicator() => equippedIndicator.HideElement();

        public void Update(GameTime gameTime)
        {
            base.Update();

            spriteList.ForEach((x) => x.UpdateAttatchmentPosition());
            spriteTextList.ForEach((x) => x.UpdateAttatchedPosition());

            newMarkerElapsedTime += MathHelper.Pi / 2 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            newMarkerElapsedTime %= MathHelper.TwoPi;

            newMarker?.SetTransparency((float)Math.Sin(newMarkerElapsedTime));
        }

        private static int GetCorrespondingIconIndex(AvatarCategory avatarCategory, Gender gender)
        {
            switch (avatarCategory)
            {
                case AvatarCategory.Body:
                    return (gender == Gender.Male) ? 0 : 1;
                case AvatarCategory.Hat:
                    return (gender == Gender.Male) ? 2 : 3;
                case AvatarCategory.Goggles:
                    return (gender == Gender.Male) ? 4 : 5;
                case AvatarCategory.Flag:
                    return 6;
                case AvatarCategory.ExItem:
                    return 29;
                case AvatarCategory.Extra:
                    return 32;
                case AvatarCategory.Misc:
                    return 33;
                case AvatarCategory.Pet:
                    return 34;

                default:
                    return 7;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            spriteList.ForEach((x) => x.Draw(gameTime, spriteBatch));
            spriteTextList.ForEach((x) => x.Draw(spriteBatch));
        }
    }
}
