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

using OpenBound_Network_Object_Library.Entity;

namespace OpenBound.Common
{
    class SoundEffectParameter
    {
        //Logo
        public const string GameLogo = "Audio/SFX/Logo/GameLogo";
        public const string CompanyLogo = "Audio/SFX/Logo/CompanyLogo";

        //InGame
        //-- Popup
        //-- -- GameResults
        public const string InGamePopupGameResultsVictory = "Audio/SFX/Popup/GameResults/Win";
        public const string InGamePopupGameResultsDefeat = "Audio/SFX/Popup/GameResults/Lose";

        //-- Gameplay
        public const string InGameGameplayClockElapsedSecond = "Audio/SFX/InGame/Gameplay/ClockSecond";
        public const string InGameGameplayNewTurn = "Audio/SFX/InGame/Gameplay/NewTurn";

        //-- Weather
        public const string InGameWeatherWindTransition = "Audio/SFX/InGame/Weather/WindTransition";

        //Tank
        //-- Movement
        public static string MobileMovement(MobileType mobileType) => $"Audio/SFX/Tank/Movement/{mobileType}";
        public static string MobileUnableToMove(MobileType mobileType) => $"Audio/SFX/Tank/Movement/{mobileType}Unable";

        //-- Projectile
        public static string MobileProjectileLaunch(MobileType mobileType, ShotType shotType) => $"Audio/SFX/Tank/Shoot/{mobileType}{shotType}";
        public static string MobileProjectileExplosion(MobileType mobileType, ShotType shotType) => $"Audio/SFX/Tank/Blast/{mobileType}{shotType}";
    }
}
