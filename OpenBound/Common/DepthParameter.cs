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

namespace OpenBound.Common
{
    public class DepthParameter
    {
        //Game Scenario
        public const float Background = 0f;
        public const float BackgroundAnim                 = Background + 0.01f;
        public const float WeatherEffectTornadoProjectile = Background + 0.019f;
        public const float WeatherEffect                  = Background + 0.02f;
        public const float CameraFade                     = Background + 0.021f;
        public const float Foreground                     = Background + 0.03f;
        //0.03

        //LoadingScreen requires a bit more variables
        public const float LoadingScreenMinimapForeground     = 0.001f;
        public const float LoadingScreenMinimapSpawnPointBox  = 0.002f;
        public const float LoadingScreenMinimapSpawnPointText = 0.004f;
        public const float LoadingScreenBackground            = 0.03f;

        //
        public const float MatchGridMinimapPortraitBackground = 0.005f;
        public const float MatchGridMinimapPortraitForeground = 0.006f;

        //Avatar
        public const float AvatarExtra   = 0.042f;
        public const float AvatarFlag    = 0.043f;
        public const float AvatarHead    = 0.044f;
        public const float AvatarGoggles = 0.045f;
        public const float AvatarBody    = 0.046f;
        public const float AvatarPet     = 0.047f;
        public const float AvatarMisc    = 0.048f;

        //Mobile
        //Player's button shadow goes in here~
        public const float Mobile                     = 0.05f;
        public const float MobileSatellite            = Mobile + 0.01f;

        public const float Projectile                 = Mobile + 0.02f;
        public const float ProjectileSFXBase          = Mobile + 0.021f;
        public const float ProjectileSFX              = Mobile + 0.03f;

        //Croshair
        public const float CrosshairFrame             = Mobile + 0.04f;
        public const float CrosshairAimRangeIndicator = Mobile + 0.05f;
        public const float CrosshairPointer           = Mobile + 0.06f;

        //HealthBar
        public const float HealthBar                  = Mobile + 0.07f;

        //Interface - Nameplates
        //Nameplate's text outline goes in here~
        public const float Nameplate                  = Mobile + 0.09f;

        public const float InGameTextBalloonBase      = Mobile + 0.11f;
        //0.16

        //HUD
        public const float HUDBackground = 0.2f;
        public const float HUDForeground = HUDBackground + 0.01f;
        public const float HUDL1         = HUDBackground + 0.02f;
        public const float HUDL2         = HUDBackground + 0.04f;
        public const float HUDL3         = HUDBackground + 0.05f;
        public const float HUDL4         = HUDBackground + 0.06f;
        public const float HUDL5         = HUDBackground + 0.07f;
        //0.26

        //Game Interface - Menus
        public const float InterfaceButton                     = 0.3f;
        public const float InterfaceButtonIcon                 = InterfaceButton + 0.01f;
        public const float InterfaceButtonAnimatedIcon         = InterfaceButton + 0.02f;
        //Text outline should be in here~
        public const float InterfaceButtonText                 = InterfaceButton + 0.04f;
        public const float InterfaceTextBalloonBase            = InterfaceButton + 0.06f;
        //0.36

        //Game Interface - Popups
        public const float InterfacePopupBackground = 0.4f;
        public const float InterfacePopupForeground = InterfacePopupBackground + 0.01f;
        public const float InterfacePopupButtons    = InterfacePopupBackground + 0.02f;
        public const float InterfacePopupButtonIcon = InterfacePopupBackground + 0.03f;
        public const float InterfacePopupText       = InterfacePopupBackground + 0.04f;

        //Game Interface - Popups - Message
        public const float InterfacePopupMessageBackground = InterfacePopupBackground + 0.05f;
        public const float InterfacePopupMessageForeground = InterfacePopupBackground + 0.06f;
        public const float InterfacePopupMessageButtons    = InterfacePopupBackground + 0.07f;
        public const float InterfacePopupMessageText       = InterfacePopupBackground + 0.09f;
        //0.49

        //Cursor
        public const float Cursor = 0.8f;

        //Scene Transitioning Effects
        public const float SceneTransitioningEffectBase = 0.9f;
        public const float SceneTransitioningEffectL1   = 0.91f;
        public const float SceneTransitioningEffectL2   = 0.92f;
        public const float SceneTransitioningEffectL3   = 0.93f;
        public const float SceneTransitioningEffectL4   = 0.94f;
        //0.99
    }
}
