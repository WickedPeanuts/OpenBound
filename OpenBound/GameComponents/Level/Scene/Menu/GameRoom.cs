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
using OpenBound.Extension;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Audio;
using OpenBound.GameComponents.Interface.Builder;
using OpenBound.GameComponents.Interface.Interactive;
using OpenBound.GameComponents.Interface.Interactive.GameRoom;
using OpenBound.GameComponents.Interface.Popup;
using OpenBound.GameComponents.Interface.Text;
using OpenBound.ServerCommunication;
using OpenBound.ServerCommunication.Service;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using System.Collections.Generic;
using System.Linq;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.Extension;
using OpenBound.GameComponents.Pawn;

namespace OpenBound.GameComponents.Level.Scene.Menu
{
    public class GameRoom : GameScene
    {
        static Dictionary<MobileType, Rectangle> portraitPresets = new Dictionary<MobileType, Rectangle>()
        {
            { MobileType.Aduka,        new Rectangle(111 * 00, 0, 111, 86) },
            { MobileType.Armor,        new Rectangle(111 * 01, 0, 111, 86) },
            { MobileType.ASate,        new Rectangle(111 * 02, 0, 111, 86) },
            { MobileType.Bigfoot,      new Rectangle(111 * 03, 0, 111, 86) },
            { MobileType.Boomer,       new Rectangle(111 * 04, 0, 111, 86) },
            { MobileType.Grub,         new Rectangle(111 * 05, 0, 111, 86) },
            { MobileType.Ice,          new Rectangle(111 * 06, 0, 111, 86) },
            { MobileType.JD,           new Rectangle(111 * 07, 0, 111, 86) },
            { MobileType.JFrog,        new Rectangle(111 * 08, 0, 111, 86) },
            { MobileType.Kalsiddon,    new Rectangle(111 * 09, 0, 111, 86) },
            { MobileType.Lightning,    new Rectangle(111 * 10, 0, 111, 86) },
            { MobileType.Mage,         new Rectangle(111 * 11, 0, 111, 86) },
            { MobileType.Nak,          new Rectangle(111 * 12, 0, 111, 86) },
            { MobileType.Random,       new Rectangle(111 * 13, 0, 111, 86) },
            { MobileType.RaonLauncher, new Rectangle(111 * 14, 0, 111, 86) },
            { MobileType.Trico,        new Rectangle(111 * 15, 0, 111, 86) },
            { MobileType.Turtle,       new Rectangle(111 * 16, 0, 111, 86) },
        };

        AnimatedButton options, matchSettings, exitDoor, ready;
        AnimatedButton selectItem;

        List<AnimatedButton> animatedButtonList;

        Button metadataRenameRoom, metadataPreviousMap, metadataNextMap;
        List<Button> buttonList;
        List<Sprite> spriteList;

        PlayerButtonList playerButtonList;

        MatchConfigurationGrid matchConfigurationGrid;
        CompositeSpriteText serverName;

        PopupSelectMobile popupSelectMobile;
        PopupSelectItem popupSelectItem;

        Sprite mobilePortrait;
        Sprite mobilePortraitAtkBar, mobilePortraitDefBar, mobilePortraitMobBar;

        TextBox textBox;

        //MatchMetadata matchMetadata;

        public GameRoom()
        {
            animatedButtonList = new List<AnimatedButton>();
            buttonList = new List<Button>();
            spriteList = new List<Sprite>();

            //Background
            Background = new Sprite(@"Interface/InGame/Scene/GameRoom/Background",
                position: Parameter.ScreenCenter,
                layerDepth: DepthParameter.Foreground,
                shouldCopyAsset: false);

            //UI Components
            CreatePlayerButton();
            CreateMiddleBarAnimatedButtons();
            CreateBottomBarAnimatedButtons();
            CreateMatchMetadataButtons();
            CreateRoomName();
            CreateMobilePortrait();

            //text box
            textBox = new TextBox(new Vector2(-391, 139), new Vector2(410, 135), 100, 0,
                hasScrollBar: true, scrollBackgroundAlpha: 0.6f,
                hasTextField: true, textFieldBackground: 0, textFieldOffset: new Vector2(30, -1), maximumTextLength: 50,
                onSendMessage: OnSendMessage);

            textBox.EnableTextField();

            matchConfigurationGrid = new MatchConfigurationGrid(Parameter.ScreenCenter - new Vector2(-1, 115));

            ServerInformationBroker.Instance.ActionCallbackDictionary[NetworkObjectParameters.GameServerRoomRefreshMetadata] += UpdateRoomMetadataAsyncCallback;
            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerRoomLeaveRoom, LeaveRoomAsyncCallback);
            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerRoomReadyRoom, ReadyRoomAsyncCallback);

            //Textual callbacks
            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerChatSendPlayerMessage, OnReceiveMessageAsyncCallback);
            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerChatSendSystemMessage, OnReceiveMessageAsyncCallback);

            playerButtonList.UpdatePlayerButtonList();

            //Popup menus
            popupSelectMobile = new PopupSelectMobile(SelectMobileAction, CloseSelectMobileAction);
            PopupHandler.Add(popupSelectMobile);
            
            PopupHandler.PopupGameOptions.OnClose = OptionsCloseAction;

            popupSelectItem = new PopupSelectItem();
            popupSelectItem.OnClose += SelectItemCloseAction;

            //Connect to channel
            ServerInformationHandler.SendChatDisconnectionRequest();
            ServerInformationHandler.SendChatConnectionRequest(Message.BuildGameServerChatGameRoom(GameInformation.Instance.RoomMetadata.ID));

            #warning Pre-release deactivation
            animatedButtonList[2].Disable();
            animatedButtonList[5].Disable();
            animatedButtonList[6].Disable();
            animatedButtonList[8].Disable();
            buttonList.Remove(metadataRenameRoom);
        }

        public override void OnSceneIsActive()
        {
            base.OnSceneIsActive();

            AudioHandler.ChangeSong(SongParameter.GameRoom, ChangeEffect.Fade);
        }

        #region AsyncCallback
        private void UpdateRoomMetadataAsyncCallback(object answer)
        {
            Player owningPlayer = GameInformation.Instance.RoomMetadata.PlayerList.Find((x) => x.ID == GameInformation.Instance.PlayerInformation.ID);
            GameInformation.Instance.PlayerInformation.PlayerTeam = owningPlayer.PlayerTeam;

            lock (playerButtonList)
            {
                playerButtonList.UpdatePlayerButtonList();
            }

            lock (buttonList)
            {
                UpdateHostOnlyButons();
                matchConfigurationGrid.UpdateStatusMenu();
            }

            lock (animatedButtonList)
            {
                if (popupSelectMobile != null && !popupSelectMobile.ShouldRender)
                    ready.Enable();
            }

            //Change player mobile
            GameInformation.Instance.PlayerInformation.PrimaryMobile = GameInformation.Instance.RoomMetadata.PlayerList.Find((x) => x.ID == GameInformation.Instance.PlayerInformation.ID).PrimaryMobile;

            UpdateMobilePortrait();
        }

        private void LeaveRoomAsyncCallback(object answer)
        {
            if ((bool)answer)
            {
                ServerInformationBroker.Instance.ActionCallbackDictionary[NetworkObjectParameters.GameServerRoomRefreshMetadata] -= UpdateRoomMetadataAsyncCallback;
                SceneHandler.Instance.RequestSceneChange(SceneType.GameList,
                    TransitionEffectType.RotatingRectangles);
            }
            else
            {
                exitDoor.Enable();
            }

            ServerInformationBroker.Instance.ActionCallbackDictionary[NetworkObjectParameters.GameServerRoomRefreshMetadata] -= UpdateRoomMetadataAsyncCallback;
            SceneHandler.Instance.RequestSceneChange(SceneType.GameList,
                    TransitionEffectType.RotatingRectangles);
        }

        private void ReadyRoomAsyncCallback(object answer)
        {
            ServerInformationBroker.Instance.ActionCallbackDictionary[NetworkObjectParameters.GameServerRoomRefreshMetadata] -= UpdateRoomMetadataAsyncCallback;
            SceneHandler.Instance.RequestSceneChange(SceneType.LoadingScreen,
                TransitionEffectType.None);
        }

        public void OnSendMessage(PlayerMessage message)
        {
            ServerInformationHandler.SendGameListMessage(message);
        }

        public void OnReceiveMessageAsyncCallback(object message)
        {
            // Check if it is possible to append a new textballoon on the scene
            if (message is PlayerMessage) {

                PlayerMessage pMessage = (PlayerMessage)message;
                Mobile pMob = null;

                // Find the mobile owned by the player who sent the message
                lock (playerButtonList)
                {
                    foreach (PlayerButton pb in playerButtonList.PlayerTeamButtonList)
                    {
                        if (pb.Player.Nickname == pMessage.Player.Nickname)
                        {
                            pMob = pb.Mobile;
                            break;
                        }
                    }
                }

                // Append the balloon on the scene
                if (pMob != null)
                    TextBalloonHandler.AsyncAddTextBalloon(pMob, pMessage, 25);
            }

            textBox.AsyncAppendText(message);
        }
        #endregion

        #region Button Actions
        private void SelectItemButtonAction(object sender)
        {
            ((AnimatedButton)sender).Disable();
            popupSelectItem.ShouldRender = true;
            DisableAllButtons();
        }

        private void SelectItemCloseAction(object sender)
        {
            EnableAllButtons();
        }

        private void ExitDoorAction(object sender)
        {
            exitDoor.Disable();
            ServerInformationHandler.LeaveRoom();
        }

        private void ReadyAction(object sender)
        {
            ready.Disable();
            ServerInformationHandler.ReadyRoom();
        }

        private void MobileAction(object sender)
        {
            DisableAllButtons();
            popupSelectMobile.ShouldRender = true;
        }

        private void OptionsAction(object sender)
        {
            buttonList.ForEach((x) => x.Disable());
            animatedButtonList.ForEach((x) => x.ShouldUpdate = false);
            PopupHandler.PopupGameOptions.ShouldRender = true;
            options.Disable();
        }

        private void OptionsCloseAction(object sender)
        {
            buttonList.ForEach((x) => x.Enable());
            animatedButtonList.ForEach((x) => x.ShouldUpdate = true);
            options.Enable();
        }

        private void SelectMobileAction(MobileType mobileType)
        {
            ServerInformationHandler.ChangePrimaryMobile(mobileType);
        }

        private void CloseSelectMobileAction()
        {
            EnableAllButtons();
            UpdateHostOnlyButons();
        }

        private void ChangeTeamAction(object sender)
        {
            AnimatedButton b = (AnimatedButton)sender;

            b.Disable();
            b.OnDisabled = (a) => { b.Enable(); };

            ServerInformationHandler.ChangeTeam();
        }

        private void ChangeMapLeftAction(object sender)
        {
            ServerInformationHandler.ChangeMap(NetworkObjectParameters.ChangeMapLeft);
        }

        private void ChangeMapRightAction(object sender)
        {
            ServerInformationHandler.ChangeMap(NetworkObjectParameters.ChangeMapRight);
        }

        private void DisableAllButtons()
        {
            lock (buttonList)
            {
                buttonList.ForEach((x) => x.Disable());
                animatedButtonList.ForEach((x) => x.Disable());
            }
        }

        private void EnableAllButtons()
        {
            lock (buttonList)
            {
                buttonList.ForEach((x) => x.Enable());
                animatedButtonList.ForEach((x) => x.Enable());

                #warning Pre-release deactivation
                animatedButtonList[2].Disable();
                animatedButtonList[5].Disable();
                animatedButtonList[6].Disable();
                animatedButtonList[8].Disable();
            }
        }
        #endregion

        //This must be turned into a interface component after the mobile information aquisition
        public void CreateMobilePortrait()
        {
            mobilePortrait = new Sprite(
                    "Interface/InGame/Scene/GameRoom/MobileStatus/Portrait",
                    Parameter.ScreenCenter + new Vector2(334, 172), DepthParameter.InterfaceButton,
                    new Rectangle(0, 0, 111, 86), false)
            {
                Pivot = new Vector2(111 / 2, 86 / 2)
            };

            spriteList.Add(mobilePortrait);

            //Max bar size = 110;
            mobilePortraitAtkBar = new Sprite("Interface/InGame/Scene/GameRoom/MobileStatus/AttackMetter",
                Parameter.ScreenCenter + new Vector2(93, 156), DepthParameter.InterfaceButton)
            {
                Pivot = new Vector2(0, 0)
            };

            spriteList.Add(mobilePortraitAtkBar);

            mobilePortraitDefBar = new Sprite("Interface/InGame/Scene/GameRoom/MobileStatus/DefenseMetter",
            Parameter.ScreenCenter + new Vector2(83, 176), DepthParameter.InterfaceButton)
            {
                Pivot = new Vector2(0, 0)
            };

            spriteList.Add(mobilePortraitDefBar);

            mobilePortraitMobBar = new Sprite("Interface/InGame/Scene/GameRoom/MobileStatus/MovementMetter",
            Parameter.ScreenCenter + new Vector2(73, 196), DepthParameter.InterfaceButton)
            {
                Pivot = new Vector2(0, 0)
            };

            spriteList.Add(mobilePortraitMobBar);

            UpdateMobilePortrait();
        }

        public void UpdateHostOnlyButons()
        {
            bool isOwner = GameInformation.Instance.RoomMetadata.RoomOwner.ID == GameInformation.Instance.PlayerInformation.ID;
            metadataNextMap.ShouldRender = metadataPreviousMap.ShouldRender = metadataRenameRoom.ShouldRender = isOwner;

            if (isOwner)
                matchSettings.Enable();
            else
                matchSettings.Disable();

            #warning Pre-release deactivation
            animatedButtonList[2].Disable();
        }

        public void UpdateMobilePortrait()
        {
            MobileStatus mS = MobileMetadata.BestMobileStatus;
            MobileStatus selectedStatus = MobileMetadata.MobileStatusPresets[GameInformation.Instance.PlayerInformation.PrimaryMobile];

            //Max size = 110;
            mobilePortraitAtkBar.Scale = new Vector2(110 * selectedStatus.Attack / mS.Attack, 1);
            mobilePortraitDefBar.Scale = new Vector2(110 * selectedStatus.Defence / mS.Defence, 1);
            mobilePortraitMobBar.Scale = new Vector2(110 * selectedStatus.Mobility / mS.Mobility, 1);

            //UpdatePortrait
            MobileType mt = GameInformation.Instance.PlayerInformation.PrimaryMobile;
            if (!portraitPresets.ContainsKey(mt)) mt = MobileType.Random;

            mobilePortrait.SourceRectangle = portraitPresets[mt];
        }

        public void CreateRoomName()
        {
            SpriteText[] serverNameArr = new SpriteText[2] {
                new SpriteText(FontTextType.Consolas11, $"{GameInformation.Instance.ConnectedServerInformation.ServerID}. ",
                Parameter.NameplateGuildColor, Alignment.Left, DepthParameter.InterfaceButton, outlineColor: Color.Black),
                new SpriteText(FontTextType.Consolas11, $"{GameInformation.Instance.ConnectedServerInformation.ServerName}",
                Color.White, Alignment.Left, DepthParameter.InterfaceButton, outlineColor: Color.Black)
            };

            serverName = CompositeSpriteText.CreateCompositeSpriteText(serverNameArr.ToList(), Orientation.Horizontal, Alignment.Center, Parameter.ScreenCenter + new Vector2(0, -295));
        }

        public void CreateMatchMetadataButtons()
        {
            metadataRenameRoom = new Button(ButtonType.RenameButton, DepthParameter.InterfaceButton, default, new Vector2(-96, -269));
            metadataPreviousMap = new Button(ButtonType.ChangeMapLeftArrow, DepthParameter.InterfaceButton, ChangeMapLeftAction, new Vector2(-90, -154));
            metadataNextMap = new Button(ButtonType.ChangeMapRightArrow, DepthParameter.InterfaceButton, ChangeMapRightAction, new Vector2(90, -154));

            buttonList.AddRange(new List<Button>() { metadataRenameRoom, metadataPreviousMap, metadataNextMap });

            UpdateHostOnlyButons();
        }

        public void CreatePlayerButton()
        {
            playerButtonList = new PlayerButtonList();
            playerButtonList.UpdatePlayerButtonList();
        }

        public void CreateMiddleBarAnimatedButtons()
        {
            //Animated Buttons
            int buttonIndex = 0;
            int spacing = 170;
            Vector2 shiftingFactor = new Vector2(spacing / 3, 0);
            Vector2 initialOffset = new Vector2(-spacing / 2, 65);

            animatedButtonList.Add(
                AnimatedButtonBuilder.BuildButton(
                    AnimatedButtonType.ChangeMobile,
                    Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++,
                    MobileAction));

            animatedButtonList.Add(selectItem = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.ChangeItem, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, SelectItemButtonAction));

            //Make the bigger version of the options button smaller
            matchSettings = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.MatchSettings, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, (sender) => { });
            matchSettings.Flipbook.Scale = new Vector2(0.6f, 0.6f);

            animatedButtonList.Add(matchSettings);

            animatedButtonList.Add(AnimatedButtonBuilder.BuildButton(AnimatedButtonType.ChangeTeam, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, ChangeTeamAction));
        }

        public void CreateBottomBarAnimatedButtons()
        {
            //Animated Buttons
            int buttonIndex = 0;
            Vector2 shiftingFactor = new Vector2(60, 0);
            Vector2 initialOffset = new Vector2(50, 265);

            exitDoor = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.ExitDoor, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, ExitDoorAction);
            animatedButtonList.Add(exitDoor);

            animatedButtonList.Add(
                AnimatedButtonBuilder.BuildButton(
                    AnimatedButtonType.Buddy,
                    Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++,
                    (sender) => { }));

            animatedButtonList.Add(
                AnimatedButtonBuilder.BuildButton(
                    AnimatedButtonType.ReportPlayer,
                    Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++,
                    (sender) => { }));

            options = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Options, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, OptionsAction);
            animatedButtonList.Add(options);

            animatedButtonList.Add(
                AnimatedButtonBuilder.BuildButton(
                    AnimatedButtonType.MuteList,
                    Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++,
                    (sender) => { }));

            ready = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Ready, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, ReadyAction);
            animatedButtonList.Add(ready);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            lock (animatedButtonList) animatedButtonList.ForEach((x) => x.Update());
            lock (buttonList) buttonList.ForEach((x) => x.Update());
            ///buttonList.ForEach((x) => x.Update(MouseState, previousMouseState));

            lock (playerButtonList) playerButtonList.Update();

            textBox.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            lock (animatedButtonList) animatedButtonList.ForEach((x) => x.Draw(gameTime, spriteBatch));
            lock (buttonList) buttonList.ForEach((x) => x.Draw(gameTime, spriteBatch));

            lock (playerButtonList) playerButtonList.Draw(gameTime, spriteBatch);
            matchConfigurationGrid.Draw(gameTime, spriteBatch);

            spriteList.ForEach((x) => x.Draw(gameTime, spriteBatch));

            serverName.Draw(spriteBatch);

            textBox.Draw(spriteBatch);
        }

        public override void Dispose()
        {
            textBox.Dispose();
        }
    }
}