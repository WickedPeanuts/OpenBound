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

using OpenBound.GameComponents.Pawn;
using OpenBound_Network_Object_Library.Entity;
using System.Collections.Generic;
using OpenBound_Network_Object_Library.Models;

namespace OpenBound.Common
{
    public class GameInformation
    {
        private static GameInformation instance { get; set; }
        public static GameInformation Instance
        {
            get
            {
                if (instance == null) instance = new GameInformation();
                return instance;
            }
        }

        public GameInformation()
        {
            GameState = GameState.Menu;
            ServerList = new List<GameServerInformation>();
        }

        public Player PlayerInformation
        {
            get;
            set;
        }

        private RoomMetadata roomMetadata;
        public RoomMetadata RoomMetadata {
            get => roomMetadata;
            set
            {
                roomMetadata = value;
                roomMetadata.Map = Map.GetMap(value.Map.ID);
            }
        }
        public GameState GameState { get; set; }

        public List<GameServerInformation> ServerList;
        public GameServerInformation ConnectedServerInformation { get; set; }

        /*
        public void ChangeGameState(GameState newGameState)
        {
            Cursor.Instance.SetMouseCursorSceneState();
        }*/

        public bool IsOwnedByPlayer(Mobile mobile)
        {
            return mobile.Owner.ID == PlayerInformation.ID;
        }
    }
}
