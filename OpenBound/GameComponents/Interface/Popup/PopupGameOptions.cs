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
using OpenBound.GameComponents.Audio;
using OpenBound.GameComponents.Interface.Interactive;
using OpenBound.GameComponents.Interface.Text;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.FileOutput;
using System;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Interface.Popup
{
    public class PopupGameOptions : PopupMenu
    {
        public List<SliderBar> sliderBarList;
        public List<RadioButtonSet> radioButtonSetList;

        RadioButtonSet aimingRadioSet;

        /*
         * Gameplay
	     * - Aiming Type (radio button) Drag / Splice
	     * - Scroll Speed (slider bar)
	     * - Mouse Speed (slider bar)
         * Sound
	     * - BGM (slider Bar)
	     * - SFX (slider bar)
         * Misc	
	     * - Background (radio button) On / Off
	     * - Interface  (radio button) Classic / TH Blue / TH Pink / OpenBound
         */

        public PopupGameOptions(Vector2 position) : base(true)
        {
            positionOffset = position;

            sliderBarList = new List<SliderBar>();
            radioButtonSetList = new List<RadioButtonSet>();

            Background = new Sprite("Interface/Popup/Blue/Options/Background", layerDepth: DepthParameter.InterfacePopupBackground);
            background.PositionOffset = position;

            //SpriteText Session
            //Gameplay
            SpriteText sp = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsGameplay, Parameter.TextColorPopupIngameOptionsCategory, Alignment.Left, DepthParameter.InterfacePopupText)
            { PositionOffset = position + new Vector2(-120, -120) };
            spriteTextList.Add(sp);

            //Gameplay / Shooting Mode
            sp = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsGameplayAimingMode, Parameter.TextColorPopupIngameOptionsSubCategory, Alignment.Left, DepthParameter.InterfacePopupText)
            { PositionOffset = position + new Vector2(-110, -100) };
            spriteTextList.Add(sp);

            //Gameplay / Shooting Mode / Drag
            SpriteText spDrag = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsGameplayAimingModeDrag, Parameter.TextColorPopupIngameOptionsElement, Alignment.Left, DepthParameter.InterfacePopupText);

            //Gameplay / Shooting Mode / Slice
            SpriteText spSlice = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsGameplayAimingModeSlice, Parameter.TextColorPopupIngameOptionsElement, Alignment.Left, DepthParameter.InterfacePopupText);

            //Gameplay / Scroll Speed
            sp = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsGameplayScrollSpeed, Parameter.TextColorPopupIngameOptionsElement, Alignment.Left, DepthParameter.InterfacePopupText)
            { PositionOffset = position + new Vector2(-110, -80) };
            spriteTextList.Add(sp);

            //Gameplay / Mouse Speed
            sp = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsGameplayMouseSpeed, Parameter.TextColorPopupIngameOptionsElement, Alignment.Left, DepthParameter.InterfacePopupText)
            { PositionOffset = position + new Vector2(-110, -60) };
            spriteTextList.Add(sp);

            //Sound
            sp = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsSound, Parameter.TextColorPopupIngameOptionsCategory, Alignment.Left, DepthParameter.InterfacePopupText)
            { PositionOffset = position + new Vector2(-120, -40) };
            spriteTextList.Add(sp);

            //Sound / BGM
            sp = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsSoundBGM, Parameter.TextColorPopupIngameOptionsElement, Alignment.Left, DepthParameter.InterfacePopupText)
            { PositionOffset = position + new Vector2(-110, -20) };
            spriteTextList.Add(sp);

            //Sound / SFX
            sp = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsSoundSFX, Parameter.TextColorPopupIngameOptionsElement, Alignment.Left, DepthParameter.InterfacePopupText)
            { PositionOffset = position + new Vector2(-110, 0) };
            spriteTextList.Add(sp);

            //Misc
            sp = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsMisc, Parameter.TextColorPopupIngameOptionsCategory, Alignment.Left, DepthParameter.InterfacePopupText)
            { PositionOffset = position + new Vector2(-120, 20) };
            spriteTextList.Add(sp);

            //Misc / Background
            sp = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsMiscBackground, Parameter.TextColorPopupIngameOptionsSubCategory, Alignment.Left, DepthParameter.InterfacePopupText)
            { PositionOffset = position + new Vector2(-110, 40) };
            spriteTextList.Add(sp);

            //Misc / Background / On
            SpriteText spOn = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsMiscBackgroundOn, Parameter.TextColorPopupIngameOptionsElement, Alignment.Left, DepthParameter.InterfacePopupText)
            { PositionOffset = position + new Vector2(20, 40) };

            //Misc / Background / Off
            SpriteText spOff = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsMiscBackgroundOff, Parameter.TextColorPopupIngameOptionsElement, Alignment.Left, DepthParameter.InterfacePopupText)
            { PositionOffset = position + new Vector2(90, 40) };

            //Misc / Interface
            sp = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsMiscInterface, Parameter.TextColorPopupIngameOptionsSubCategory, Alignment.Left, DepthParameter.InterfacePopupText)
            { PositionOffset = position + new Vector2(-110, 60) };
            spriteTextList.Add(sp);

            //Misc / Interface / Classic
            SpriteText spClassic = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsMiscInterfaceClassic, Parameter.TextColorPopupIngameOptionsElement, Alignment.Left, DepthParameter.InterfacePopupText);

            //Misc / Interface / TH Blue
            SpriteText spTHBlue = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsMiscInterfaceTHBlue, Parameter.TextColorPopupIngameOptionsElement, Alignment.Left, DepthParameter.InterfacePopupText);

            //Misc / Interface / TH White 
            SpriteText spTHWhite = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsMiscInterfaceTHWhite, Parameter.TextColorPopupIngameOptionsElement, Alignment.Left, DepthParameter.InterfacePopupText);

            //Misc / Interface / OpenBound
            SpriteText spOpenBound = new SpriteText(FontTextType.Consolas10, Language.PopupGameOptionsMiscInterfaceOpenBound, Parameter.TextColorPopupIngameOptionsElement, Alignment.Left, DepthParameter.InterfacePopupText);

            //Radio Button Sections
            aimingRadioSet = new RadioButtonSet(RadioButtonType.InterfaceRadioButton, position + new Vector2(10, -94), new Vector2(70, 0), new List<SpriteText>() { spDrag, spSlice }, new Vector2(5, 1), 1, DepthParameter.InterfacePopupButtons);
            #warning Pre-release deactivation
            //radioButtonSetList.Add(aimingRadioSet);

            RadioButtonSet rbSet2 = new RadioButtonSet(RadioButtonType.InterfaceRadioButton, position + new Vector2(10, 46), new Vector2(70, 0), new List<SpriteText>() { spOn, spOff }, new Vector2(5, 1), 0, DepthParameter.InterfacePopupButtons);
            #warning Pre-release deactivation
            //radioButtonSetList.Add(rbSet2);

            RadioButtonSet rbSet3 = new RadioButtonSet(RadioButtonType.InterfaceRadioButton, position + new Vector2(-90, 86), new Vector2(120, 0), new List<SpriteText>() { spClassic, spTHBlue, spTHWhite, spOpenBound }, new Vector2(5, 1), 1, DepthParameter.InterfacePopupButtons);
            rbSet3.UpdateButtonPosition(2, position + new Vector2(-90, 106));
            rbSet3.UpdateButtonPosition(3, position + new Vector2(30, 106));
            #warning Pre-release deactivation
            //radioButtonSetList.Add(rbSet3);

            //Sliding Bar Section
            //Gameplay / Scroll Speed
            SliderBar sbSensitivity1 = new SliderBar(position + new Vector2(60, -73), 50);
#warning Pre-release deactivation
            //sliderBarList.Add(sbSensitivity1);

            //Gameplay / Mouse Speed
#warning Pre-release deactivation
            SliderBar sbSensitivity2 = new SliderBar(position + new Vector2(60, -53), 50);
            //sliderBarList.Add(sbSensitivity2);

            //Sound / BGM
            SliderBar sbSound1 = new SliderBar(position + new Vector2(60, -13), (int)(AudioHandler.BGMVolume * 100));
            sliderBarList.Add(sbSound1);
            sbSound1.OnBeingDragged += (pgBar) => AudioHandler.ChangeBGMVolume((int)((ProgressBar)pgBar).Intensity);

            //Sound / SFX
            SliderBar sbSound2 = new SliderBar(position + new Vector2(60, 7), (int)(AudioHandler.SFXVolume * 100));
            sliderBarList.Add(sbSound2);
            sbSound2.OnBeingDragged += (pgBar) => AudioHandler.ChangeSFXVolume((int)((ProgressBar)pgBar).Intensity);

            //Accept / Cancel Popup
            buttonList.Add(new Button(ButtonType.Accept, DepthParameter.InterfacePopupButtons, CloseAction, buttonOffset: position + new Vector2(130, 144)));
            //buttonList.Add(new Button(ButtonType.Cancel, 1, CloseAction, buttonOffset: new Vector2(130, 144)));
        }

        //protected void RadioButtonSet

        protected override void CloseAction(object sender)
        {
            base.CloseAction(sender);

            GameClientSettingsInformation gcsi = Parameter.GameClientSettingsInformation;
            //gcsi.AimType = 
            gcsi.BGM = (int)(AudioHandler.BGMVolume * 100);
            gcsi.SFX = (int)(AudioHandler.SFXVolume * 100);
            //gcsi.IsBackgroundOn =
            //gcsi.InterfaceType =

            ConfigFileManager.OverwriteGameServerSettings(gcsi);
        }

        public override void RealocateElements(Vector2 delta)
        {
            delta = delta.ToIntegerDomain();

            base.RealocateElements(delta);

            sliderBarList.ForEach((x) => x.RealocateElements(delta));
            radioButtonSetList.ForEach((x) => x.RealocateElements(delta));
        }

        protected override void UpdateAttatchmentPosition()
        {
            base.UpdateAttatchmentPosition();
        }

        public override void Update(GameTime gameTime)
        {
            if (!ShouldRender) return;

            base.Update(gameTime);

            UpdateAttatchmentPosition();
            sliderBarList.ForEach((x) => x.Update());
            radioButtonSetList.ForEach((x) => x.Update());
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!ShouldRender) return;

            base.Draw(gameTime, spriteBatch);

            sliderBarList.ForEach((x) => x.Draw(gameTime, spriteBatch));
            radioButtonSetList.ForEach((x) => x.Draw(gameTime, spriteBatch));
        }
    }
}
