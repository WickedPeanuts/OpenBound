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
using OpenBound_Network_Object_Library.Entity;
using System.Collections.Generic;
using OpenBound_Network_Object_Library.Models;
using System.Linq;

namespace OpenBound.GameComponents.Interface.Interactive.GameRoom
{
    public class PlayerButtonList
    {
        //private List<Player> teamA, teamB;
        private List<PlayerButton> teamAButtonList, teamBButtonList;
        static readonly Vector2 redTeamStartingPosition = -new Vector2(285, 235);
        static readonly Vector2 blueTeamStartingPosition = -new Vector2(-285, 235);
        public PlayerButtonList()
        {
            teamAButtonList = new List<PlayerButton>();
            teamBButtonList = new List<PlayerButton>();
        }

        public void Update()
        {
            teamAButtonList.ForEach((x) => x.Update());
            teamBButtonList.ForEach((x) => x.Update());
        }

        public List<PlayerButton> PlayerTeamButtonList =>
            teamAButtonList.Union(teamBButtonList).ToList();

        public void UpdatePlayerButtonList()
        {
            RoomMetadata room = GameInformation.Instance.RoomMetadata;

            RemovePlayerButtons(room.TeamASafe, room.TeamBSafe);
            CreatePlayerButtons(room.TeamASafe, room.TeamBSafe);
            UpdatePlayerButtonPositions();
        }

        public void RemovePlayerButtons(List<Player> teamA, List<Player> teamB)
        {
            List<PlayerButton> toBeRemovedList = new List<PlayerButton>();
            foreach (PlayerButton pb in teamAButtonList)
            {
                if (!teamA.Exists((x) => x == pb.Player))
                    toBeRemovedList.Add(pb);
            }

            toBeRemovedList.ForEach((x) => teamAButtonList.Remove(x));
            toBeRemovedList.Clear();

            foreach (PlayerButton pb in teamBButtonList)
            {
                if (!teamB.Exists((x) => x == pb.Player))
                    toBeRemovedList.Add(pb);
            }

            toBeRemovedList.ForEach((x) => teamBButtonList.Remove(x));
        }

        public void CreatePlayerButtons(List<Player> teamA, List<Player> teamB)
        {
            foreach (Player gpA in teamA)
            {
                if (!teamAButtonList.Exists((x) => x.Player == gpA))
                    teamAButtonList.Add(new PlayerButton(gpA, default, default));
            }

            foreach (Player gpB in teamB)
            {
                if (!teamBButtonList.Exists((x) => x.Player == gpB))
                    teamBButtonList.Add(new PlayerButton(gpB, default, default));
            }
        }

        public void UpdatePlayerButtonPositions()
        {
            for (int i = 0; i < teamAButtonList.Count; i++)
            {
                PlayerButton pbA = teamAButtonList[i];
                pbA.ChangePosition(redTeamStartingPosition + new Vector2((i % 2 == 0) ? 0 : 43, i * 102));
            }

            for (int i = 0; i < teamBButtonList.Count; i++)
            {
                PlayerButton pbB = teamBButtonList[i];
                pbB.ChangePosition(blueTeamStartingPosition + new Vector2((i % 2 == 0) ? 0 : -43, i * 102));
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            teamAButtonList.ForEach((x) => x.Draw(gameTime, spriteBatch));
            teamBButtonList.ForEach((x) => x.Draw(gameTime, spriteBatch));
        }
    }
}
