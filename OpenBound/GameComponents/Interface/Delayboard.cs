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
using OpenBound.GameComponents.Interface.Builder;
using OpenBound.GameComponents.Interface.Text;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Pawn;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.Entity.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenBound_Network_Object_Library.Models;

namespace OpenBound.GameComponents.Interface
{
    public class PlayerScoreboardEntry
    {
        public Mobile Mobile;
        public Sprite RankingSprite;
        public NumericSpriteFont TurnCounterSpriteFont;
        public NumericSpriteFont DelayCounterSpriteFont;
        public SpriteText NicknameSpriteText;
        public int DisplayingDelay;

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            RankingSprite.Draw(gameTime, spriteBatch);
            TurnCounterSpriteFont.Draw(gameTime, spriteBatch);
            DelayCounterSpriteFont.Draw(gameTime, spriteBatch);
            NicknameSpriteText.Draw(spriteBatch);
        }
    }

    public class Delayboard
    {
        private Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                UpdatePosition();
            }
        }

        public PlayerScoreboardEntry OwningPlayerScoreboardEntry;

        private List<PlayerScoreboardEntry> playerScoreboardEntryList;
        private List<PlayerScoreboardEntry> unusedPlayerScoreboardEntryList;
        private Sprite playerBGBar;
        private int biggestNickWidth;
        private float xOffset, yOffset;
        private Vector2 rankingSpritePosition;

        /*
         * Table Scheme:
         *                             V
         *   +---+----+-----+------+-------+-----+-----+-----+
         *   | # | LV | Mob | Nick | Delay | DMG | K/D | ACC |
         *   | 1 |ICON| ICO | Nick | -1000 | 150 | 5/0 | 100 |
         *   | 2 |ICON| ICO | Nick |  -100 | 500 | 3/2 |  98 |
         * > | 3 |ICON| ICO | Nick |     0 | 900 | 1/0 | 100 |
         *   | 4 |ICON| ICO | Nick |    10 |  30 | 1/2 |  30 |
         *   | 5 |ICON| ICO | Nick |   100 |  50 | 1/1 |  12 |
         *   | 6 |ICON| ICO | Nick |  1000 | 300 | 2/3 |  30 |
         *   +---+----+-----+------+-------+-----+-----------+
         * 
         * #     = turnCounterSpriteFontList
         * LV    = rankingSpriteFontList
         * Mob   = ??
         * Nick  = compositeSpriteTextList[0].SentenceList
         * Delay = delayCounterSpriteFontList
         * DMG   = compositeSpriteTextList[1].SentenceList
         * K/D   = compositeSpriteTextList[2].SentenceList
         * ACC   = compositeSpriteTextList[3].SentenceList
         * 
         * DMG/K/D/ACC were scraped out (temporarily) because the original game do not have a proper numfont, breaking the board esthetic.
         * I am probably going to move those statistics to other screen components (Scoreboard)
         */

        public Delayboard(List<Mobile> mobileList, Vector2 position)
        {
            playerScoreboardEntryList = new List<PlayerScoreboardEntry>();
            unusedPlayerScoreboardEntryList = new List<PlayerScoreboardEntry>();

            playerBGBar = new Sprite("Interface/InGame/HUD/Blue/ActionBar/SelectedPlayerTurnBG", layerDepth: DepthParameter.HUDBackground)
            { Pivot = new Vector2(0, 0) };

            yOffset = 16;
            xOffset = 4;

            for (int i = 0; i < mobileList.Count; i++)
            {
                Mobile mob = mobileList[i];

                FontType delayCounterFontType = FontType.HUDBlueBlueTeamTurnCounter;
                Color entryColor = Parameter.TextColorTeamBlue;

                if (mob.Owner.PlayerTeam == PlayerTeam.Red)
                {
                    entryColor = Parameter.TextColorTeamRed;
                    delayCounterFontType = FontType.HUDBlueRedTeamTurnCounter;
                }

                //Turn Counter
                NumericSpriteFont turnCounterSF = new NumericSpriteFont(delayCounterFontType, 1, DepthParameter.HUDForeground, textAnchor: TextAnchor.Left, StartingValue: mobileList.IndexOf(mob));

                //Ranking
                Sprite playerRankingIcon = IconBuilder.Instance.BuildPlayerRank(mob.Owner.PlayerRank, DepthParameter.HUDForeground);
                playerRankingIcon.Pivot = new Vector2(0, 0);

                //Nicknames
                SpriteText nicknameST = new SpriteText(FontTextType.NotoSans10Family, mob.Owner.Nickname, entryColor, Alignment.Left, DepthParameter.HUDL2, default, Color.Black);

                //Delay
                NumericSpriteFont delayCounterSF = new NumericSpriteFont(FontType.HUDBlueDelay, 5, DepthParameter.HUDForeground, textAnchor: TextAnchor.Right, StartingValue: mob.SyncMobile.Delay);

                //Creating Scoreboard Entry
                playerScoreboardEntryList.Add(new PlayerScoreboardEntry()
                {
                    Mobile = mob,
                    DelayCounterSpriteFont = delayCounterSF,
                    RankingSprite = playerRankingIcon,
                    TurnCounterSpriteFont = turnCounterSF,
                    NicknameSpriteText = nicknameST
                });

                //Owning Player Index
                if (mob.Owner.ID == GameInformation.Instance.PlayerInformation.ID) OwningPlayerScoreboardEntry = playerScoreboardEntryList.Last();
            }

            biggestNickWidth = (int)playerScoreboardEntryList.Max((x) => x.NicknameSpriteText.MeasureSize.X);
            rankingSpritePosition = new Vector2((int)(playerScoreboardEntryList[0].TurnCounterSpriteFont.MeasureSize.X + xOffset), 0);

            Position = position;
        }

        public void SetPlayerOrder(List<SyncMobile> syncMobileList)
        {
            List<PlayerScoreboardEntry> tmpPSE = new List<PlayerScoreboardEntry>();

            int entryCount = playerScoreboardEntryList.Count;

            for (int i = 0; i < entryCount; i++)
            {
                PlayerScoreboardEntry entry = playerScoreboardEntryList.Find((x) => x.Mobile.Owner.ID == syncMobileList[i].Owner.ID);

                playerScoreboardEntryList.Remove(entry);

                tmpPSE.Add(entry);
            }

            playerScoreboardEntryList = tmpPSE;
        }

        public void ComputeDelay()
        {
            OwningPlayerScoreboardEntry.DisplayingDelay =
                OwningPlayerScoreboardEntry.Mobile.MobileMetadata
                .GetDelay(OwningPlayerScoreboardEntry.Mobile.Owner, OwningPlayerScoreboardEntry.Mobile.SelectedShotType);
        }

        public PlayerScoreboardEntry GetEntryByPlayerID(int ID)
        {
            return playerScoreboardEntryList.Find((x) => x.Mobile.Owner.ID == ID);
        }

        public void SetPlayerBGBarRectangle()
        {
            int playerNumber = playerScoreboardEntryList.Count();
            int p = playerScoreboardEntryList.IndexOf(OwningPlayerScoreboardEntry);

            //1 = green
            //4 = orange
            //7 = red
            //10 = gray

            int xPos = 1;

            if (playerNumber <= 2)
            {
                if (p == 1) xPos = 4;
            }
            else if (playerNumber <= 4)
            {
                if (p == 1) xPos = 4;
                else if (p == 2) xPos = 7;
                else if (p == 3) xPos = 10;
            }
            else if (playerNumber <= 6)
            {
                if (p <= 1) xPos = 1;
                else if (p <= 3) xPos = 4;
                else if (p <= 5) xPos = 7;
                else xPos = 10;
            }
            else
            {
                if (p <= 1) xPos = 1;
                else if (p <= 3) xPos = 4;
                else if (p <= 5) xPos = 7;
                else xPos = 10;
            }

            playerBGBar.SourceRectangle = new Rectangle(xPos, 0, 1, playerBGBar.SpriteHeight);
        }

        private void UpdatePosition()
        {
            if (!playerScoreboardEntryList.Any()) return;

            //Sets the anchor of the UI element from Top-Left to Bottom-Left
            Vector2 tmpPos = new Vector2((int)Position.X, (int)(Position.Y - yOffset * playerScoreboardEntryList.Count));
            int biggestDelayWidth = (int)playerScoreboardEntryList.Max((x) => x.DelayCounterSpriteFont.MeasureCompleteSize.X);
            Vector2 delayCounterPosition = new Vector2(biggestNickWidth + biggestDelayWidth + xOffset, 0);

            for (int i = 0; i < playerScoreboardEntryList.Count; i++)
            {
                PlayerScoreboardEntry entry = playerScoreboardEntryList[i];

                NumericSpriteFont tNSF = entry.TurnCounterSpriteFont;
                Sprite rS = entry.RankingSprite;
                SpriteText nST = entry.NicknameSpriteText;
                NumericSpriteFont dSF = entry.DelayCounterSpriteFont;

                tNSF.PositionOffset = tmpPos;
                rS.PositionOffset = tmpPos + rankingSpritePosition;
                nST.PositionOffset = rS.PositionOffset +
                    new Vector2(rS.SourceRectangle.Width + xOffset, 0) +
                    //(Vector2.UnitY * nST.MeasureSize / 2f) +
                    nST.FontFamilyMetadata.ElementHeightOffset;
                dSF.PositionOffset = nST.PositionOffset + new Vector2(0, nST.MeasureSize.Y - 6) / 2f + delayCounterPosition;

                tmpPos += new Vector2(0, (int)yOffset);
            }

            playerBGBar.Scale = new Vector2(Math.Abs((OwningPlayerScoreboardEntry.TurnCounterSpriteFont.PositionOffset - OwningPlayerScoreboardEntry.DelayCounterSpriteFont.PositionOffset).X) + xOffset, 1);
            playerBGBar.PositionOffset = OwningPlayerScoreboardEntry.TurnCounterSpriteFont.PositionOffset;

            UpdateAttatchmentPositions();
        }

        public void UpdateDelay()
        {
            foreach (PlayerScoreboardEntry pse in playerScoreboardEntryList)
            {
                if (LevelScene.CurrentTurnOwner?.Owner.ID == OwningPlayerScoreboardEntry.Mobile.Owner.ID)
                {
                    if (pse.Mobile.IsPlayable)
                        pse.DelayCounterSpriteFont.UpdateValue(pse.Mobile.MobileMetadata.GetDelay(pse.Mobile.Owner, pse.Mobile.SelectedShotType));
                    else
                        pse.DelayCounterSpriteFont.UpdateValue(pse.Mobile.SyncMobile.Delay - OwningPlayerScoreboardEntry.Mobile.SyncMobile.Delay - OwningPlayerScoreboardEntry.Mobile.MobileMetadata.GetDelay(OwningPlayerScoreboardEntry.Mobile.Owner, OwningPlayerScoreboardEntry.Mobile.SelectedShotType));
                }
                else
                {
                    if (pse.Mobile.IsPlayable)
                        pse.DelayCounterSpriteFont.UpdateValue(pse.DisplayingDelay);
                    else
                        pse.DelayCounterSpriteFont.UpdateValue(pse.Mobile.SyncMobile.Delay - OwningPlayerScoreboardEntry.Mobile.SyncMobile.Delay);

                }
            }

            //Re-sort the list
            playerScoreboardEntryList = playerScoreboardEntryList.OrderBy((x) =>
            {
                if (LevelScene.CurrentTurnOwner?.Owner.ID == OwningPlayerScoreboardEntry.Mobile.Owner.ID && x.Mobile.IsPlayable)
                    return x.Mobile.SyncMobile.Delay + x.Mobile.MobileMetadata.GetDelay(x.Mobile.Owner, x.Mobile.SelectedShotType);
                else
                    return x.Mobile.SyncMobile.Delay;
            }).ToList();

            for (int i = 0; i < playerScoreboardEntryList.Count; i++)
            {
                PlayerScoreboardEntry pse = playerScoreboardEntryList[i];
                pse.TurnCounterSpriteFont.UpdateValue(i);
            }

            UpdatePosition();
            SetPlayerBGBarRectangle();
        }

        public void RemoveEntry(Mobile mobile)
        {
            PlayerScoreboardEntry psE = playerScoreboardEntryList.Find((x) => x.Mobile == mobile);

            if (psE == null) return;

            if (mobile.IsPlayable)
                playerBGBar.HideElement();

            playerScoreboardEntryList.Remove(psE);
            unusedPlayerScoreboardEntryList.Add(psE);
        }

        public void AddEntry(Mobile mobile)
        {
            PlayerScoreboardEntry psE = unusedPlayerScoreboardEntryList.Find((x) => x.Mobile == mobile);

            if (psE == null) return;

            if (mobile.IsPlayable)
                playerBGBar.ShowElement();

            playerScoreboardEntryList.Add(psE);
            unusedPlayerScoreboardEntryList.Remove(psE);
        }

        public void Update()
        {
            playerScoreboardEntryList.ForEach((x) =>
            {
                x.DelayCounterSpriteFont.Update(null);
                x.TurnCounterSpriteFont.Update(null);
            });

            UpdateDelay();
            UpdateAttatchmentPositions();
        }

        public void UpdateAttatchmentPositions()
        {

            playerScoreboardEntryList.ForEach((x) =>
            {
                x.RankingSprite.UpdateAttatchmentPosition();
                x.NicknameSpriteText.UpdateAttatchedPosition();
            });

            playerBGBar.UpdateAttatchmentPosition();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            playerScoreboardEntryList.ForEach((x) => x.Draw(gameTime, spriteBatch));
            playerBGBar.Draw(gameTime, spriteBatch);
        }
    }
}
