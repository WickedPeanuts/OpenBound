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
using OpenBound.GameComponents.Interface.Interactive.GameList;
using OpenBound.GameComponents.Interface.Popup;
using OpenBound.GameComponents.Interface.Text;
using OpenBound.ServerCommunication;
using OpenBound.ServerCommunication.Service;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Text;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.Extension;

namespace OpenBound.GameComponents.Level.Scene.Menu
{
    class GameList : GameScene
    {
        private List<AnimatedButton> animatedButtonList;

        private AnimatedButton leftArrow, rightArrow;
        private AnimatedButton soloFilter, scoreFilter, tagFilter, jewelFilter, viewAllFilter, viewFriendsFilter, viewWaitingFilter;

        private AnimatedButton options;

        private List<RoomButton> roomButtonList;
        private PopupCreateGame createGamePopup;

        private List<RoomMetadata> requestedRoomMetadataList;
        private RoomMetadata roomFilter;

        //Textbox
        private TextBox textBox;

        //Button positioning variables
        private readonly int widthFactor;
        private readonly int buttonHeightPosition;
        private readonly int heightFactor;

        //Online users
        private OnlineUserList onlineUserList;
        private List<Button> channelButtonList;

        //Button previous state saving
        private List<bool> buttonState;

        public GameList()
        {
            animatedButtonList = new List<AnimatedButton>();
            roomButtonList = new List<RoomButton>();
            requestedRoomMetadataList = new List<RoomMetadata>();

            Background = new Sprite(@"Interface/InGame/Scene/GameList/Background",
                position: Parameter.ScreenCenter,
                layerDepth: DepthParameter.Background,
                shouldCopyAsset: false);

            CreateBottomBarAnimatedButtons();
            CreateRoomFilteringAnimatedButtons();

            //popups
            PopupHandler.PopupGameOptions.OnClose = OptionsCloseAction;

            createGamePopup = new PopupCreateGame(Vector2.Zero);
            createGamePopup.OnClose = (sender) => { UnlockAllInterfaceElements(); };

            PopupHandler.Add(createGamePopup);

            //button positioning variables
            widthFactor = Background.SpriteWidth / 3;
            heightFactor = 69;
            buttonHeightPosition = 140 - (int)Parameter.ScreenCenter.Y;

            //Room Filtering
            roomFilter = new RoomMetadata(GameMode.Any, default, default, default, default, 0, default, default);
            roomFilter.IsPlaying = false;
            roomFilter.PageNumber = 0;

            //Textbox 
            textBox = new TextBox(new Vector2(-380, 40), new Vector2(493, 141), 100, 0,
                hasScrollBar: true, scrollBackgroundAlpha: 0.6f,
                hasTextField: true, textFieldBackground: 0, textFieldOffset: new Vector2(20, 0), maximumTextLength: 60,
                onSendMessage: OnSendMessage);

            textBox.EnableTextField();

            //Online users list
            onlineUserList = new OnlineUserList(new Vector2(140, 40), new Vector2(250, 166));
            channelButtonList = new List<Button>();

            channelButtonList.Add(new Button(ButtonType.ChannelListButton1, DepthParameter.InterfaceButton, (o) => { ChannelListAction(1); }, new Vector2(-47, 28)));
            channelButtonList.Add(new Button(ButtonType.ChannelListButton2, DepthParameter.InterfaceButton, (o) => { ChannelListAction(2); }, new Vector2(-26 - 2, 28)));
            channelButtonList.Add(new Button(ButtonType.ChannelListButton3, DepthParameter.InterfaceButton, (o) => { ChannelListAction(3); }, new Vector2(-1 - 4, 29)));
            channelButtonList.Add(new Button(ButtonType.ChannelListButton4, DepthParameter.InterfaceButton, (o) => { ChannelListAction(4); }, new Vector2(23 - 6, 28)));
            channelButtonList.Add(new Button(ButtonType.ChannelListButton5, DepthParameter.InterfaceButton, (o) => { ChannelListAction(5); }, new Vector2(44 - 8, 29)));
            channelButtonList.Add(new Button(ButtonType.ChannelListButton6, DepthParameter.InterfaceButton, (o) => { ChannelListAction(6); }, new Vector2(68 - 10 + 1, 29)));
            channelButtonList.Add(new Button(ButtonType.ChannelListButton7, DepthParameter.InterfaceButton, (o) => { ChannelListAction(7); }, new Vector2(92 - 12, 29)));
            channelButtonList.Add(new Button(ButtonType.ChannelListButton8, DepthParameter.InterfaceButton, (o) => { ChannelListAction(8); }, new Vector2(116 - 14, 30)));

            channelButtonList.ForEach((x) => x.Disable());

            //Textual callbacks
            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerChatJoinChannel, RequestJoinChannelAsyncCallback);
            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerChatEnter, RequestChatConnectionAsyncCallback);
            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerChatLeave, RequestChatDisconnectionAsyncCallback);
            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerChatSendSystemMessage, OnReceiveMessageAsyncCallback);
            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerChatSendPlayerMessage, OnReceiveMessageAsyncCallback);

            //Game Lists callbacks
            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerRoomListRequestList, RequestRoomListAsyncCallback);

            viewWaitingFilter.Disable(true);
            leftArrow.Disable(true);
            rightArrow.Disable(true);

            RequestRooms();

            //Connect to channel
            ServerInformationHandler.SendChatConnectionRequest(Message.BuildGameServerChatGameList(0));

            #warning Pre-release deactivation
            animatedButtonList[1].Disable();
            animatedButtonList[2].Disable();
            animatedButtonList[3].Disable();
            animatedButtonList[5].Disable();
            animatedButtonList[6].Disable();
            //animatedButtonList[12].Disable();
            //animatedButtonList[13].Disable();
            //animatedButtonList[14].Disable();
            animatedButtonList[15].Disable();
            //animatedButtonList[16].Disable();
            animatedButtonList[17].Disable();
            animatedButtonList[19].Disable();
        }

        public override void OnSceneIsActive()
        {
            base.OnSceneIsActive();
            AudioHandler.ChangeSong(SongParameter.ServerAndRoomSelection, ChangeEffect.Fade);
        }

        #region Button State Manipulation
        private void LockAllInterfaceElements()
        {
            buttonState = new List<bool>();
            animatedButtonList.ForEach((x) => buttonState.Add(x.IsDisabled));
            animatedButtonList.ForEach((x) => x.Disable(true));
            roomButtonList.ForEach((x) => x.ShouldUpdate = false);
            textBox.DisableTextField();
        }

        private void UnlockAllInterfaceElements()
        {
            animatedButtonList.ForEach((x) => x.Enable());
            roomButtonList.ForEach((x) => x.ShouldUpdate = true);

            for (int i = 0; i < buttonState.Count; i++)
            {
                if (buttonState[i])
                {
                    animatedButtonList[i].Disable(true);
                }
                else
                {
                    animatedButtonList[i].Enable();
                }
            }


            #warning Pre-release deactivation
            animatedButtonList[1].Disable();
            animatedButtonList[2].Disable();
            animatedButtonList[3].Disable();
            animatedButtonList[5].Disable();
            animatedButtonList[6].Disable();
            //animatedButtonList[12].Disable();
            //animatedButtonList[13].Disable();
            //animatedButtonList[14].Disable();
            animatedButtonList[15].Disable();
            //animatedButtonList[16].Disable();
            animatedButtonList[17].Disable();
            animatedButtonList[19].Disable();

            textBox.EnableTextField();
        }
        #endregion

        #region Networking
        public void RequestRoomListAsyncCallback(object answer)
        {
            lock (requestedRoomMetadataList)
            {
                requestedRoomMetadataList.Add((RoomMetadata)answer);
            }
        }

        private void RequestRooms()
        {
            leftArrow.Disable(); rightArrow.Disable();

            roomButtonList.Clear();
            ServerInformationHandler.GameServerRequestRoomList(roomFilter);
        }

        private void ConnectToRoomAsyncCallback(object answer)
        {
            RoomMetadata room = (RoomMetadata)answer;

            if (room == null)
            {
                UnlockAllInterfaceElements();
                return;
            }

            GameInformation.Instance.RoomMetadata = room;
            SceneHandler.Instance.RequestSceneChange(SceneType.GameRoom, TransitionEffectType.RotatingRectangles);
        }
        #endregion

        #region Button Actions

        #region Room Manipulation
        private void CreateRoomAction(object sender)
        {
            LockAllInterfaceElements();
            ((AnimatedButton)sender).Disable();
            createGamePopup.ShouldRender = true;
        }

        private void ConnectToRoom(object sender, RoomMetadata roomMetadata)
        {
            LockAllInterfaceElements();
            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerRoomListRoomEnter, ConnectToRoomAsyncCallback);
            ServerInformationHandler.ConnectToRoom(roomMetadata);
        }
        #endregion

        #region Arrows
        private void LeftArrowAction(object sender)
        {
            roomFilter.PageNumber--;

            if (roomFilter.PageNumber == 0)
                leftArrow.Disable();
            else
                leftArrow.ChangeButtonState(ButtonAnimationState.Activated);

            RequestRooms();
        }

        private void RightArrowAction(object sender)
        {
            roomFilter.PageNumber++;

            RequestRooms();

            leftArrow.Enable();
        }
        #endregion

        #region Filtering
        private void SoloFilterAction(object sender)
        {
            roomFilter.GameMode = GameMode.Solo;
            roomFilter.PageNumber = 0;
            roomFilter.IsPlaying = false;

            viewAllFilter.Enable();

            soloFilter.Disable();
            scoreFilter.Enable(); jewelFilter.Enable(); tagFilter.Enable();

            RequestRooms();
        }

        private void ScoreFilterAction(object sender)
        {
            roomFilter.GameMode = GameMode.Score;
            roomFilter.PageNumber = 0;
            roomFilter.IsPlaying = false;

            viewAllFilter.Enable();

            scoreFilter.Disable();
            soloFilter.Enable(); jewelFilter.Enable(); tagFilter.Enable();
            RequestRooms();
        }

        private void JewelFilterAction(object sender)
        {
            roomFilter.GameMode = GameMode.Jewel;
            roomFilter.PageNumber = 0;
            roomFilter.IsPlaying = false;

            viewAllFilter.Enable();

            jewelFilter.Disable();
            scoreFilter.Enable(); soloFilter.Enable(); tagFilter.Enable();
            RequestRooms();
        }

        private void TagFilterAction(object sender)
        {
            roomFilter.GameMode = GameMode.Tag;
            roomFilter.PageNumber = 0;
            roomFilter.IsPlaying = false;

            viewAllFilter.Enable();

            tagFilter.Disable();
            scoreFilter.Enable(); jewelFilter.Enable(); soloFilter.Enable();
            RequestRooms();
        }

        private void ViewAllFilterAction(object sender)
        {
            roomFilter.PageNumber = 0;
            roomFilter.IsPlaying = true;

            viewAllFilter.Disable();
            viewFriendsFilter.Enable(); viewWaitingFilter.Enable();

            roomFilter.GameMode = GameMode.Any;
            scoreFilter.Enable(); jewelFilter.Enable(); tagFilter.Enable(); soloFilter.Enable();

            RequestRooms();
        }

        private void ViewWaitingFilterAction(object sender)
        {
            roomFilter.PageNumber = 0;
            roomFilter.IsPlaying = false;

            viewWaitingFilter.Disable();
            viewFriendsFilter.Enable(); viewAllFilter.Enable();
            RequestRooms();
        }

        private void ViewFriendsFilterAction(object sender)
        {
            roomFilter.PageNumber = 0;
            roomFilter.IsPlaying = false;

            viewFriendsFilter.Disable();
            viewWaitingFilter.Enable(); viewAllFilter.Enable();

            RequestRooms();
        }
        #endregion

        private void ChannelListAction(int channelIndex)
        {
            channelButtonList.ForEach((x) => x.Disable());
            ServerInformationHandler.SendChatConnectionRequest(Message.BuildGameServerChatGameList(channelIndex));
        }

        private void AvatarShopAction(object sender)
        {
            ServerInformationHandler.SendChatDisconnectionRequest();
            SceneHandler.Instance.RequestSceneChange(SceneType.AvatarShop, TransitionEffectType.RotatingRectangles);
        }

        private void ExitDoorAction(object sender)
        {
            ServerInformationBroker.Instance.DisconnectFromGameServer();
            SceneHandler.Instance.RequestSceneChange(SceneType.ServerSelection, TransitionEffectType.RotatingRectangles);
            ((AnimatedButton)sender).Disable();
        }

        private void OptionsCloseAction(object sender)
        {
            roomButtonList.ForEach((x) => x.ShouldUpdate = true);
            animatedButtonList.ForEach((x) => x.ShouldUpdate = true);
            options.Enable();
        }

        private void OptionsAction(object sender)
        {
            roomButtonList.ForEach((x) => x.ShouldUpdate = false);
            animatedButtonList.ForEach((x) => x.ShouldUpdate = false);
            PopupHandler.PopupGameOptions.ShouldRender = true;
            options.Disable();
        }

        #endregion

        #region Textbox
        //Textbox channels
        public void RequestJoinChannelAsyncCallback(object response)
        {
            int r = (int)response;
            if (r == 0)
            {
                //Error
            }
            else
            {
                for (int i = 0; i < channelButtonList.Count; i++)
                    if (i + 1 != r)
                        channelButtonList[i].Enable();
            }
        }

        //Whenever a player tries to connect to a channel this method receives a stream of connected players
        //Including your own credentials. If it returns null, the server could not 
        public void RequestChatConnectionAsyncCallback(object response)
        {
            Player player = (Player)response;

            //User has connected to a chat
            if (player.ID == GameInformation.Instance.PlayerInformation.ID)
                onlineUserList.Clear();

            onlineUserList.AppendNameplate(player);
        }

        //Whenever a player leaves a channel this method receives this player.
        public void RequestChatDisconnectionAsyncCallback(object response)
        {
            Player player = (Player)response;
            onlineUserList.RemoveNameplate(player);
        }

        public void OnSendMessage(PlayerMessage message)
        {
            ServerInformationHandler.SendGameListMessage(message);
        }

        public void OnReceiveMessageAsyncCallback(object message)
        {
            textBox.AsyncAppendText(message);
        }
        #endregion

        public void CreateListedRoomButtons()
        {
            lock (requestedRoomMetadataList)
            {
                while (requestedRoomMetadataList.Count > 0)
                {
                    RoomMetadata room = requestedRoomMetadataList[0];

                    requestedRoomMetadataList.RemoveAt(0);

                    //On the tenth attempt to create a button...
                    if (roomButtonList.Count == 9)
                    {
                        rightArrow.Enable();
                        requestedRoomMetadataList.Clear();
                        break;
                    }

                    int i = roomButtonList.Count;
                    RoomButton newButton = RoomButton.CreateRoomButton(
                        room,
                        new Vector2(((i % 3) - 1) * widthFactor, buttonHeightPosition + heightFactor * (i / 3)),
                        (sender) => { ConnectToRoom(sender, room); });
                    roomButtonList.Add(newButton);

                    if (createGamePopup.ShouldRender)
                        newButton.ShouldUpdate = false;
                }
            }
        }

        public void CreateRoomFilteringAnimatedButtons()
        {
            //Animated Buttons
            float buttonIndex = 0;
            Vector2 shiftingFactor = new Vector2(50, 0);
            Vector2 initialOffset = new Vector2(-350, -40);

            #region GameModes
            soloFilter = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Solo, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, SoloFilterAction);
            tagFilter = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Tag, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, TagFilterAction);
            scoreFilter = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Score, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, ScoreFilterAction);
            jewelFilter = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Jewel, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, JewelFilterAction);
            animatedButtonList.Add(soloFilter);
            animatedButtonList.Add(tagFilter);
            animatedButtonList.Add(scoreFilter);
            animatedButtonList.Add(jewelFilter);
            #endregion

            #region RoomNavigation
            buttonIndex += 0.6f;
            leftArrow = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.LeftArrow, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, LeftArrowAction);
            rightArrow = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.RightArrow, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, RightArrowAction);
            animatedButtonList.Add(leftArrow);
            animatedButtonList.Add(rightArrow);
            #endregion

            #region RoomStatus

            buttonIndex += 0.6f;

            viewAllFilter = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.ViewAll, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, ViewAllFilterAction);
            animatedButtonList.Add(viewAllFilter);

            buttonIndex += 0.1f;

            viewWaitingFilter = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.ViewWaiting, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, ViewWaitingFilterAction);
            animatedButtonList.Add(viewWaitingFilter);

            buttonIndex += 0.1f;

            viewFriendsFilter = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.ViewFriend, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, ViewFriendsFilterAction);
            animatedButtonList.Add(viewFriendsFilter);

            buttonIndex += 0.1f;

            animatedButtonList.Add(
                AnimatedButtonBuilder.BuildButton(
                    AnimatedButtonType.GoTo,
                    Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++,
                    (sender) => { }));
            #endregion

            #region RoomOptions
            initialOffset += new Vector2(40, 5);


            animatedButtonList.Add(
                AnimatedButtonBuilder.BuildButton(
                    AnimatedButtonType.Create,
                    Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, CreateRoomAction));

            buttonIndex += 0.4f;

            animatedButtonList.Add(
                AnimatedButtonBuilder.BuildButton(
                    AnimatedButtonType.QuickJoin,
                    Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++,
                    (sender) => { }));
            #endregion
        }

        public void CreateBottomBarAnimatedButtons()
        {
            //Animated Buttons
            int buttonIndex = 0;
            Vector2 shiftingFactor = new Vector2(80, 0);
            Vector2 initialOffset = new Vector2(-330, 265);

            animatedButtonList.Add(AnimatedButtonBuilder.BuildButton(AnimatedButtonType.ExitDoor, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++, ExitDoorAction));

            animatedButtonList.Add(
                AnimatedButtonBuilder.BuildButton(
                    AnimatedButtonType.Buddy,
                    Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++,
                    (sender) => { }));

            animatedButtonList.Add(
                AnimatedButtonBuilder.BuildButton(
                    AnimatedButtonType.CashCharge,
                    Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++,
                    (sender) => { }));

            animatedButtonList.Add(
                AnimatedButtonBuilder.BuildButton(
                    AnimatedButtonType.ReportPlayer,
                    Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex++,
                    (sender) => { }));

            animatedButtonList.Add(AnimatedButtonBuilder.BuildButton(AnimatedButtonType.AvatarShop, Parameter.ScreenCenter + initialOffset + shiftingFactor * (buttonIndex + 0.5f), AvatarShopAction));

            initialOffset = new Vector2(-initialOffset.X, initialOffset.Y);
            buttonIndex = 0;

            animatedButtonList.Add(
             AnimatedButtonBuilder.BuildButton(
                    AnimatedButtonType.MuteList,
                    Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex--,
                    (sender) => { }));

            animatedButtonList.Add(
                AnimatedButtonBuilder.BuildButton(
                    AnimatedButtonType.MyInfo,
                    Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex--,
                    (sender) => { }));

            options = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Options, Parameter.ScreenCenter + initialOffset + shiftingFactor * buttonIndex--, OptionsAction);
            animatedButtonList.Add(options);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            CreateListedRoomButtons();

            animatedButtonList.ForEach((x) => x.Update());
            roomButtonList.ForEach((x) => x.Update());

            textBox.Update(gameTime);
            onlineUserList.Update();

            channelButtonList.ForEach((x) => x.Update());
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            animatedButtonList.ForEach((x) => x.Draw(gameTime, spriteBatch));
            roomButtonList.ForEach((x) => x.Draw(gameTime, spriteBatch));
            textBox.Draw(spriteBatch);
            onlineUserList.Draw(spriteBatch);
            channelButtonList.ForEach((x) => x.Draw(gameTime, spriteBatch));
        }

        public override void Dispose()
        {
            textBox.Dispose();
        }
    }
}
