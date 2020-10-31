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
using OpenBound.GameComponents.Interface.Text;
using OpenBound.GameComponents.Level.Scene;
using OpenBound_Network_Object_Library.Entity;
using System.Collections.Generic;
using System.Linq;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.Entity.Text;

namespace OpenBound.GameComponents.Interface.Popup
{
    public class PopupGameResults : PopupMenu
    {
        //Blue Team Background
        Sprite background2;

        //Nameplate
        List<Nameplate> nameplateList;

        public PopupGameResults() : base(false)
        {
            RoomMetadata room = GameInformation.Instance.RoomMetadata;

            //Background
            if (room.VictoriousTeam == PlayerTeam.Red)
            {
                Background = new Sprite("Interface/Popup/Blue/Results/Background1", layerDepth: DepthParameter.InterfacePopupBackground);
                background2 = new Sprite("Interface/Popup/Blue/Results/Background2", layerDepth: DepthParameter.InterfacePopupBackground);
            }
            else
            {
                Background = new Sprite("Interface/Popup/Blue/Results/Background2", layerDepth: DepthParameter.InterfacePopupBackground);
                background2 = new Sprite("Interface/Popup/Blue/Results/Background1", layerDepth: DepthParameter.InterfacePopupBackground);
            }

            background2.PositionOffset = Background.PositionOffset;
            Background.PositionOffset -= new Vector2(180, 0);
            background2.PositionOffset += new Vector2(180, 0);

            //Player - Team Red
            nameplateList = new List<Nameplate>();
            spriteList = new List<Sprite>();

            AppendPlayersToTable(room.TeamASafe, Background.PositionOffset, true);
            AppendPlayersToTable(room.TeamBSafe, background2.PositionOffset, false);

            buttonList.Add(new Button(ButtonType.Accept, DepthParameter.InterfacePopupButtons, OnAcceptAction, PositionOffset + new Vector2(325, 132)));

            UpdateAttatchmentPosition();

            shouldRender = true;
        }

        private void AppendPlayersToTable(List<Player> team, Vector2 position, bool isTeamRed)
        {
            for (int i = 0; i < team.Count(); i++)
            {
                Vector2 center = position + new Vector2(0, -70) + new Vector2(0, 54) * i;

                Player p = team[i];

                //Nameplate
                Nameplate nmp = new Nameplate(p, Alignment.Left, center - new Vector2(135, 10), false, DepthParameter.InterfacePopupText);
                nameplateList.Add(nmp);

                //Team Frame
                //If you are the player
                if (GameInformation.Instance.PlayerInformation.ID == p.ID)
                {

                    Sprite teamMarker = new Sprite("Interface/Popup/Blue/Results/TeamMarker", center - new Vector2(140, 0), DepthParameter.InterfacePopupButtons, new Rectangle(46 * (isTeamRed ? 1 : 3), 0, 46, 47))
                    { Pivot = new Vector2(46 / 2, 47 / 2), };
                    spriteList.Add(teamMarker);

                    spriteList.Add(new Sprite("Interface/Popup/Blue/Results/PlayerFrame", center, DepthParameter.InterfacePopupForeground));
                }
                else
                {
                    Sprite teamMarker = new Sprite("Interface/Popup/Blue/Results/TeamMarker", center - new Vector2(140, 0), DepthParameter.InterfacePopupButtons, new Rectangle(46 * (isTeamRed ? 0 : 2), 0, 46, 47))
                    { Pivot = new Vector2(46 / 2, 47 / 2), };
                    spriteList.Add(teamMarker);
                }
            }
        }

        private void OnAcceptAction(object sender)
        {
            SceneHandler.Instance.RequestSceneChange(SceneType.GameRoom, TransitionEffectType.RotatingRectangles);
        }

        protected override void UpdateAttatchmentPosition()
        {
            base.UpdateAttatchmentPosition();

            background2.UpdateAttatchmentPosition();
            nameplateList.ForEach((x) => x.UpdateAttatchmentPosition());
            spriteList.ForEach((x) => x.UpdateAttatchmentPosition());
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateAttatchmentPosition();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            background2.Draw(gameTime, spriteBatch);
            nameplateList.ForEach((x) => x.Draw(spriteBatch));
            spriteList.ForEach((x) => x.Draw(gameTime, spriteBatch));
        }
    }
}
