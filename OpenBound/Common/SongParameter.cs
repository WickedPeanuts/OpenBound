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

using System.Collections.Generic;

namespace OpenBound.Common
{
    class SongParameter
    {
        public static readonly List<string> GameRoom = new List<string>()
        {
            "Audio/Music/GameRoom/GameRoomS1",
            "Audio/Music/GameRoom/GameRoomS2-1",
            "Audio/Music/GameRoom/GameRoomS2-2",
        };

        public static readonly List<string> GameSceneEvent = new List<string>()
        {
            "Audio/Music/LevelScene/Event/BirthdayS2",
            "Audio/Music/LevelScene/Event/ChristmasS1",
            "Audio/Music/LevelScene/Event/ChristmasS2",
            "Audio/Music/LevelScene/Event/UnknownS1-1",
            "Audio/Music/LevelScene/Event/UnknownS1-2",
            "Audio/Music/LevelScene/Event/UnknownS1-3",
        };

        public static readonly List<string> GameSceneSuddenDeath = new List<string>()
        {
            "Audio/Music/LevelScene/SuddenDeath/SuddenDeathS1",
        };

        public static readonly List<string> LevelScene = new List<string>()
        {
            "Audio/Music/LevelScene/ThemeBeta-1",
            "Audio/Music/LevelScene/ThemeBeta-2",
            "Audio/Music/LevelScene/ThemeS1-1",
            "Audio/Music/LevelScene/ThemeS1-2",
            "Audio/Music/LevelScene/ThemeS1-3",
            "Audio/Music/LevelScene/ThemeS1-4",
            "Audio/Music/LevelScene/ThemeS1-5",
            "Audio/Music/LevelScene/ThemeS1-6",
            "Audio/Music/LevelScene/ThemeS2-1",
            "Audio/Music/LevelScene/ThemeS2-2",
            "Audio/Music/LevelScene/ThemeS2-3",
            "Audio/Music/LevelScene/ThemeS2-4",
        };

        public static readonly List<string> MiscMagicPunk = new List<string>()
        {
            "Audio/Music/Misc/MagicPunk/hightempo",
            "Audio/Music/Misc/MagicPunk/lobby",
        };

        public static readonly List<string> ServerAndRoomSelection = new List<string>()
        {
            "Audio/Music/ServerSelection/ServerSelectionBeta",
            "Audio/Music/ServerSelection/ServerSelectionS1",
            "Audio/Music/ServerSelection/ServerSelectionS2-1",
            "Audio/Music/ServerSelection/ServerSelectionS2-2",
        };
    }
}
