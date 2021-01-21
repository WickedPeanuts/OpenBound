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
using OpenBound.GameComponents.Interface.Interactive;
using OpenBound.GameComponents.Interface.Interactive.Misc;
using OpenBound.GameComponents.Interface.Popup;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Pawn;
using OpenBound_Network_Object_Library.Entity;
using System.Collections.Generic;
using OpenBound_Network_Object_Library.Models;
using OpenBound.GameComponents.Interface.Text;
using OpenBound_Network_Object_Library.Entity.Text;
using Microsoft.Xna.Framework.Input;
using OpenBound.GameComponents.Input;
using System.Linq;
using OpenBound.ServerCommunication.Service;
using System;
using Microsoft.Xna.Framework.Audio;
using OpenBound.GameComponents.Asset;
using OpenBound.GameComponents.Audio;
using OpenBound.Extension;
using OpenBound_Network_Object_Library.Common;
using Language = OpenBound.Common.Language;
using OpenBound_Network_Object_Library.Extension;

namespace OpenBound.GameComponents.Interface
{
    public class HUD
    {
        public TimedProgressBar StrenghtBar { get; private set; }
        public ProgressBar MovementBar { get; private set; }

        private List<Sprite> spriteList;
        private Sprite strenghtBarPreviousShotMarker, shotStrenghtSelector;

        private ShotButton shot1Button, shot2Button, shotSSButton;
        private List<Button> menuButtonList;

        private List<SpriteNumericField> spriteNumericFieldList;
        public CurrencySpriteFont GoldNumericField;

        private NumericSpriteFont currentAngle;
        private NumericSpriteFont previousAngle;

        private readonly Mobile mobile;
        private Mobile currentTurnOwner;
        private Flipbook turnCounterBallooon;

        //All game floating texts
        public FloatingTextHandler FloatingTextHandler;

        //Units lifebars
        private List<HealthBar> healthBarList;
        private List<Nameplate> nameplateList;

        //Weather effects
        public WindCompass WindCompass;
        public WeatherDisplay WeatherDisplay;

        //Static popups
        private Button gameOptionsButton;
        private PopupGameOptions popupGameOptions;

        //Texts
        private Button textFilterButton;
        private SpriteText allTextFilterSpriteFont, teamTextFilterSpriteFont;

        //Pass Turn button
        private Button passTurnButton;
        public Delayboard Delayboard;

        //Textboxes
        public TextBox[] TextBoxes;
        float[] textBoxesTransparencyElapsedTime;
        float[] textBoxesTransparencyFadeTime;
        public bool IsChatOpen { get; private set; }

        //Items
        public EquippedButtonBar EquippedButtonBar;
        public Sprite MatchStatusClosed;

        Action useItemOnTurnBeginAction, removeItemFromBarAction;
        Sprite matchStatus;

        //Status Bar
        public Dictionary<Mobile, AttributeStatusBar> StatusBarDictionary;

        //Turn Counter
        private NumericSpriteFont turnCounter;
        private double turnCounterCurrentTime;
        private bool turnCounterShouldUpdate;

        //Sound Effects
        SoundEffect goldAcquisitionSE;

        private Vector2 origin;

        public HUD(Mobile mobile, List<Mobile> mobileList)
        {
            //Attributions
            this.mobile = mobile;

            //Initializing lists
            RoomMetadata room = GameInformation.Instance.RoomMetadata;

            spriteList = new List<Sprite>();
            menuButtonList = new List<Button>();
            FloatingTextHandler = new FloatingTextHandler();
            spriteNumericFieldList = new List<SpriteNumericField>();

            //Background
            Sprite barBG = new Sprite("Interface/InGame/HUD/Blue/ActionBar/Background", layerDepth: DepthParameter.HUDBackground);
            spriteList.Add(barBG);

            //Defining the origin
            origin = new Vector2(0, (Parameter.ScreenResolution.Y / GameScene.Camera.Zoom.Y - barBG.SpriteHeight) / 2).ToIntegerDomain();
            barBG.PositionOffset = origin;

            //Right Menu (TAG || SCORE)
            if (room.GameMode == GameMode.Score)
            {
                Sprite rightMenuScoreboard = new Sprite("Interface/InGame/HUD/Blue/ActionBar/RightMenuScoreBoard", layerDepth: DepthParameter.HUDForeground,
                        sourceRectangle: (this.mobile.Owner.PlayerTeam == PlayerTeam.Red) ? new Rectangle(0, 0, 149, 63) : new Rectangle(149, 0, 149, 63))
                { PositionOffset = new Vector2(401, -11) + origin };
                spriteList.Add(rightMenuScoreboard);

                //Team Score - Left
                spriteNumericFieldList.Add(new NumericSpriteFont(FontType.HUDBlueScoreboard, 1, DepthParameter.HUDL5,
                    PositionOffset: ((this.mobile.Owner.PlayerTeam == PlayerTeam.Red) ? new Vector2(318, -5) : new Vector2(392, -5)) + origin,
                    StartingValue: 2, textAnchor: TextAnchor.Right));

                //Team Score - Right
                spriteNumericFieldList.Add(new NumericSpriteFont(FontType.HUDBlueScoreboard, 1, DepthParameter.HUDL5,
                    PositionOffset: ((this.mobile.Owner.PlayerTeam == PlayerTeam.Blue) ? new Vector2(318, -5) : new Vector2(392, -5)) + origin,
                    StartingValue: 2, textAnchor: TextAnchor.Right));
            }
            else if (room.GameMode == GameMode.Tag)
            {
                Sprite rightMenuTag = new Sprite("Interface/InGame/HUD/Blue/ActionBar/RightMenuTag", layerDepth: DepthParameter.HUDForeground)
                { PositionOffset = new Vector2(326, 1) + origin };
                spriteList.Add(rightMenuTag);

                menuButtonList.Add(new Button(ButtonType.BlueTag, DepthParameter.HUDL1, default, new Vector2(288, 9) + origin));
            }

            //GoldCounter
            spriteNumericFieldList.Add(GoldNumericField = new CurrencySpriteFont(FontType.HUDBlueGold, 6, DepthParameter.HUDL5,
                positionOffset: new Vector2(372, 23) + origin, textAnchor: TextAnchor.Right));

            //Shot Strenght Metter
            StrenghtBar = new TimedProgressBar("Interface/InGame/HUD/Blue/ActionBar/StrenghtMetter",
                DepthParameter.HUDForeground, Parameter.ShotStrenghtTimeLimit,
                Parameter.ShotStrenghtBarStep, new Vector2(41, 17) + origin, true);

            strenghtBarPreviousShotMarker = new Sprite("Interface/InGame/HUD/Blue/ActionBar/PreviousShootingStrenghtPointer",
                layerDepth: DepthParameter.HUDL1)
            { PositionOffset = new Vector2(-160, 26) + origin };
            spriteList.Add(strenghtBarPreviousShotMarker);

            shotStrenghtSelector = new Sprite("Interface/InGame/HUD/Blue/ActionBar/ShotStrenghtSelector",
                layerDepth: DepthParameter.HUDL2)
            { PositionOffset = new Vector2(-160, 17) + origin };
            spriteList.Add(shotStrenghtSelector);

            //Movement Metter
            MovementBar = new ProgressBar("Interface/InGame/HUD/Blue/ActionBar/MovementMetter",
                DepthParameter.HUDForeground, -100f / this.mobile.Movement.MaximumStepsPerTurn,
                new Vector2(41, 35) + origin);

            //Reset HUD components
            MovementBar.Reset();

            //Panel that spawns above the items
            EquippedButtonBar = new EquippedButtonBar(new Vector2(-116, -37) + origin,
                defaultLayerIndex: DepthParameter.HUDL2,
                onClick: UseItemAction);

            MatchStatusClosed = new Sprite("Interface/InGame/HUD/Blue/ActionBar/MatchStatus", layerDepth: DepthParameter.HUDL3,
                sourceRectangle: new Rectangle(0, 0, 217, 35))
            {
                PositionOffset = new Vector2(318, -20) + origin,
                Color = Color.Transparent,
            };

            spriteList.Add(MatchStatusClosed);

            matchStatus = new Sprite("Interface/InGame/HUD/Blue/ActionBar/MatchStatus", layerDepth: DepthParameter.HUDL4,
                sourceRectangle: new Rectangle(((int)room.MatchSuddenDeathType + 1) * 217, 0, 217, 35))
            {
                PositionOffset = new Vector2(318, -20) + origin,
                Color = Color.Transparent,
            };

            spriteList.Add(matchStatus);

            currentAngle = new NumericSpriteFont(FontType.HUDBlueCurrentAngle, 3, DepthParameter.HUDL5,
                PositionOffset: new Vector2(-149, -26) + origin, textAnchor: TextAnchor.Right);
            spriteNumericFieldList.Add(currentAngle);

            previousAngle = new NumericSpriteFont(FontType.HUDBluePreviousAngle, 3, DepthParameter.HUDL5,
                PositionOffset: new Vector2(-185, -20) + origin, textAnchor: TextAnchor.Right);
            spriteNumericFieldList.Add(previousAngle);

            //Menu Buttons
            shot1Button = new ShotButton(ButtonType.BlueShotS1, this.mobile, DepthParameter.HUDForeground, ShotType.S1, ButtonOffset: new Vector2(-371, -4) + origin, IsActivated: true);
            shot2Button = new ShotButton(ButtonType.BlueShotS2, this.mobile, DepthParameter.HUDForeground, ShotType.S2, ButtonOffset: new Vector2(-328, -4) + origin);
            shotSSButton = new ShotButton(ButtonType.BlueShotSS, this.mobile, DepthParameter.HUDForeground, ShotType.SS, ButtonOffset: new Vector2(-285, -4) + origin);

            shot1Button.OtherButtons = shot2Button.OtherButtons = shotSSButton.OtherButtons = new List<ShotButton>() { shot1Button, shot2Button, shotSSButton };
            shot1Button.ChangeButtonState(ButtonAnimationState.Activated);

            menuButtonList.Add(shot2Button);
            menuButtonList.Add(shotSSButton);

            gameOptionsButton = new Button(ButtonType.BlueOptions, DepthParameter.HUDForeground, GameOptionsOnClickAction, new Vector2(-359, 26) + origin);
            menuButtonList.Add(gameOptionsButton);

            passTurnButton = new Button(ButtonType.BlueSkipTurn, DepthParameter.HUDForeground, PassTurnButtonAction, new Vector2(-296, 26) + origin);
            menuButtonList.Add(passTurnButton);

            //Shot Strenght Selector (Button)
            TransparentButton shotStrenghtSelectorButton = new TransparentButton(
                new Vector2(41, 17) + origin, new Rectangle(0, 0, 400, 19));
            shotStrenghtSelectorButton.OnBeingDragged = (button) =>
            {
                shotStrenghtSelector.PositionOffset = new Vector2(
                    MathHelper.Clamp(
                        Cursor.Instance.CurrentFlipbook.Position.X + GameScene.Camera.CameraOffset.X,
                        -160 + origin.X,
                        -160 + origin.X + StrenghtBar.BarSprite.SpriteWidth),
                    shotStrenghtSelector.PositionOffset.Y);
            };
            menuButtonList.Add(shotStrenghtSelectorButton);

            //Health Bars
            healthBarList = new List<HealthBar>();
            nameplateList = new List<Nameplate>();

            mobileList.ForEach((x) =>
            {
                healthBarList.Add(new HealthBar(x));
                nameplateList.Add(new Nameplate(x));
            });

            textFilterButton = new Button(ButtonType.MessageFilter, DepthParameter.HUDForeground, OnTeamMessageFilterIsClicked, new Vector2(-40, -60) + origin);
            allTextFilterSpriteFont = new SpriteText(FontTextType.Consolas10, Language.HUDTextAllText, Color.White, Alignment.Center, DepthParameter.HUDL1, outlineColor: Color.Black);
            teamTextFilterSpriteFont = new SpriteText(FontTextType.Consolas10, Language.HUDTextTeamText, Color.White, Alignment.Center, DepthParameter.HUDL1, outlineColor: Color.Black);

            allTextFilterSpriteFont.PositionOffset = teamTextFilterSpriteFont.PositionOffset = new Vector2(-40, -55) + origin;
            teamTextFilterSpriteFont.SetTransparency(0);

            menuButtonList.Add(textFilterButton);

            //Wind Compass
            WindCompass = new WindCompass(Vector2.Zero + new Vector2(0, (54 - Parameter.ScreenCenter.Y) * 0.90f / GameScene.Camera.Zoom.Y));

            //Weather Display
            WeatherDisplay = new WeatherDisplay(origin + new Vector2(157, -20));

            //Delayboard
            Delayboard = new Delayboard(mobileList, new Vector2(origin.X - spriteList[0].SpriteWidth / 2, origin.Y - spriteList[0].SpriteHeight / 2));

            //Popups
            popupGameOptions = new PopupGameOptions(default);
            popupGameOptions.OnClose += PopupGameOptionsOnCloseAction;
            PopupHandler.Add(popupGameOptions);

            //Time Counter
            turnCounter = new NumericSpriteFont(FontType.HUDTurnTimer, 2, DepthParameter.HUDBackground,
                Vector2.Zero, new Vector2(Parameter.ScreenCenter.X * 0.95f, -Parameter.ScreenCenter.Y * 0.95f / GameScene.Camera.Zoom.Y),
                TextAnchor.Right, 0, true, true);
            spriteNumericFieldList.Add(turnCounter);

            turnCounterBallooon = new Flipbook(Vector2.Zero, new Vector2(17, 15), 34, 31,
                "Interface/InGame/HUD/Blue/TurnCounterBalloon", 
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 20 }, DepthParameter.HUDBackground, 0);

            turnCounter.HideElement();
            turnCounterBallooon.HideElement();

            //Text
            TextBoxes = new TextBox[] {
                //Player Messages & Textbox
                new TextBox(-(Parameter.ScreenResolution / 2 - new Vector2(5, 5)) / GameScene.Camera.Zoom,
                    new Vector2(370, 300), 0, 0f,
                    hasScrollBar: false, scrollBackgroundAlpha: 0,
                    hasTextField: true, textFieldBackground: 0, textFieldOffset: new Vector2(185, 0), textFieldFixedPosition: new Vector2(-205, -70) + origin, maximumTextLength: 50,
                    onSendMessage: OnSendMessage),

                //Server Messages
                new TextBox( (Parameter.ScreenResolution / 2 * new Vector2(1, -1) - new Vector2(370, 0) + new Vector2(-5, 5)) / GameScene.Camera.Zoom,
                    new Vector2(370, 300), 0, 0f,
                    hasTextField: false, scrollBackgroundAlpha: 0,
                    hasScrollBar: false, alignment: Alignment.Right)
            };

            textBoxesTransparencyElapsedTime = new float[2];
            textBoxesTransparencyFadeTime = new float[2];

            //Status Bar
            StatusBarDictionary = new Dictionary<Mobile, AttributeStatusBar>();
            foreach (Mobile m in mobileList)
                StatusBarDictionary.Add(m, new AttributeStatusBar(m));

            //Sound Effects
            goldAcquisitionSE = AssetHandler.Instance.RequestSoundEffect("Audio/SFX/InGame/Gameplay/GoldAcquisition");
        }

        public void PlayGoldAcquisitionSE()
        {
            //Multiple 
            AudioHandler.PlaySoundEffectWhile(goldAcquisitionSE, () => GoldNumericField.FinalValue != (int)GoldNumericField.CurrentValue && !LevelScene.TerminateThreads);
            AudioHandler.PlaySoundEffectWhile(goldAcquisitionSE, () => GoldNumericField.FinalValue != (int)GoldNumericField.CurrentValue && !LevelScene.TerminateThreads, delay: 100);
            AudioHandler.PlaySoundEffectWhile(goldAcquisitionSE, () => GoldNumericField.FinalValue != (int)GoldNumericField.CurrentValue && !LevelScene.TerminateThreads, delay: 200);
        }

        public void UnlockItem()
        {
            if (WeatherDisplay.ActiveWeather?.Weather != WeatherType.Ignorance)
            {
                MatchStatusClosed.SetTransparency(0);
                EquippedButtonBar.Enable();
            }
        }

        public void LockItem()
        {
            MatchStatusClosed.SetTransparency(1);
            EquippedButtonBar.Disable();
        }

        public void EnableSS() => shotSSButton.Enable();

        public void DisableSS()
        {
            if (mobile.SelectedShotType == ShotType.SS)
            {
                mobile.ChangeShot(ShotType.S2);
                shot2Button.IsActivated = true;
            }

            shotSSButton.Disable();
        }

        #region Textbox
        //Creates two distinct transparency animations for each textbox
        public void UpdateTextboxTransparencies(GameTime gameTime)
        {
            for (int i = 0; i < TextBoxes.Length; i++)
            {
                if (TextBoxes[i].IsTextFieldActive) continue;

                if (textBoxesTransparencyElapsedTime[i] < Parameter.InterfaceInGameTextBoxTimeToStartFading)
                {
                    textBoxesTransparencyElapsedTime[i] += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else if (textBoxesTransparencyFadeTime[i] < Parameter.InterfaceInGameTextBoxTimeFadeTime)
                {
                    textBoxesTransparencyFadeTime[i] += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    TextBoxes[i].Transparency = 1 - textBoxesTransparencyFadeTime[i] / Parameter.InterfaceInGameTextBoxTimeFadeTime;
                }
            }
        }

        //Changes the color of the text depending on which team you are
        public void OnTeamMessageFilterIsClicked(object b)
        {
            if (textFilterButton.IsActivated)
            {
                allTextFilterSpriteFont.SetTransparency(0);
                teamTextFilterSpriteFont.SetTransparency(1);

                if (GameInformation.Instance.PlayerInformation.PlayerTeam == PlayerTeam.Red)
                    TextBoxes[0].SetTextFieldColor(Parameter.TextColorTeamRed, Color.Black);
                else
                    TextBoxes[0].SetTextFieldColor(Parameter.TextColorTeamBlue, Color.Black);
            }
            else
            {
                allTextFilterSpriteFont.SetTransparency(1);
                teamTextFilterSpriteFont.SetTransparency(0);

                TextBoxes[0].SetTextFieldColor(Color.White, Color.Black);
            }
        }

        public void OnSendMessage(PlayerMessage message)
        {
            if (textFilterButton.IsActivated) message.PlayerTeam = GameInformation.Instance.PlayerInformation.PlayerTeam;
            TextBoxes[0].DeactivateTextField();
            ServerInformationHandler.SendGameListMessage(message);
        }

        //Updates both textboxes, but the 2nd textbox has no textfield.
        public void UpdateTextBoxes(GameTime gameTime)
        {
            if (InputHandler.IsBeingPressed(Keys.Enter))
            {
                if (TextBoxes[0].IsTextFieldActive)
                {
                    TextBoxes[0].DeactivateTextField();
                    IsChatOpen = mobile.IsActionsLocked = false;
                }
                else
                {
                    TextBoxes[0].ActivateTextField();
                    textBoxesTransparencyElapsedTime[0] = 0;
                    textBoxesTransparencyFadeTime[0] = 0;
                    TextBoxes[0].Transparency = 1;
                    IsChatOpen = mobile.IsActionsLocked = true;
                }
            }

            TextBoxes[0].Update(gameTime);
            TextBoxes[1].Update(gameTime);
        }

        public void OnReceiveMessageAsyncCallback(object message, int textBoxIndex)
        {
            TextBoxes[textBoxIndex].AsyncAppendText(message);

            textBoxesTransparencyElapsedTime[textBoxIndex] = 0;
            textBoxesTransparencyFadeTime[textBoxIndex] = 0;
            TextBoxes[textBoxIndex].Transparency = 1;

            // Check if it is possible to append a new textballoon on the scene
            if (message is PlayerMessage)
            {
                PlayerMessage pMessage = (PlayerMessage)message;
                Mobile pMob = null;

                // Find the mobile owned by the player who sent the message
                lock (LevelScene.MobileList)
                {
                    foreach (Mobile mob in LevelScene.MobileList)
                    {
                        if (mob.Owner.Nickname == pMessage.Player.Nickname)
                        {
                            pMob = mob;
                            break;
                        }
                    }
                }

                // Append the balloon on the scene
                if (pMob != null)
                    TextBalloonHandler.AsyncAddTextBalloon(pMob, pMessage, 60, DepthParameter.InGameTextBalloonBase);
            }
        }
        #endregion

        public void UseItemAction(Button sender, ItemButtonPreset preset)
        {
            if (mobile.IsAbleToUseItem)
            {
                if (mobile.Movement.IsFalling)
                {
                    sender.Disable();
                    sender.Enable();
                    return;
                }

                if (!sender.IsEnabled)
                {
                    sender.ChangeButtonState(ButtonAnimationState.Activated, true);
                    return;
                }

                removeItemFromBarAction = () =>
                {
                    EquippedButtonBar.RemoveButton(sender, preset);
                };

                EquippedButtonBar.Disable();
                sender.ChangeButtonState(ButtonAnimationState.Activated, true);
                sender.IsEnabled = false;
                mobile.RequestUseItem(preset.ItemPreset.ItemType);

                //Changing SS button state if activated
                DisableSS();
            }
            else
            {
                if (!sender.IsEnabled)
                {
                    sender.ChangeButtonState(ButtonAnimationState.Activated, true);
                    return;
                }

                foreach (Button b in EquippedButtonBar.ButtonList)
                {
                    if (b != sender && b.IsActivated)
                    {
                        b.Disable();
                        b.Enable();
                    }
                }

                if (sender.IsActivated)
                    useItemOnTurnBeginAction = () => { UseItemAction(sender, preset); };
                else
                    useItemOnTurnBeginAction = default;
            }
        }

        public void MobileDeath(Mobile mobile)
        {
            Delayboard.RemoveEntry(mobile);
            healthBarList.Find((x) => x.Mobile == mobile).HideElement();
            nameplateList.Find((x) => x.Mobile == mobile).HideElement();
        }

        public void PassTurnButtonAction(object sender)
        {
            mobile.RequestLoseTurn();
        }

        public void LoseTurn()
        {
            turnCounterShouldUpdate = false;

            turnCounterBallooon.HideElement();

            if (currentTurnOwner.IsPlayable)
            {
                passTurnButton.Disable();
            }
        }

        public void GrantTurn(Mobile mobile)
        {
            currentTurnOwner = mobile;

            if (mobile.IsPlayable)
            {
                StrenghtBar.Reset();
                MovementBar.Reset();
                passTurnButton.Enable();

                UnlockItem();

                removeItemFromBarAction?.Invoke();
                removeItemFromBarAction = default;
                useItemOnTurnBeginAction?.Invoke();
                useItemOnTurnBeginAction = default;
            }

            ResetTimer(mobile);
        }

        #region Timer
        private void ResetTimer(Mobile mobile)
        {
            turnCounterCurrentTime = NetworkObjectParameters.TimePerPlayerTurn + 0.9f;
            turnCounterBallooon.HideElement();
            turnCounterShouldUpdate = true;

            if (mobile.IsPlayable)
            {
                turnCounter.ShowElement();
                turnCounter.UpdateValue((int)turnCounterCurrentTime);
            }
            else
            {
                turnCounter.HideElement();
            }
        }

        public void UpdateTimer(GameTime gameTime)
        {
            int tmp = (int)turnCounterCurrentTime;

            if (turnCounterShouldUpdate)
                turnCounterCurrentTime = Math.Max(turnCounterCurrentTime - gameTime.ElapsedGameTime.TotalSeconds, 0);

            //SFX
            if (tmp != (int)turnCounterCurrentTime)
            {
                if (currentTurnOwner.IsPlayable || tmp < 10)
                    AudioHandler.PlaySoundEffect("Audio/SFX/InGame/Gameplay/ClockSecond", pitch: MathHelper.Clamp(((float)(tmp - 1) / NetworkObjectParameters.TimePerPlayerTurn - 0.5f) * 2, -1, 1));

                if (tmp == 0) turnCounterBallooon.HideElement();
            }

            //Playable Mobile Timer
            if (currentTurnOwner != null)
            {
                if (currentTurnOwner.IsPlayable)
                {
                    if (turnCounterCurrentTime <= 0 && currentTurnOwner.IsAbleToShoot)
                        mobile.RequestLoseTurn();

                    turnCounter.UpdateValue((int)turnCounterCurrentTime);
                }
                else
                {
                    //Turn Counter Balloon
                    if (turnCounterCurrentTime <= Parameter.InterfaceInGameTurnTimeBalloonMaximumNumber)
                    {
                        turnCounterBallooon.Position = currentTurnOwner.Position - new Vector2(0, 120) + Vector2.UnitY * (float)Math.Sin(MathHelper.PiOver2 + (Parameter.InterfaceInGameTurnTimeBalloonMaximumNumber - turnCounterCurrentTime)) * 5f;
                        turnCounterBallooon.ShowElement();
                        turnCounterBallooon.SetCurrentFrame(Parameter.InterfaceInGameTurnTimeBalloonMaximumNumber - tmp);
                    }
                }
            }
        }
        #endregion

        public void GameOptionsOnClickAction(object sender)
        {
            popupGameOptions.ShouldRender = true;
            gameOptionsButton.Disable();
        }

        public void PopupGameOptionsOnCloseAction(object sender)
        {
            gameOptionsButton.Enable();
        }

        public void Update(GameTime gameTime)
        {
            //Sprites
            spriteList.ForEach((x) => x.UpdateAttatchmentPosition());

            //Bars
            MovementBar.UpdateAttatchmentPosition();
            StrenghtBar.UpdateAttatchmentPosition();

            //Floating Texts
            FloatingTextHandler.Update(gameTime);

            //Menu buttons
            menuButtonList.ForEach((x) => x.Update());
            shot1Button.Update();

            //Angle
            currentAngle.UpdateValue(mobile.Crosshair.HUDSelectedAngle);

            //Fonts and texts
            spriteNumericFieldList.ForEach((x) => x.Update(gameTime));

            //Health bars
            healthBarList.ForEach((x) => x.Update());
            nameplateList.ForEach((x) => x.Update());

            //TextButtons
            allTextFilterSpriteFont.UpdateAttatchedPosition();
            teamTextFilterSpriteFont.UpdateAttatchedPosition();

            //Delayboard
            Delayboard.Update();

            //Wind Compass
            WindCompass.Update(gameTime);

            //Incoming Weather
            WeatherDisplay.Update(gameTime);

            //Text
            UpdateTextboxTransparencies(gameTime);
            UpdateTextBoxes(gameTime);

            //Items
            EquippedButtonBar.Update();
            EquippedButtonBar.UpdateAttatchMentPosition();

            //Timer
            UpdateTimer(gameTime);

            //StatusBar
            StatusBarDictionary.ForEachValues((x) => x.Update());
        }

        public void UpdatePreviousShotMarker()
        {
            strenghtBarPreviousShotMarker.PositionOffset = new Vector2(-160 + StrenghtBar.BarSprite.SourceRectangle.Width, 26) + origin;
            previousAngle.UpdateValue((int)currentAngle.CurrentValue);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Sprites
            spriteList.ForEach((x) => x.Draw(gameTime, spriteBatch));

            //Bars
            StrenghtBar.Draw(gameTime, spriteBatch);
            MovementBar.Draw(gameTime, spriteBatch);

            //Floating Texts
            FloatingTextHandler.Draw(gameTime, spriteBatch);

            //SpriteFont
            spriteNumericFieldList.ForEach((x) => x.Draw(gameTime, spriteBatch));

            //Static Buttons
            shot1Button.Draw(gameTime, spriteBatch);
            menuButtonList.ForEach((x) => x.Draw(gameTime, spriteBatch));

            //Text List
            allTextFilterSpriteFont.Draw(spriteBatch);
            teamTextFilterSpriteFont.Draw(spriteBatch);

            //Health bars
            healthBarList.ForEach((x) => x.Draw(gameTime, spriteBatch));
            nameplateList.ForEach((x) => x.Draw(spriteBatch));

            //Delayboard
            Delayboard.Draw(gameTime, spriteBatch);

            //Wind Compass
            WindCompass.Draw(spriteBatch);

            //IncomingWeather
            WeatherDisplay.Draw(gameTime, spriteBatch);

            //Text
            TextBoxes[0].Draw(spriteBatch);
            TextBoxes[1].Draw(spriteBatch);

            //Items
            EquippedButtonBar.Draw(gameTime, spriteBatch);

            //Time Balloon
            turnCounterBallooon.Draw(gameTime, spriteBatch);

            //Draw
            StatusBarDictionary.ForEachValues((x) => x.Draw(spriteBatch));
        }

        public void Dispose()
        {
            TextBoxes[0].Dispose();
            TextBoxes[1].Dispose();
        }
    }
}