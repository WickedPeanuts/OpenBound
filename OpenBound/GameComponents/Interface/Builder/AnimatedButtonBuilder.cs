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
using OpenBound.GameComponents.Interface.Interactive;
using OpenBound.GameComponents.Interface.Text;
using OpenBound_Network_Object_Library.Entity.Text;
using System;

namespace OpenBound.GameComponents.Interface.Builder
{
    public class AnimatedButtonBuilder
    {
        public static AnimatedButton BuildButton(AnimatedButtonType type, Vector2 position, Action<object> onClick = default)
        {
            string buttonText = "";

            //Animated Buttons
            if (type == AnimatedButtonType.ExitDoor)
                buttonText = Language.ButtonTextExitText;
            else if (type == AnimatedButtonType.Buddy)
                buttonText = Language.ButtonTextBuddyList;
            else if (type == AnimatedButtonType.CashCharge)
                buttonText = Language.ButtonTextCashCharge;
            else if (type == AnimatedButtonType.ReportPlayer)
                buttonText = Language.ButtonTextReportPlayer;
            else if (type == AnimatedButtonType.AvatarShop)
                buttonText = Language.ButtonTextAvatarShop;
            else if (type == AnimatedButtonType.Options)
                buttonText = Language.ButtonTextOptions;
            else if (type == AnimatedButtonType.MyInfo)
                buttonText = Language.ButtonTextMyInfo;
            else if (type == AnimatedButtonType.MuteList)
                buttonText = Language.ButtonTextMuteList;
            //Room Filtering - Gamemode
            else if (type == AnimatedButtonType.Solo)
                buttonText = Language.ButtonTextRoomFilteringSolo;
            else if (type == AnimatedButtonType.Tag)
                buttonText = Language.ButtonTextRoomFilteringTag;
            else if (type == AnimatedButtonType.Score)
                buttonText = Language.ButtonTextRoomFilteringScore;
            else if (type == AnimatedButtonType.Jewel)
                buttonText = Language.ButtonTextRoomFilteringJewel;
            //Room Filtering - Type
            else if (type == AnimatedButtonType.ViewAll)
                buttonText = Language.ButtonTextRoomFilteringViewAll;
            else if (type == AnimatedButtonType.ViewWaiting)
                buttonText = Language.ButtonTextRoomFilteringViewWaiting;
            else if (type == AnimatedButtonType.ViewFriend)
                buttonText = Language.ButtonTextRoomFilteringViewFriend;
            //Room Options
            else if (type == AnimatedButtonType.GoTo)
                buttonText = Language.ButtonTextGoTo;
            else if (type == AnimatedButtonType.Create)
                buttonText = Language.ButtonTextCreate;
            else if (type == AnimatedButtonType.QuickJoin)
                buttonText = Language.ButtonTextQuickJoin;
            //Room Buttons
            else if (type == AnimatedButtonType.Ready)
                buttonText = Language.ButtonTextInRoomReady;
            else if (type == AnimatedButtonType.MatchSettings)
                buttonText = Language.ButtonTextInRoomMatchSettings;
            else if (type == AnimatedButtonType.ChangeMobile)
                buttonText = Language.ButtonTextInRoomChangeMobile;
            else if (type == AnimatedButtonType.ChangeItem)
                buttonText = Language.ButtonTextInRoomChangeItem;
            else if (type == AnimatedButtonType.ChangeTeam)
                buttonText = Language.ButtonTextInRoomChangeTeam;
            //Avatar Shop Buttons
            else if (type == AnimatedButtonType.Try)
                buttonText = Language.ButtonTextAvatarShopTry;
            else if (type == AnimatedButtonType.Buy)
                buttonText = Language.ButtonTextAvatarShopBuy;
            else if (type == AnimatedButtonType.Gift)
                buttonText = Language.ButtonTextAvatarShopGift;
            //Avatar Shop Buttons - Filter
            else if (type == AnimatedButtonType.Hat)
                buttonText = Language.ButtonTextAvatarShopHat;
            else if (type == AnimatedButtonType.Body)
                buttonText = Language.ButtonTextAvatarShopBody;
            else if (type == AnimatedButtonType.Goggles)
                buttonText = Language.ButtonTextAvatarShopGoggles;
            else if (type == AnimatedButtonType.Flag)
                buttonText = Language.ButtonTextAvatarShopFlag;
            else if (type == AnimatedButtonType.ExItem)
                buttonText = Language.ButtonTextAvatarShopExItem;
            else if (type == AnimatedButtonType.Pet)
                buttonText = Language.ButtonTextAvatarShopPet;
            else if (type == AnimatedButtonType.Necklace)
                buttonText = Language.ButtonTextAvatarShopNecklace;
            else if (type == AnimatedButtonType.Ring)
                buttonText = Language.ButtonTextAvatarShopRing;

            return new AnimatedButton(type, position, onClick,
                new SpriteText(Parameter.AnimatedButtonFont,
                    buttonText, Color.White, Alignment.Center,
                    layerDepth: DepthParameter.InterfaceButtonText));
        }
    }
}
