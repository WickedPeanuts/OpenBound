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
using OpenBound.GameComponents.Animation;
using OpenBound_Network_Object_Library.Entity;
using System.Collections.Generic;
using OpenBound_Network_Object_Library.Models;

namespace OpenBound.GameComponents.Interface.Builder
{
    public enum ServerRankLimitationIcon
    {
        ChickL = 0,
        ChichH = 1,

        WoodHammer1L = 2, WoodHammer2L = 4, StoneHammer1L = 6, StoneHammer2L = 8,
        WoodHammer1H = 3, WoodHammer2H = 5, StoneHammer1H = 7, StoneHammer2H = 9,

        Axe1L = 10, Axe2L = 12, SilverAxe1L = 14, SilverAxe2L = 16, GoldenAxe1L = 18, GoldenAxe2L = 20,
        Axe1H = 11, Axe2H = 13, SilverAxe1H = 15, SilverAxe2H = 17, GoldenAxe1H = 19, GoldenAxe2H = 21,

        DAxe1L = 22, DAxe2L = 24, DSilverAxe1L = 26, DSilverAxe2L = 28, DGoldenAxe1L = 30, DGoldenAxe2L = 32,
        DAxe1H = 23, DAxe2H = 25, DSilverAxe1H = 27, DSilverAxe2H = 29, DGoldenAxe1H = 31, DGoldenAxe2H = 33,

        Staff1L = 34, Staff2L = 36, Staff3L = 38, Staff4L = 40,
        Staff1H = 35, Staff2H = 37, Staff3H = 39, Staff4H = 41,

        Dragon1L = 42, Dragon2L = 44, Dragon3L = 46,
        Dragon1H = 43, Dragon2H = 45, Dragon3H = 47,

        Champion1L = 48,
        Champion1H = 49,

        GML = 50,
        GMH = 51,

        AvatarOff = 52,
        AvatarOn = 53,
    }

    public class IconBuilder
    {
        public static Dictionary<ServerRankLimitationIcon, Rectangle> ServerRankLimitationIconDictionary;
        public static Dictionary<PlayerRank, Rectangle> RankIconDictionary;

        private static IconBuilder instance;
        public static IconBuilder Instance
        {
            get
            {
                if (instance == null) instance = new IconBuilder();
                return instance;
            }
        }

        private IconBuilder()
        {
            ServerRankLimitationIconDictionary = new Dictionary<ServerRankLimitationIcon, Rectangle>();

            for (int i = 0; i <= 53; i++)
            {
                int x = (i % 20) * 29;
                int y = (i / 20) * 19;
                ServerRankLimitationIconDictionary.Add((ServerRankLimitationIcon)i, new Rectangle(x, y, 29, 19));
            }

            RankIconDictionary = new Dictionary<PlayerRank, Rectangle>();
            for (int i = 0; i <= (int)PlayerRank.GM; i++)
            {
                int x = (i % 20) * 27;
                int y = (i / 20) * 17;
                RankIconDictionary.Add((PlayerRank)i, new Rectangle(x, y, 27, 17));
            }
        }

        public Sprite BuildServerIcon(ServerRankLimitationIcon ServerRankLimitation, float LayerDepth)
        {
            Sprite spr = new Sprite("Interface/InGame/Scene/ServerList/ServerLevelIndicationRange",
                layerDepth: LayerDepth,
                sourceRectangle: ServerRankLimitationIconDictionary[ServerRankLimitation]);
            spr.Pivot = new Vector2(29 / 2, 19 / 2);

            return spr;
        }

        public Sprite BuildPlayerRank(PlayerRank PlayerRank, float LayerDepth)
        {
            Sprite spr = new Sprite("Interface/Rank/Horizontal",
                layerDepth: LayerDepth,
                sourceRectangle: RankIconDictionary[PlayerRank]);
            spr.Pivot = new Vector2(27 / 2, 17 / 2);

            return spr;
        }
    }
}
