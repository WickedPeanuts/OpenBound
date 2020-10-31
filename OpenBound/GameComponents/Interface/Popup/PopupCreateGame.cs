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
using OpenBound.GameComponents.Interface.Interactive;
using OpenBound.GameComponents.Interface.Text;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.ServerCommunication;
using OpenBound.ServerCommunication.Service;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.Extension;
using System.Collections.Generic;
using System.Linq;

using Language = OpenBound.Common.Language;

namespace OpenBound.GameComponents.Interface.Popup
{
    public class PopupCreateGame : PopupMenu
    {
        private List<Button> roomSizeButtons;
        private List<Button> gameTypeButtons;

        private List<SpriteText> compositeSpriteTextSentenceList;

        private RoomMetadata roomMetadata;

        private List<TextField> textFieldList;

        public PopupCreateGame(Vector2 positionOffset) : base(false)
        {
            Background = new Sprite("Interface/Popup/Blue/CreateRoom/Background",
                position: Parameter.ScreenCenter,
                layerDepth: DepthParameter.InterfacePopupBackground);

            buttonList.Add(new Button(ButtonType.CreateGame1v1, DepthParameter.InterfacePopupButtons, (sender) => { CreateGameRoomSizeButtonAction(sender, RoomSize.OneVsOne); }));
            buttonList.Add(new Button(ButtonType.CreateGame2v2, DepthParameter.InterfacePopupButtons, (sender) => { CreateGameRoomSizeButtonAction(sender, RoomSize.TwoVsTwo); }));
            buttonList.Add(new Button(ButtonType.CreateGame3v3, DepthParameter.InterfacePopupButtons, (sender) => { CreateGameRoomSizeButtonAction(sender, RoomSize.ThreeVsThree); }));
            buttonList.Add(new Button(ButtonType.CreateGame4v4, DepthParameter.InterfacePopupButtons, (sender) => { CreateGameRoomSizeButtonAction(sender, RoomSize.FourVsFour); }));

            buttonList.Add(new Button(ButtonType.CreateGameJewel, DepthParameter.InterfacePopupButtons, (sender) => { CreateGameGameModeButtonAction(sender, GameMode.Jewel); }));
            buttonList.Add(new Button(ButtonType.CreateGameTag,   DepthParameter.InterfacePopupButtons, (sender) => { CreateGameGameModeButtonAction(sender, GameMode.Tag); }));
            buttonList.Add(new Button(ButtonType.CreateGameScore, DepthParameter.InterfacePopupButtons, (sender) => { CreateGameGameModeButtonAction(sender, GameMode.Score); }));
            buttonList.Add(new Button(ButtonType.CreateGameSolo,  DepthParameter.InterfacePopupButtons, (sender) => { CreateGameGameModeButtonAction(sender, GameMode.Solo); }));

            buttonList.Add(new Button(ButtonType.Accept, DepthParameter.InterfacePopupButtons, CreateRoomAction));
            buttonList.Add(new Button(ButtonType.Cancel, DepthParameter.InterfacePopupButtons, CloseAction));

            compositeSpriteTextSentenceList = new List<SpriteText>();
            compositeSpriteTextSentenceList.Add(new SpriteText(FontTextType.Consolas10, "", Color.White, Alignment.Left, DepthParameter.InterfacePopupText, outlineColor: Color.Black));
            compositeSpriteTextSentenceList.Add(new SpriteText(FontTextType.Consolas10, "", Color.White, Alignment.Left, DepthParameter.InterfacePopupText, outlineColor: Color.Black));

            compositeSpriteTextList.Add(CompositeSpriteText.CreateCompositeSpriteText(compositeSpriteTextSentenceList, Orientation.Vertical, Alignment.Left, new Vector2(0, 0), 5));

            compositeSpriteTextList.Add(CompositeSpriteText.CreateCompositeSpriteText(
                new List<SpriteText>()
                {
                    new SpriteText(FontTextType.Consolas10, Language.PopupCreateRoomTitle,    Color.White, Alignment.Left, DepthParameter.InterfacePopupText, outlineColor: Color.Black),
                    new SpriteText(FontTextType.Consolas10, Language.PopupCreateRoomPassword, Color.White, Alignment.Left, DepthParameter.InterfacePopupText, outlineColor: Color.Black)
                },
                Orientation.Vertical, Alignment.Right, Vector2.Zero, 9));

            //Button Texts
            spriteTextList.Add(new SpriteText(FontTextType.Consolas10, Language.GameModeJewel, Color.White, Alignment.Center, DepthParameter.InterfacePopupText));
            spriteTextList.Add(new SpriteText(FontTextType.Consolas10, Language.GameModeTag, Color.White, Alignment.Center, DepthParameter.InterfacePopupText));
            spriteTextList.Add(new SpriteText(FontTextType.Consolas10, Language.GameModeScore, Color.White, Alignment.Center, DepthParameter.InterfacePopupText));
            spriteTextList.Add(new SpriteText(FontTextType.Consolas10, Language.GameModeSolo, Color.White, Alignment.Center, DepthParameter.InterfacePopupText));

            //Room default configurations
            buttonList[3].ChangeButtonState(ButtonAnimationState.Activated, true);
            buttonList[7].ChangeButtonState(ButtonAnimationState.Activated, true);

            //Button variable helpers
            roomMetadata = new RoomMetadata(GameMode.Score, TurnsToSuddenDeath.Turn56, MatchSuddenDeathType.NoDeath, RoomSize.FourVsFour,
                GameInformation.Instance.PlayerInformation, 0, "", "");

            roomSizeButtons = buttonList.GetRange(0, 4);
            gameTypeButtons = buttonList.GetRange(4, 4);

            roomSizeButtons.Union(gameTypeButtons).ToList().ForEach((x) => x.OnBeingReleased = (sender) => ((Button)sender).ChangeButtonState(ButtonAnimationState.Activated, true));

            //Textfields
            textFieldList = new List<TextField>() {
                new TextField(default, 185, 14, 16, FontTextType.Consolas10, Color.White, DepthParameter.InterfacePopupText, outlineColor: Color.Black),
                new TextField(default, 185, 14, 16, FontTextType.Consolas10, Color.White, DepthParameter.InterfacePopupText, outlineColor: Color.Black),
            };

            textFieldList[0].TabIndex = textFieldList[1].TabIndex = textFieldList;

            PositionOffset = positionOffset;

            #warning Pre-release deactivation
            roomMetadata.GameMode = GameMode.Solo;
            buttonList[4].Disable();
            buttonList[5].Disable();
            buttonList[6].Disable();

            UpdateText();
            UpdateAttatchmentPosition();

            //Networking
            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerRoomListCreateRoom, CreateRoomAsyncCallback);
        }

        private void CreateRoomAction(object sender)
        {
            if (textFieldList[0].Text.Text.Length == 0)
            { textFieldList[0].Text.Text = Language.PopupCreateRoomNamePlaceholder; }

            roomMetadata.Name = textFieldList[0].Text.Text;
            roomMetadata.Password = textFieldList[1].Text.Text;

            GameInformation.Instance.RoomMetadata = roomMetadata;

            ServerInformationHandler.CreateRoom(roomMetadata);

            buttonList.ForEach((x) => x.Disable());
            textFieldList.ForEach((x) => x.Disable());
        }

        private void CreateRoomAsyncCallback(object answer)
        {
            if (answer == null)
            {
                buttonList.ForEach((x) => x.Disable());
                textFieldList.ForEach((x) => x.Disable());
                return;
            }

            GameInformation.Instance.RoomMetadata = (RoomMetadata)answer;

            SceneHandler.Instance.RequestSceneChange(SceneType.GameRoom, TransitionEffectType.RotatingRectangles);
        }

        private void CreateGameRoomSizeButtonAction(object sender, RoomSize roomSize)
        {
            roomSizeButtons.Where((x) => x != sender).ToList().ForEach((x) =>
            {
                x.Disable();
                x.Enable();
            });

            roomMetadata.Size = roomSize;

            UpdateText();
            UpdateAttatchmentPosition();
        }

        private void CreateGameGameModeButtonAction(object sender, GameMode gameMode)
        {
            gameTypeButtons.Where((x) => x != sender).ToList().ForEach((x) =>
            {
                x.Disable();
                x.Enable();
            });

            roomMetadata.GameMode = gameMode;

            UpdateText();
            UpdateAttatchmentPosition();

#warning Pre-release deactivation
            buttonList[4].Disable();
            buttonList[5].Disable();
            buttonList[6].Disable();
        }

        private void UpdateText()
        {
            string firstLineText = "", secondLineText = "";

            switch (roomMetadata.Size)
            {
                case RoomSize.OneVsOne:
                    firstLineText = Language.PopupCreateRoomOneVersusOneText;
                    break;
                case RoomSize.TwoVsTwo:
                    firstLineText = Language.PopupCreateRoomTwoVersusTwoText;
                    break;
                case RoomSize.ThreeVsThree:
                    firstLineText = Language.PopupCreateRoomThreeVersusThreeText;
                    break;
                case RoomSize.FourVsFour:
                    firstLineText = Language.PopupCreateRoomFourVersusFourText;
                    break;
            }

            switch (roomMetadata.GameMode)
            {
                case GameMode.Score:
                    secondLineText = Language.PopupCreateRoomScoreText;
                    break;
                case GameMode.Solo:
                    secondLineText = Language.PopupCreateRoomSoloText;
                    break;
                case GameMode.Tag:
                    secondLineText = Language.PopupCreateRoomTagText;
                    break;
                case GameMode.Jewel:
                    secondLineText = Language.PopupCreateRoomJewelText;
                    break;
            }

            compositeSpriteTextSentenceList[0].Text = firstLineText;
            compositeSpriteTextSentenceList[1].Text = secondLineText;
        }

        protected override void UpdateAttatchmentPosition()
        {
            Background.Position = positionOffset - GameScene.Camera.CameraOffset;

            compositeSpriteTextList[0].Position = positionOffset - GameScene.Camera.CameraOffset - new Vector2(155, -25);
            compositeSpriteTextList[1].Position = positionOffset - GameScene.Camera.CameraOffset - new Vector2(55, 91);

            buttonList[0].ButtonOffset = positionOffset - new Vector2(120, 30);
            buttonList[1].ButtonOffset = positionOffset - new Vector2(60, 30);
            buttonList[2].ButtonOffset = positionOffset - new Vector2(90, 0);
            buttonList[3].ButtonOffset = positionOffset - new Vector2(30, 0);

            buttonList[4].ButtonOffset = positionOffset + new Vector2(120, 0);
            buttonList[5].ButtonOffset = positionOffset + new Vector2(60, 0);
            buttonList[6].ButtonOffset = positionOffset + new Vector2(90, -30);
            buttonList[7].ButtonOffset = positionOffset + new Vector2(30, -30);

            spriteTextList[0].Position = buttonList[4].ButtonOffset - GameScene.Camera.CameraOffset + new Vector2(0, 5);
            spriteTextList[1].Position = buttonList[5].ButtonOffset - GameScene.Camera.CameraOffset + new Vector2(0, 5);
            spriteTextList[2].Position = buttonList[6].ButtonOffset - GameScene.Camera.CameraOffset + new Vector2(0, 5);
            spriteTextList[3].Position = buttonList[7].ButtonOffset - GameScene.Camera.CameraOffset + new Vector2(0, 5);

            buttonList[8].ButtonOffset = positionOffset + new Vector2(100, 95);
            buttonList[9].ButtonOffset = positionOffset + new Vector2(135, 95);

            textFieldList[0].Position = positionOffset - GameScene.Camera.CameraOffset + new Vector2(-55, -91);
            textFieldList[1].Position = positionOffset - GameScene.Camera.CameraOffset + new Vector2(-55, -67);
        }

        public override void Update(GameTime gameTime)
        {
            if (!ShouldRender) return;

            base.Update(gameTime);
            textFieldList.ForEach((x) => x.Update(gameTime));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!ShouldRender) return;

            base.Draw(gameTime, spriteBatch);

            textFieldList.ForEach((x) => x.Draw(spriteBatch));
        }
    }
}
