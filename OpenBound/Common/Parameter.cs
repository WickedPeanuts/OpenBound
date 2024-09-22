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
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.FileManager;
using System;

namespace OpenBound.Common
{
    public class Parameter
    {
        public static void Initialize(LauncherInformation launcherInformation)
        {
            // Player
            GameInformation.Instance.PlayerInformation = launcherInformation.Player;

            GameClientSettingsInformation = launcherInformation.GameClientSettingsInformation;

            launcherInformation.GameClientSettingsInformation.WindowedWidth = Math.Max(
                launcherInformation.GameClientSettingsInformation.WindowedWidth,
                launcherInformation.GameClientSettingsInformation.MenuSupportedResolutionWidth);

            launcherInformation.GameClientSettingsInformation.WindowedHeight = Math.Max(
                launcherInformation.GameClientSettingsInformation.WindowedHeight,
                launcherInformation.GameClientSettingsInformation.MenuSupportedResolutionHeight);

            // Screen Resoltuion
            ScreenResolution = new Vector2(
                launcherInformation.GameClientSettingsInformation.WindowedWidth,
                launcherInformation.GameClientSettingsInformation.WindowedHeight);

            ScreenCenter = ScreenResolution / 2;

            MenuSupportedResolution = new Vector2(
                launcherInformation.GameClientSettingsInformation.MenuSupportedResolutionWidth,
                launcherInformation.GameClientSettingsInformation.MenuSupportedResolutionHeight);

            InGameSupportedResolution = new Vector2(
                launcherInformation.GameClientSettingsInformation.InGameResolutionWidth,
                launcherInformation.GameClientSettingsInformation.InGameResolutionHeight);
        }

        #region Camera
        public const float CameraFadeSpeedFactor = 0.6f;
        public const float CameraMinimumFadeOutTransparency = 0.85f;
        #endregion

        #region Game Configuration
        //Game Configuration File
        public static GameClientSettingsInformation GameClientSettingsInformation { get; private set; }

        //Gameplay Constants
        public const int GameplayConstantDelayTimerFirstTurn = 2000;
        public const int GameplayConstantDelayTimerWindChange = 3000;
        public const float GameplayTimeFrameMaximumDeltaTime = 1 / 60f;

        //Screen Resolution
        public static Vector2 ScreenResolution { get; set; }
        public static Vector2 ScreenCenter { get; set; }
        public static Vector2 MenuSupportedResolution { get; private set; }
        public static Vector2 InGameSupportedResolution { get; private set; }
        public static bool IsFullScreen => GameClientSettingsInformation.IsFullScreen;
        public static bool IsVSyncOn => GameClientSettingsInformation.IsVSyncOn;
        public static bool IsAntiAliasingOn => GameClientSettingsInformation.IsMultiSamplingEnabled;

        public static bool ShouldChangeBGScale = true;
        #endregion

        #region BitMasks
        //Camera
        public const byte TopMask = 0x1;
        public const byte BottomMask = 0x1 << 1;
        public const byte LeftMask = 0x1 << 2;
        public const byte RightMask = 0x1 << 3;
        #endregion

        #region Interface
        public static readonly Color NameplateGuildColor = new Color(242, 198, 75);
        public static readonly Color NameplateGuildOutlineColor = new Color(95, 81, 16);

        public const float InterfaceNumericTextFieldUpdateRate = 0.05f;
        public const float InterfaceNumericTextFieldFactorNumber = 20f;

        public const float InterfaceMinimapScrollSpeed = 10f;

        public const float InterfaceInGameTextBoxTimeToStartFading = 10;
        public const float InterfaceInGameTextBoxTimeFadeTime = 5;

        public const int InterfaceInGameTurnTimeBalloonMaximumNumber = 20;

        public static readonly Color InterfaceAvatarShopButtonGoldColor = new Color(237, 232, 128);
        public static readonly Color InterfaceAvatarShopButtonGoldOutlineColor = new Color(85, 88, 59);

        public static readonly Color InterfaceAvatarShopButtonCashColor = new Color(137, 190, 234);
        public static readonly Color InterfaceAvatarShopButtonCashOutlineColor = new Color(9, 55, 104);

        public const int InterfaceAvatarShopAvatarButtonNewStampDay = 14; //Two weeks
        #endregion

        #region Misc
        //Is Loading Textures Flag
        public static bool IsLoadingGameAssets = true;

        //Random number generator
        public static readonly Random Random = new Random(/*0*/);

        //Movement
        public const float TankMovementMaxYStepping = 6f;
        public const float TankMovementMinYStepping = 6f;
        public const float TankMovementSpeed = 1f;
        public const float TankMovementInitialGravity = 3f;

        public const float TankMovementGravityFactor = 0.15f;
        public const float TankMovementGravityDelay = 0.05f;
        public const float TankMovementSidewaysDelay = 0.1f;

        //Mobile Rotation Calculation
        public const int TankMovementRotationCalculationOffsetX = 12;
        public const int TankMovementRotationCalculationOffsetY = 25;

        //Projectiles
        public const float ProjectileMovementGravity = 9.8f * 20f;
        public const float ProjectileMovementForceFactor = 100f;
        public const float ProjectileMovementTimeElapsedPerInteraction = 0.0005f;
        public const float ProjectileMovementTotalTimeElapsed = 0.015f; //At 60fps it should be 0.01666667. The projectiles are going to be slightly faster.
        public const float ProjectileMovementFixedElapedTime = 1f / 60f;

        public const int ProjectilePlayableMapAreaYLimit = -300;

        //Camera
        public const float CameraSlidingSensibility = 10;
        public const float CameraSlidingStartThreshold = 3;

        public const float CameraTrackingDampeningFactor = 25;

        //Explosion Blackmask
        public const float BlastBlackmaskRadius = 3f;

        public static readonly float[] BlastBlackmaskExplosionRadiusColorFactor = new float[] { 0.4f, 0.4f, 0.4f, 1f };

        //HUD
        //-- Crosshair
        public static readonly Vector2 HUDCrosshairAnimationStartingScale = new Vector2(5f, 5f);
        public const float HUDCrosshairAnimationTotalTime = 0.25f;

        public static readonly Color HUDCrosshairFadeColor = new Color(0.6f, 0.6f, 0.6f, 0.6f);
        public const float HUDCrosshairAngleIndicator = 0.62f;
        public const float HUDCrosshairInitialSensibility = 0.15f;
        public const float HUDCrosshairSensibilityReductionFactor = 0.008f;
        public const float HUDCrosshairMinimumSensibility = 0.03f;

        //-- Health Bar
        public const float HealthBarLowHealthThreshold = 0.30f;

        public static readonly Color HealthBarColorGreen = new Color(99, 182, 74);
        public static readonly Color HealthBarColorBlue = new Color(33, 56, 198);
        public static readonly Color HealthBarColorBlack = new Color(41, 44, 41);
        public static readonly Color HealthBarColorRed = new Color(198, 20, 0);

        public static readonly Vector2 HealthBarOffset = new Vector2(0, 40);

        //-- Shot Strenght
        public static readonly float ShotStrenghtTimeLimit = 0.04f;
        public static readonly float ShotStrenghtBarStep = 2;

        //Animations
        //-- InGame - DeathAnimation
        public static readonly Vector2 AnimationInGameDeathAnimationSpeed = new Vector2(0, -100);

        //-- InGame - WindCompass
        public const float HUDAnimationCompassWindForceFactor = 5f / NetworkObjectParameters.WeatherMaximumWindForce;
        #endregion

        #region Mobile
        //Satellite
        //-- SS Animation
        public const float SatelliteSSAnimationTotalAngle = (float)(2 * Math.PI);
        public const float SatelliteSSAnimationInitialAngle = 0;
        public const float SatelliteSSAnimationTotalMotionTime = 0.4f;

        public static readonly Vector2 SatelliteSSAnimationMovementRange = new Vector2(45, 0);

        //-- Attacking
        public const float StelliteAttackSpeedFactor = 2.5f;

        //-- Movement
        public const float SatelliteMovementStartingSpeed = 1000f;
        #endregion

        #region Text Color
        //Popups
        //-- In-game - Options
        public static readonly Color TextColorPopupIngameOptionsCategory = new Color(255, 172, 17);
        public static readonly Color TextColorPopupIngameOptionsSubCategory = new Color(205, 206, 150);
        public static readonly Color TextColorPopupIngameOptionsElement = new Color(205, 206, 205);

        //-- GameRoom - SelectMobile
        public static readonly Color TextColorPopupSelectMobilePortrait = new Color(0, 56, 107);

        //-- Chat
        public static readonly Color TextColorTextBoxSelectedMessage = Color.LightGray;

        //-- Loading Screen - Minimap
        public static readonly Color TextColorTeamRed = new Color(246, 161, 98);
        public static readonly Color TextColorTeamBlue = new Color(98, 149, 230);

        //-- PopupSelectItem - Item Colors
        public static readonly Color TextColorItemRed = new Color(156, 56, 16);
        public static readonly Color TextColorItemGreen = new Color(98, 149, 74);
        public static readonly Color TextColorItemBlue = new Color(82, 121, 164);
        public static readonly Color TextColorItemPurple = new Color(131, 105, 131);
        #endregion

        #region Fonts
        public static readonly FontTextType AnimatedButtonFont = FontTextType.Consolas10Family;
        public static readonly FontTextType ServerButtonFont = FontTextType.Consolas11;
        #endregion

        #region Special Effects
        public const float GroundParticleMaximumRotatingSpeed = (float)Math.PI * 50f;
        public const float GroundParticleMaximumScalingFactor = 0.5f;
        public const float GroundParticleInitialAlphaFactor = 0.3f;
        public const float GroundParticleInitialYSpeed = -500f;
        public const float GroundParticleMaximumYSpeed = 2000;
        public const float GroundParticleMaximumXSpeed = 2000;
        public const float GroundParticleWindInfluenceFactor = 5f;
        public const float GroundParticleInitialYAcceleration = 500f;
        public const int GroundParticleSpreadFactor = 5;

        public const float BaseSmokeParticleEmissionTime = 0.1f;
        #endregion

        #region Mobile Projectile and Balancing
        //Generic
        public const float ProjectileTeleportationBeaconMass = 10f;
        public const float ProjectileTeleportationBeaconWindInfluence = 0f;

        //Armor
        public const int ProjectileArmorS1ExplosionRadius = 25;
        public const int ProjectileArmorS2ExplosionRadius = 25;
        public const int ProjectileArmorS2EExplosionRadius = 20;
        public const int ProjectileArmorSSExplosionRadius = 25;
        public const int ProjectileArmorSSEExplosionRadius = 25;

        public const int ProjectileArmorS1BaseDamage = 150;
        public const int ProjectileArmorS2BaseDamage = 200;
        public const int ProjectileArmorS2EBaseDamage = 80;
        public const int ProjectileArmorSSBaseDamage = 150;
        public const int ProjectileArmorSSEBaseDamage = 50;

        public const float ProjectileArmorS1Mass = 14f;
        public const float ProjectileArmorS2Mass = 14f;
        public const float ProjectileArmorSSMass = 14f;

        public const float ProjectileArmorS1WindInfluence = 1.2f;
        public const float ProjectileArmorS2WindInfluence = 1.2f;
        public const float ProjectileArmorSSWindInfluence = 1.2f;

        public const float ProjectileArmorSSTransformTime = 2f;

        //Bigfoot
        public const int ProjectileBigfootS1ExplosionRadius = 25;
        public const int ProjectileBigfootS2ExplosionRadius = 25;
        public const int ProjectileBigfootSSExplosionRadius = 25;

        public const int ProjectileBigfootS1BaseDamage = 90;
        public const int ProjectileBigfootS2BaseDamage = 70;
        public const int ProjectileBigfootSSBaseDamage = 90;

        public const float ProjectileBigfootS1Mass = 14f;
        public const float ProjectileBigfootS2Mass = 14f;
        public const float ProjectileBigfootSSMass = 14f;

        public const float ProjectileBigfootS1WindInfluence = 1.2f;
        public const float ProjectileBigfootS2WindInfluence = 1.2f;
        public const float ProjectileBigfootSSWindInfluence = 1.2f;

        //Dragon
        public const float ProjectileParticleNewEmissionMaxDistance = 30f;

        public const int ProjectileDragonS1ExplosionRadius = 25;
        public const int ProjectileDragonS2ExplosionRadius = 25;
        public const int ProjectileDragonSSExplosionRadius = 25;

        public const int ProjectileDragonS1BaseDamage = 100;
        public const int ProjectileDragonS2BaseDamage = 100;
        public const int ProjectileDragonSSBaseDamage = 100;

        public const float ProjectileDragonS1Mass = 10f;
        public const float ProjectileDragonS2Mass = 10f;
        public const float ProjectileDragonSSMass = 10f;

        public const float ProjectileDragonS1WindInfluence = 1.4f;
        public const float ProjectileDragonS2WindInfluence = 1.4f;
        public const float ProjectileDragonSSWindInfluence = 1.4f;

        public const float ProjectileDragonSSEAccelerationStartingFactor = 200f;
        public const float ProjectileDragonSSESpeedStartingFactor = 100f;

        //Mage
        public const int ProjectileMageS1ExplosionRadius = 25;
        public const int ProjectileMageS2ExplosionRadius = 25;
        public const int ProjectileMageS2EExplosionRadius = 20;
        public const int ProjectileMageSSExplosionRadius = 25;
        public const int ProjectileMageSSEExplosionRadius = 100;

        public const int ProjectileMageS1BaseDamage = 150;
        public const int ProjectileMageS2BaseDamage = 200;
        public const int ProjectileMageS2EBaseDamage = 80;
        public const int ProjectileMageSSBaseDamage = 300;

        public const float ProjectileMageS1Mass = 14f;
        public const float ProjectileMageS2Mass = 14f;
        public const float ProjectileMageSSMass = 14f;

        public const float ProjectileMageS1WindInfluence = 1.2f;
        public const float ProjectileMageS2WindInfluence = 1.2f;
        public const float ProjectileMageSSWindInfluence = 1.2f;

        //Ice
        public const int ProjectileIceS1ExplosionRadius = 25;
        public const int ProjectileIceS2ExplosionRadius = 25;
        public const int ProjectileIceSSExplosionRadius = 25;

        public const int ProjectileIceS1BaseDamage = 180;
        public const int ProjectileIceS2BaseDamage = 220;
        public const int ProjectileIceSSBaseDamage = 320;

        public const float ProjectileIceS1Mass = 14f;
        public const float ProjectileIceS2Mass = 14f;
        public const float ProjectileIceSSMass = 14f;

        public const float ProjectileIceS1WindInfluence = 1.2f;
        public const float ProjectileIceS2WindInfluence = 1.2f;
        public const float ProjectileIceSSWindInfluence = 1.2f;

        public const int ProjectileIceS2DefenseReduction = -5;
        public const int ProjectileIceS2MaximumDefenseReduction = -35;

        //Knight
        //-- Satellite
        public const float ProjectileKnightDistanceBetweenShots = 15f;
        public const float ProjectileKnightSpeed = 800f;
        public const float ProjectileKnightInitialAlpha = 0.9f;
        public const float ProjectileKnightTraceDistanceOffset = 45f;
        public const float ProjectileKnightTraceAlphaDecay = .04f;
        public const float ProjectileKnightTraceAlphaDecayFactor = .04f;
        public const float ProjectileKnightS1OwnerOffset = -150f;
        public const float ProjectileKnightS2OwnerOffset = -300f;
        public const float ProjectileKnightSSOwnerOffset = -300f;

        public const int ProjectileKnightExplosionRadius = 25;
        public const int ProjectileKnightSwordBaseDamage = 80;

        public const float ProjectileKnightS1Mass = 10f;
        public const float ProjectileKnightS2Mass = 10f;
        public const float ProjectileKnightSSMass = 10f;

        public const float ProjectileKnightS1WindInfluence = 1.4f;
        public const float ProjectileKnightS2WindInfluence = 1.4f;
        public const float ProjectileKnightSSWindInfluence = 1.4f;

        //Raon
        public const int ProjectileRaonLauncherS1ExplosionRadius = 15;
        public const int ProjectileRaonLauncherS2ExplosionRadius = 15;
        public const int ProjectileRaonLauncherSSExplosionRadius = 30;

        public const int ProjectileRaonLauncherS1BaseDamage = 100;
        public const int ProjectileRaonLauncherS2BaseDamage = 200;
        public const int ProjectileRaonLauncherSSBaseDamage = 500;

        public const float ProjectileRaonLauncherS1Mass = 10f;
        public const float ProjectileRaonLauncherS2Mass = 10f;
        public const float ProjectileRaonLauncherSSMass = 10f;

        public const float ProjectileRaonLauncherS1WindInfluence = 1.2f;
        public const float ProjectileRaonLauncherS2WindInfluence = 1.2f;
        public const float ProjectileRaonLauncherSSWindInfluence = 1.2f;

        public const float ProjectileRaonLauncherS2MineTurnFreezetime = 0.5f;

        public const int ProjectileRaonLauncherS2MineMaximumStepsPerTurn = 90;
        public const int ProjectileRaonLauncherS2MineSquaredProximityRange = 90 * 90;

        public const int ProjectileRaonLauncherSSMineMaximumStepsPerTurn = ProjectileRaonLauncherS2MineMaximumStepsPerTurn * 4;

        //Trico
        public const int ProjectileTricoS1ExplosionRadius = 25;
        public const int ProjectileTricoS2ExplosionRadius = 25;
        public const int ProjectileTricoS2EExplosionRadius = 20;
        public const int ProjectileTricoSSExplosionRadius = 25;
        public const int ProjectileTricoSSEExplosionRadius = 25;

        public const int ProjectileTricoS1BaseDamage = 200;
        public const int ProjectileTricoS2BaseDamage = 180;
        public const int ProjectileTricoSSBaseDamage = 120;

        public const float ProjectileTricoS1Mass = 10f;
        public const float ProjectileTricoS2Mass = 10f;
        public const float ProjectileTricoSSMass = 10f;

        public const float ProjectileTricoS1WindInfluence = 1.2f;
        public const float ProjectileTricoS2WindInfluence = 1.2f;
        public const float ProjectileTricoSSWindInfluence = 1.2f;

        public const float ProjectileTricoSSExtraBlastTime = 0.1f;
        public const int ProjectileTricoSSNumberOfExplosionsFactor = 45;
        public const int ProjectileTricoSSNumberOfExplosions = 360 / ProjectileTricoSSNumberOfExplosionsFactor;
        public static float ProjectileTricoSSRotationOffset = MathHelper.ToRadians(ProjectileTricoSSNumberOfExplosionsFactor);
        public static Vector2 ProjectileTricoSSExplosionOffset = new Vector2(20, 0);

        //Turtle
        public const int ProjectileTurtleS1ExplosionRadius = 25;
        public const int ProjectileTurtleS2ExplosionRadius = 25;
        public const int ProjectileTurtleS2EExplosionRadius = 20;
        public const int ProjectileTurtleSSExplosionRadius = 25;
        public const int ProjectileTurtleSSEExplosionRadius = 15;

        public const int ProjectileTurtleS1BaseDamage = 150;
        public const int ProjectileTurtleS2BaseDamage = 200;
        public const int ProjectileTurtleS2EBaseDamage = 80;
        public const int ProjectileTurtleSSBaseDamage = 50;
        public const int ProjectileTurtleSSEBaseDamage = 60;

        public const float ProjectileTurtleS1Mass = 14f;
        public const float ProjectileTurtleS2Mass = 14f;
        public const float ProjectileTurtleSSMass = 14f;

        public const float ProjectileTurtleS1WindInfluence = 1.2f;
        public const float ProjectileTurtleS2WindInfluence = 1.2f;
        public const float ProjectileTurtleSSWindInfluence = 1.2f;

        public const float ProjectileTurtleS2AngleOffsetTimer = 2f;
        public const float ProjectileTurtleS2AngleOffsetFactor = 0.8f;
        public const float ProjectileTurtleS2RotationFactor = 1.5f;

        public const float ProjectileTurtleSSTransformTime = 1.5f;
        public const float ProjectileTurtleSSAngleOffsetDegrees = 3;
        public const float ProjectileTurtleSSBubbleNumber = 6;
        public const float ProjectileTurtleSSDampeningFactor = 3f;

        //Lightning

        //Projectile
        public const int ProjectileLightningS1ExplosionRadius = 25;
        public const int ProjectileLightningS2ExplosionRadius = 25;
        public const int ProjectileLightningSSExplosionRadius = 25;
        public const int ProjectileLightningSSEExplosionRadius = 100;

        //Lightning Bolt
        public const int ProjectileLightningS1ElectricityExplosionRadius = 25;
        public const int ProjectileLightningS1ElectricityEExplosionRadius = 25;
        public const float ProjectileLightningS1ElectricityAngle = MathHelper.PiOver2;

        public const float ProjectileLightningElectricityFadeTime = 0.7f;


        public const int ProjectileLightningS1BaseDamage = 95;
        public const int ProjectileLightningS1ElectricityBaseDamage = 95;
        public const int ProjectileLightningS1ElectricityEBaseDamage = 55;

        public const int ProjectileLightningS2BaseDamage = 75;

        public const int ProjectileLightningS2ElectricityExplosionRadius = 25;
        public const int ProjectileLightningS2ElectricityEExplosionRadius = 25;

        public const int ProjectileLightningS2ElectricityBaseDamage = 100;
        public const int ProjectileLightningS2ElectricityEBaseDamage = 35;


        public const int ProjectileLightningSSBaseDamage = 200;

        public const int ProjectileLightningSSElectricityExplosionRadius = 25;
        public const int ProjectileLightningSSElectricityEExplosionRadius = 25;

        public const int ProjectileLightningSSElectricityBaseDamage = 95;
        public const int ProjectileLightningSSElectricityEBaseDamage = 55;

        public const float ProjectileLightningSSElectricityAngle = MathHelper.PiOver2;
        public static readonly float[] ProjectileLightningS2AnglesOffset = new float[4]
        {
            1 * MathHelper.PiOver4,
            3 * MathHelper.PiOver4,
            5 * MathHelper.PiOver4,
            7 * MathHelper.PiOver4
        };

        public const float ProjectileLightningS1Mass = 14f;
        public const float ProjectileLightningS2Mass = 14f;
        public const float ProjectileLightningSSMass = 14f;

        public const float ProjectileLightningS1WindInfluence = 1.2f;
        public const float ProjectileLightningS2WindInfluence = 1.2f;
        public const float ProjectileLightningSSWindInfluence = 1.2f;
        #endregion

        #region WeatherEffects
        public const float WeaterEffectRandomFlipbookUpdateTimer = 0.1f;
        public const float WeatherEffectVerticalScrollingUpdateSpeed = 50f;
        public const float WeatherEffectFadeTime = 1f;

        //Tornado
        public const float WeatherEffectTornadoMinimumProjectileSpeed = 25f;

        //Force
        public const float WeatherEffectForceDamageIncreaseFactor = 1.2f;
        public const float WeatherEffectForceDamageIncreaseValue = 10f;
        public const float WeatherEffectForceSpawnParticleStartingTime = 0.5f;

        //Weakness
        public const float WeatherEffectWeaknessDamageDecreaseFactor = 1.2f;
        public const float WeatherEffectWeaknessDamageDecreaseValue = 10f;
        public static Color WeatherEffectWeaknessColorModifier = new Color(30, 30, 30, 255);

        //Mirror
        public const float WeatherEffectMirrorDamageIncreaseFactor = 1.1f;
        public const float WeatherEffectMirrorDamageIncreaseValue = 5f;
        public const float WeatherEffectMirrorDistanceFromTopReduction = 2 / 3f;

        //Electricity
        public const int WeatherEffectElectricityBaseDamage = 0;
        public const int WeatherEffectElectricityExplosionRadius = 10;
        public const int WeatherEffectElectricityEExplosionRadius = 30;
        public const int WeatherEffectElectricityEBaseDamage = 30;

        //Thor
        public static Color NeonGreen = new Color(057, 255, 020, 255 / 4);
        public static Color NeonBlue = new Color(027, 003, 163, 255 / 4);
        public static Color NeonYellow = new Color(204, 255, 000, 255 / 4);
        public static Color NeonRed = new Color(255, 007, 058, 255 / 4);
        public static Color NeonWhite = new Color(255, 255, 255, 255 / 4);
        public static Color[] ColorGradient = new Color[] { NeonGreen, NeonBlue, NeonYellow, NeonRed, NeonWhite };

        public const int ProjectileThorExplosionRadius = 25;
        public const int ProjectileThorBaseDamage = 40;
        public const int ProjectileThorBaseDamagePerLevel = 20;
        public const int ProjectileThorBeamDistanceThreshold = 2000;
        public static int[] ThorExperienceTable = new int[] { 800, 1400, 2000, 2400, 3000 };
        #endregion
    }
}
