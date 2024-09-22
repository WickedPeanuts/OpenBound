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
using OpenBound.GameComponents.Interface.Interactive;
using OpenBound.GameComponents.Interface.Text;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound.GameComponents.Interface.Popup
{
    public class PopupSelectMobile : PopupMenu
    {
        private static Dictionary<MobileType, Rectangle> portraitPresets = new Dictionary<MobileType, Rectangle>()
        {
            {MobileType.Aduka,        new Rectangle(102 * 00, 0, 102, 80) },
            {MobileType.Armor,        new Rectangle(102 * 01, 0, 102, 80) },
            {MobileType.ASate,        new Rectangle(102 * 02, 0, 102, 80) },
            {MobileType.Bigfoot,      new Rectangle(102 * 03, 0, 102, 80) },
            {MobileType.Boomer,       new Rectangle(102 * 04, 0, 102, 80) },
            {MobileType.Grub,         new Rectangle(102 * 05, 0, 102, 80) },
            {MobileType.Ice,          new Rectangle(102 * 06, 0, 102, 80) },
            {MobileType.JD,           new Rectangle(102 * 07, 0, 102, 80) },
            {MobileType.JFrog,        new Rectangle(102 * 08, 0, 102, 80) },
            {MobileType.Kalsiddon,    new Rectangle(102 * 09, 0, 102, 80) },
            {MobileType.Lightning,    new Rectangle(102 * 10, 0, 102, 80) },
            {MobileType.Mage,         new Rectangle(102 * 11, 0, 102, 80) },
            {MobileType.Nak,          new Rectangle(102 * 12, 0, 102, 80) },
            {MobileType.RaonLauncher, new Rectangle(102 * 13, 0, 102, 80) },
            {MobileType.Trico,        new Rectangle(102 * 14, 0, 102, 80) },
            {MobileType.Turtle,       new Rectangle(102 * 15, 0, 102, 80) },
            {MobileType.Random,       new Rectangle(102 * 16, 0, 102, 80) },
        };

        Sprite portrait;
        SpriteText portraitMobileName;

        Action<MobileType> OnSelectMobile;

        public PopupSelectMobile(Action<MobileType> OnSelectMobile, Action OnClose) : base(false)
        {
            this.OnSelectMobile = OnSelectMobile;

            MobileType defaultMobile = GameInformation.Instance.PlayerInformation.PrimaryMobile;

            Background = new Sprite("Interface/Popup/Blue/SelectMobile/Background", layerDepth: DepthParameter.InterfacePopupBackground);

            spriteList.Add(new Sprite("Interface/Popup/Blue/SelectMobile/AttackMetter", position: new Vector2(-18, 191), layerDepth: DepthParameter.InterfacePopupForeground));
            spriteList.Add(new Sprite("Interface/Popup/Blue/SelectMobile/DefenseMetter", position: new Vector2(-10, 203), layerDepth: DepthParameter.InterfacePopupForeground));
            spriteList.Add(new Sprite("Interface/Popup/Blue/SelectMobile/MovementMetter", position: new Vector2(-2, 215), layerDepth: DepthParameter.InterfacePopupForeground));
            spriteList.Add(new Sprite("Interface/Popup/Blue/SelectMobile/DelayMetter", position: new Vector2(8, 227), layerDepth: DepthParameter.InterfacePopupForeground));

            portrait = new Sprite("Interface/Popup/Blue/SelectMobile/Portrait", position: new Vector2(-188, 195), layerDepth: DepthParameter.InterfacePopupForeground, sourceRectangle: portraitPresets[MobileType.Random]);
            portrait.Pivot = new Vector2(51, 40);

            spriteList.Add(portrait);

            Vector2 bSPos = new Vector2(-145, -162);
            Vector2 xFact = new Vector2(66, 0);
            Vector2 yFact = new Vector2(0, 61);

            buttonList.Add(new Button(ButtonType.SelectMobileArmor, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 0 + yFact * 0));
            buttonList.Add(new Button(ButtonType.SelectMobileBigfoot, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 1 + yFact * 0));
            buttonList.Add(new Button(ButtonType.SelectMobileTrico, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 5 + yFact * 0));
            buttonList.Add(new Button(ButtonType.SelectMobileDragon, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 6 + yFact * 0));

            buttonList.Add(new Button(ButtonType.SelectMobileFrank, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 0 + yFact * 1));
            buttonList.Add(new Button(ButtonType.SelectMobileRaonLauncher, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 1 + yFact * 1));
            buttonList.Add(new Button(ButtonType.SelectMobileMage, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 2 + yFact * 1));
            buttonList.Add(new Button(ButtonType.SelectMobileASate, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 3 + yFact * 1));
            buttonList.Add(new Button(ButtonType.SelectMobileKnight, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 4 + yFact * 1));
            buttonList.Add(new Button(ButtonType.SelectMobileJFrog, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 5 + yFact * 1));
            buttonList.Add(new Button(ButtonType.SelectMobileBlueWhale, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 6 + yFact * 1));

            buttonList.Add(new Button(ButtonType.SelectMobileAduka, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 0 + yFact * 2));
            buttonList.Add(new Button(ButtonType.SelectMobileKalsiddon, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 1 + yFact * 2));
            buttonList.Add(new Button(ButtonType.SelectMobileJD, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 2 + yFact * 2));
            buttonList.Add(new Button(ButtonType.SelectMobileLightning, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 3 + yFact * 2));
            buttonList.Add(new Button(ButtonType.SelectMobileWolf, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 4 + yFact * 2));
            buttonList.Add(new Button(ButtonType.SelectMobileGrub, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 5 + yFact * 2));

            buttonList.Add(new Button(ButtonType.SelectMobileNak, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 0 + yFact * 3));
            buttonList.Add(new Button(ButtonType.SelectMobileMaya, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 1 + yFact * 3));
            buttonList.Add(new Button(ButtonType.SelectMobilePhoenix, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 2 + yFact * 3));
            buttonList.Add(new Button(ButtonType.SelectMobileTiburon, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 3 + yFact * 3));
            buttonList.Add(new Button(ButtonType.SelectMobileBoomer, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 4 + yFact * 3));
            buttonList.Add(new Button(ButtonType.SelectMobileIce, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 5 + yFact * 3));
            buttonList.Add(new Button(ButtonType.SelectMobileTurtle, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 6 + yFact * 3));

            buttonList.Add(new Button(ButtonType.SelectMobileRandom, DepthParameter.InterfacePopupButtons, ButtonAction, buttonOffset: bSPos + xFact * 0 + yFact * 4 + new Vector2(0, 1)));

            //Mobile Portrait Name
            portraitMobileName = new SpriteText(FontTextType.Consolas16, GameInformation.Instance.PlayerInformation.PrimaryMobile.ToString(), Parameter.TextColorPopupSelectMobilePortrait, Alignment.Left, DepthParameter.InterfacePopupText, outlineColor: Color.White);
            portraitMobileName.PositionOffset = spriteList.Last().Position + new Vector2(-50, -60);

            spriteTextList.Add(portraitMobileName);

            //Append Functionality to the buttons and SpriteTexts, Map Buttons with current MobileTypes
            List<MobileType> mTL = new List<MobileType>(){
                MobileType.Armor, MobileType.Bigfoot,      MobileType.Trico,   MobileType.Dragon,
                MobileType.Frank, MobileType.RaonLauncher, MobileType.Mage,    MobileType.ASate,     MobileType.Knight, MobileType.JFrog, MobileType.BlueWhale,
                MobileType.Aduka, MobileType.Kalsiddon,    MobileType.JD,      MobileType.Lightning, MobileType.Wolf,   MobileType.Grub,
                MobileType.Nak,   MobileType.Maya,         MobileType.Phoenix, MobileType.Tiburon,   MobileType.Boomer, MobileType.Ice,   MobileType.Turtle,
                MobileType.Random
            };

            int index = 0;
            foreach (MobileType mt in mTL)
            {
                buttonList[index].Tag = mt;
                SpriteText tmp = new SpriteText(FontTextType.Consolas10Family, mTL[index].ToString(), Color.White, Alignment.Center, DepthParameter.InterfacePopupText, outlineColor: Color.Black);
                tmp.PositionOffset = buttonList[index].ButtonOffset + new Vector2(0, 15);
                spriteTextList.Add(tmp);
                index++;
            }

            //Default selected buttons
            buttonList.Find((x) => (MobileType)x.Tag == GameInformation.Instance.PlayerInformation.PrimaryMobile).ChangeButtonState(ButtonAnimationState.Clicked, true);

            //Disabled Mobiles (Temporary)
            foreach (Button b in buttonList)
                if (!NetworkObjectParameters.ImplementedMobileList.Contains((MobileType)b.Tag))
                    b.Disable();

            //Close Button
            buttonList.Add(new Button(ButtonType.Cancel, DepthParameter.InterfacePopupButtons, (sender) => { CloseAction(sender); OnClose(); }, new Vector2(230, 253)));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateAttatchmentPosition();
        }

        protected override void UpdateAttatchmentPosition()
        {
            base.UpdateAttatchmentPosition();

            spriteList.ForEach((x) => x.UpdateAttatchmentPosition());
        }

        public void ButtonAction(object button)
        {
            MobileType mt = (MobileType)((Button)button).Tag;

            OnSelectMobile.Invoke(mt);

            //Button Animation triggering
            foreach (Button b in buttonList)
            {
                if (b.IsEnabled)
                {
                    b.IsActivated = false;
                    b.ChangeButtonState(ButtonAnimationState.Normal, true);
                }
            }

            ((Button)button).ChangeButtonState(ButtonAnimationState.Activated, true);

            //Button Actions - Status bars
            MobileStatus mobS = MobileMetadata.MobileStatusPresets[mt];

            spriteList[0].Scale = Vector2.One * new Vector2(200 * mobS.Attack / MobileMetadata.BestMobileStatus.Attack, 1);
            spriteList[1].Scale = Vector2.One * new Vector2(200 * mobS.Defence / MobileMetadata.BestMobileStatus.Defence, 1);
            spriteList[2].Scale = Vector2.One * new Vector2(200 * mobS.Delay / MobileMetadata.BestMobileStatus.Delay, 1);
            spriteList[3].Scale = Vector2.One * new Vector2(200 * mobS.Mobility / MobileMetadata.BestMobileStatus.Mobility, 1);

            //Text Updating
            portraitMobileName.Text = mt.ToString();

            if (!portraitPresets.ContainsKey(mt)) mt = MobileType.Random;

            spriteList[4].SourceRectangle = portraitPresets[mt];

            GameInformation.Instance.PlayerInformation.PrimaryMobile = mt;
        }
    }
}
