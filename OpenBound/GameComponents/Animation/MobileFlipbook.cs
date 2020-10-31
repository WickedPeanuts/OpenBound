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
using OpenBound.Common;
using OpenBound_Network_Object_Library.Entity;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound.GameComponents.Animation
{
    public enum ActorFlipbookState
    {
        Stand, //Normal

        Active, //Used only by thor

        //Mobile-exclusive flipbook states
        StandLowHealth, //WNormal

        Moving, //Move
        MovingLowHealth, //WMove

        UnableToMove, //Unmove

        Emotion1, //Emotion1
        Emotion2, // 1/25 (no need)

        BeingDamaged1, //fDamage
        BeingDamaged2, //bDamage
        BeingShocked, //Shock
        BeingFrozen, //ICE

        ChargingS1, //bFire1 CHARGING t1
        ShootingS1, //Fire1

        ChargingS2, //bFire2 CHARGING t2
        ShootingS2, //Fire2

        ChargingSS, //bsFire CHARGING SS SHOT
        ShootingSS, //sFire

        UsingItem, //Item

        DepletingShield, //Shield

        Dead, //Dead

        Falling, //Drop

        All, //SFX

        //Raon's Mine
        Dormant,
        Activated,
    }

    public class MobileFlipbook : Flipbook
    {
        static readonly Dictionary<MobileType, Dictionary<ActorFlipbookState, AnimationInstance>> MobileStatePresets = new Dictionary<MobileType, Dictionary<ActorFlipbookState, AnimationInstance>>()
        {
            {
                MobileType.Random,
                new Dictionary<ActorFlipbookState, AnimationInstance>()
                {
                    { ActorFlipbookState.Stand, new AnimationInstance() { StartingFrame = 000, EndingFrame = 011, TimePerFrame = 1/20f } },
                }
            },
            #region Armor
            {
                MobileType.Armor,
                new Dictionary<ActorFlipbookState, AnimationInstance>()
                {
                    { ActorFlipbookState.Stand,           new AnimationInstance() { StartingFrame = 000, EndingFrame = 020, TimePerFrame = 1/20f } },
                    { ActorFlipbookState.StandLowHealth,  new AnimationInstance() { StartingFrame = 151, EndingFrame = 171, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.Moving,          new AnimationInstance() { StartingFrame = 021, EndingFrame = 039, TimePerFrame = 1/17f } },
                    { ActorFlipbookState.MovingLowHealth, new AnimationInstance() { StartingFrame = 169, EndingFrame = 189, TimePerFrame = 1/17f } },

                    { ActorFlipbookState.UnableToMove,    new AnimationInstance() { StartingFrame = 191, EndingFrame = 210, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Emotion1,        new AnimationInstance() { StartingFrame = 349, EndingFrame = 378, TimePerFrame = 1/29f } },
                    { ActorFlipbookState.Emotion2,        new AnimationInstance() { StartingFrame = 312, EndingFrame = 349, TimePerFrame = 1/29f } },

                    { ActorFlipbookState.BeingDamaged1,   new AnimationInstance() { StartingFrame = 378, EndingFrame = 405, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingDamaged2,   new AnimationInstance() { StartingFrame = 404, EndingFrame = 430, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingShocked,    new AnimationInstance() { StartingFrame = 049, EndingFrame = 069, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.BeingFrozen,     new AnimationInstance() { StartingFrame = 216, EndingFrame = 216, TimePerFrame = 1f } },

                    { ActorFlipbookState.ChargingS1,      new AnimationInstance() { StartingFrame = 091, EndingFrame = 110, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS1,      new AnimationInstance() { StartingFrame = 040, EndingFrame = 049, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.ChargingS2,      new AnimationInstance() { StartingFrame = 091, EndingFrame = 110, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS2,      new AnimationInstance() { StartingFrame = 291, EndingFrame = 318, TimePerFrame = 1/26f } },

                    { ActorFlipbookState.ChargingSS,      new AnimationInstance() { StartingFrame = 091, EndingFrame = 110, TimePerFrame = 1/14f } },
                    { ActorFlipbookState.ShootingSS,      new AnimationInstance() { StartingFrame = 429, EndingFrame = 454, TimePerFrame = 1/20f } },

                    { ActorFlipbookState.UsingItem,       new AnimationInstance() { StartingFrame = 230, EndingFrame = 250, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Dead,            new AnimationInstance() { StartingFrame = 071, EndingFrame = 089, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Falling,         new AnimationInstance() { StartingFrame = 131, EndingFrame = 149, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.All,             new AnimationInstance() { StartingFrame = 000, EndingFrame = 456, TimePerFrame = 1/18f } },
                }
            },
            #endregion
            #region Bigfoot
            {
                MobileType.Bigfoot,
                new Dictionary<ActorFlipbookState, AnimationInstance>()
                {
                    { ActorFlipbookState.Stand,           new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/20f } },
                    { ActorFlipbookState.StandLowHealth,  new AnimationInstance() { StartingFrame = 045, EndingFrame = 064, TimePerFrame = 1/18f } },
                    { ActorFlipbookState.Moving,          new AnimationInstance() { StartingFrame = 020, EndingFrame = 044, TimePerFrame = 1/17f } },
                    { ActorFlipbookState.MovingLowHealth, new AnimationInstance() { StartingFrame = 065, EndingFrame = 104, TimePerFrame = 1/17f } },
                    //
                    { ActorFlipbookState.UnableToMove,    new AnimationInstance() { StartingFrame = 105, EndingFrame = 124, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Emotion1,        new AnimationInstance() { StartingFrame = 354, EndingFrame = 393, TimePerFrame = 1/29f } },
                    { ActorFlipbookState.Emotion2,        new AnimationInstance() { StartingFrame = 354, EndingFrame = 393, TimePerFrame = 1/29f } },

                    { ActorFlipbookState.BeingDamaged1,   new AnimationInstance() { StartingFrame = 394, EndingFrame = 418, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingDamaged2,   new AnimationInstance() { StartingFrame = 419, EndingFrame = 441, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingShocked,    new AnimationInstance() { StartingFrame = 147, EndingFrame = 166, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.BeingFrozen,     new AnimationInstance() { StartingFrame = 301, EndingFrame = 301, TimePerFrame = 0f } },

                    { ActorFlipbookState.ChargingS1,      new AnimationInstance() { StartingFrame = 212, EndingFrame = 233, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS1,      new AnimationInstance() { StartingFrame = 125, EndingFrame = 146, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.ChargingS2,      new AnimationInstance() { StartingFrame = 254, EndingFrame = 273, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS2,      new AnimationInstance() { StartingFrame = 125, EndingFrame = 146, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.ChargingSS,      new AnimationInstance() { StartingFrame = 254, EndingFrame = 273, TimePerFrame = 1/14f } },
                    { ActorFlipbookState.ShootingSS,      new AnimationInstance() { StartingFrame = 274, EndingFrame = 293, TimePerFrame = 1/20f } },

                    { ActorFlipbookState.UsingItem,       new AnimationInstance() { StartingFrame = 234, EndingFrame = 253, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Dead,            new AnimationInstance() { StartingFrame = 202, EndingFrame = 211, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Falling,         new AnimationInstance() { StartingFrame = 187, EndingFrame = 201, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.All,             new AnimationInstance() { StartingFrame = 000, EndingFrame = 469, TimePerFrame = 1/18f } },
                }
            },
            #endregion
            #region Dragon
            {
                MobileType.Dragon,
                new Dictionary<ActorFlipbookState, AnimationInstance>()
                {
                    { ActorFlipbookState.Stand,           new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/20f } },
                    { ActorFlipbookState.StandLowHealth,  new AnimationInstance() { StartingFrame = 040, EndingFrame = 059, TimePerFrame = 1/18f } },
                    { ActorFlipbookState.Moving,          new AnimationInstance() { StartingFrame = 020, EndingFrame = 039, TimePerFrame = 1/17f } },
                    { ActorFlipbookState.MovingLowHealth, new AnimationInstance() { StartingFrame = 060, EndingFrame = 079, TimePerFrame = 1/17f } },

                    { ActorFlipbookState.UnableToMove,    new AnimationInstance() { StartingFrame = 080, EndingFrame = 099, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Emotion1,        new AnimationInstance() { StartingFrame = 278, EndingFrame = 297, TimePerFrame = 1/29f } },
                    { ActorFlipbookState.Emotion2,        new AnimationInstance() { StartingFrame = 278, EndingFrame = 297, TimePerFrame = 1/29f } },

                    { ActorFlipbookState.BeingDamaged1,   new AnimationInstance() { StartingFrame = 479, EndingFrame = 503, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingDamaged2,   new AnimationInstance() { StartingFrame = 504, EndingFrame = 528, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingShocked,    new AnimationInstance() { StartingFrame = 388, EndingFrame = 407, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.BeingFrozen,     new AnimationInstance() { StartingFrame = 448, EndingFrame = 448, TimePerFrame = 0f } },

                    { ActorFlipbookState.ChargingS1,      new AnimationInstance() { StartingFrame = 328, EndingFrame = 342, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS1,      new AnimationInstance() { StartingFrame = 100, EndingFrame = 114, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.ChargingS2,      new AnimationInstance() { StartingFrame = 343, EndingFrame = 357, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS2,      new AnimationInstance() { StartingFrame = 115, EndingFrame = 134, TimePerFrame = 1/21f } },

                    { ActorFlipbookState.ChargingSS,      new AnimationInstance() { StartingFrame = 449, EndingFrame = 463, TimePerFrame = 1/14f } },
                    { ActorFlipbookState.ShootingSS,      new AnimationInstance() { StartingFrame = 464, EndingFrame = 478, TimePerFrame = 1/20f } },

                    { ActorFlipbookState.UsingItem,       new AnimationInstance() { StartingFrame = 141, EndingFrame = 160, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Dead,            new AnimationInstance() { StartingFrame = 263, EndingFrame = 277, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Falling,         new AnimationInstance() { StartingFrame = 248, EndingFrame = 262, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.All,             new AnimationInstance() { StartingFrame = 000, EndingFrame = 469, TimePerFrame = 1/18f } },
                }
            },
            #endregion
            #region Mage
            {
                MobileType.Mage,
                new Dictionary<ActorFlipbookState, AnimationInstance>()
                {
                    { ActorFlipbookState.Stand,           new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/20f } },
                    { ActorFlipbookState.StandLowHealth,  new AnimationInstance() { StartingFrame = 040, EndingFrame = 059, TimePerFrame = 1/18f } },
                    { ActorFlipbookState.Moving,          new AnimationInstance() { StartingFrame = 020, EndingFrame = 039, TimePerFrame = 1/17f } },
                    { ActorFlipbookState.MovingLowHealth, new AnimationInstance() { StartingFrame = 060, EndingFrame = 079, TimePerFrame = 1/17f } },

                    { ActorFlipbookState.UnableToMove,    new AnimationInstance() { StartingFrame = 080, EndingFrame = 099, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Emotion1,        new AnimationInstance() { StartingFrame = 322, EndingFrame = 340, TimePerFrame = 1/29f } },
                    { ActorFlipbookState.Emotion2,        new AnimationInstance() { StartingFrame = 322, EndingFrame = 340, TimePerFrame = 1/29f } },

                    { ActorFlipbookState.BeingDamaged1,   new AnimationInstance() { StartingFrame = 341, EndingFrame = 365, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingDamaged2,   new AnimationInstance() { StartingFrame = 366, EndingFrame = 390, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingShocked,    new AnimationInstance() { StartingFrame = 120, EndingFrame = 139, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.BeingFrozen,     new AnimationInstance() { StartingFrame = 167, EndingFrame = 167, TimePerFrame = 0f } },

                    { ActorFlipbookState.ChargingS1,      new AnimationInstance() { StartingFrame = 204, EndingFrame = 222, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS1,      new AnimationInstance() { StartingFrame = 100, EndingFrame = 119, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.ChargingS2,      new AnimationInstance() { StartingFrame = 303, EndingFrame = 321, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS2,      new AnimationInstance() { StartingFrame = 100, EndingFrame = 119, TimePerFrame = 1/21f } },

                    { ActorFlipbookState.ChargingSS,      new AnimationInstance() { StartingFrame = 391, EndingFrame = 409, TimePerFrame = 1/14f } },
                    { ActorFlipbookState.ShootingSS,      new AnimationInstance() { StartingFrame = 100, EndingFrame = 119, TimePerFrame = 1/20f } },

                    { ActorFlipbookState.UsingItem,       new AnimationInstance() { StartingFrame = 243, EndingFrame = 262, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Dead,            new AnimationInstance() { StartingFrame = 179, EndingFrame = 203, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Falling,         new AnimationInstance() { StartingFrame = 160, EndingFrame = 178, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.All,             new AnimationInstance() { StartingFrame = 000, EndingFrame = 411, TimePerFrame = 1/18f } },
                }
            },
            #endregion
            #region Ice
            {
                MobileType.Ice,
                new Dictionary<ActorFlipbookState, AnimationInstance>()
                {
                    { ActorFlipbookState.Stand,           new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/20f } },
                    { ActorFlipbookState.StandLowHealth,  new AnimationInstance() { StartingFrame = 035, EndingFrame = 054, TimePerFrame = 1/18f } },
                    { ActorFlipbookState.Moving,          new AnimationInstance() { StartingFrame = 020, EndingFrame = 034, TimePerFrame = 1/20f } },
                    { ActorFlipbookState.MovingLowHealth, new AnimationInstance() { StartingFrame = 055, EndingFrame = 074, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.UnableToMove,    new AnimationInstance() { StartingFrame = 075, EndingFrame = 094, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Emotion1,        new AnimationInstance() { StartingFrame = 203, EndingFrame = 222, TimePerFrame = 1/29f } },
                    { ActorFlipbookState.Emotion2,        new AnimationInstance() { StartingFrame = 203, EndingFrame = 222, TimePerFrame = 1/29f } },

                    { ActorFlipbookState.BeingDamaged1,   new AnimationInstance() { StartingFrame = 363, EndingFrame = 387, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingDamaged2,   new AnimationInstance() { StartingFrame = 388, EndingFrame = 412, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingShocked,    new AnimationInstance() { StartingFrame = 273, EndingFrame = 292, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.BeingFrozen,     new AnimationInstance() { StartingFrame = 135, EndingFrame = 135, TimePerFrame = 0f } },

                    { ActorFlipbookState.ChargingS1,      new AnimationInstance() { StartingFrame = 223, EndingFrame = 237, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS1,      new AnimationInstance() { StartingFrame = 095, EndingFrame = 114, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.ChargingS2,      new AnimationInstance() { StartingFrame = 238, EndingFrame = 252, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS2,      new AnimationInstance() { StartingFrame = 115, EndingFrame = 129, TimePerFrame = 1/21f } },

                    { ActorFlipbookState.ChargingSS,      new AnimationInstance() { StartingFrame = 313, EndingFrame = 327, TimePerFrame = 1/14f } },
                    { ActorFlipbookState.ShootingSS,      new AnimationInstance() { StartingFrame = 328, EndingFrame = 347, TimePerFrame = 1/20f } },

                    { ActorFlipbookState.UsingItem,       new AnimationInstance() { StartingFrame = 253, EndingFrame = 272, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Dead,            new AnimationInstance() { StartingFrame = 190, EndingFrame = 202, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Falling,         new AnimationInstance() { StartingFrame = 293, EndingFrame = 312, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.All,             new AnimationInstance() { StartingFrame = 000, EndingFrame = 469, TimePerFrame = 1/18f } },
                }
            },
            #endregion
            #region Knight
            {
                MobileType.Knight,
                new Dictionary<ActorFlipbookState, AnimationInstance>()
                {
                    { ActorFlipbookState.Stand,           new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/20f } },
                    { ActorFlipbookState.StandLowHealth,  new AnimationInstance() { StartingFrame = 182, EndingFrame = 199, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.Moving,          new AnimationInstance() { StartingFrame = 020, EndingFrame = 037, TimePerFrame = 1/17f } },
                    { ActorFlipbookState.MovingLowHealth, new AnimationInstance() { StartingFrame = 200, EndingFrame = 217, TimePerFrame = 1/17f } },

                    { ActorFlipbookState.UnableToMove,    new AnimationInstance() { StartingFrame = 103, EndingFrame = 122, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Emotion1,        new AnimationInstance() { StartingFrame = 218, EndingFrame = 247, TimePerFrame = 1/29f } },
                    { ActorFlipbookState.Emotion2,        new AnimationInstance() { StartingFrame = 218, EndingFrame = 247, TimePerFrame = 1/29f } },

                    { ActorFlipbookState.BeingDamaged1,   new AnimationInstance() { StartingFrame = 308, EndingFrame = 332, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingDamaged2,   new AnimationInstance() { StartingFrame = 333, EndingFrame = 357, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingShocked,    new AnimationInstance() { StartingFrame = 288, EndingFrame = 307, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.BeingFrozen,     new AnimationInstance() { StartingFrame = 216, EndingFrame = 216, TimePerFrame = 1f } },

                    { ActorFlipbookState.ChargingS1,      new AnimationInstance() { StartingFrame = 068, EndingFrame = 087, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS1,      new AnimationInstance() { StartingFrame = 088, EndingFrame = 102, TimePerFrame = 1/14f } },

                    { ActorFlipbookState.ChargingS2,      new AnimationInstance() { StartingFrame = 038, EndingFrame = 052, TimePerFrame = 1/14f } },
                    { ActorFlipbookState.ShootingS2,      new AnimationInstance() { StartingFrame = 053, EndingFrame = 067, TimePerFrame = 1/14f } },

                    { ActorFlipbookState.ChargingSS,      new AnimationInstance() { StartingFrame = 358, EndingFrame = 372, TimePerFrame = 1/14f } },
                    { ActorFlipbookState.ShootingSS,      new AnimationInstance() { StartingFrame = 053, EndingFrame = 067, TimePerFrame = 1/14f } },

                    { ActorFlipbookState.UsingItem,       new AnimationInstance() { StartingFrame = 248, EndingFrame = 267, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Dead,            new AnimationInstance() { StartingFrame = 268, EndingFrame = 287, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Falling,         new AnimationInstance() { StartingFrame = 123, EndingFrame = 141, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.All,             new AnimationInstance() { StartingFrame = 000, EndingFrame = 373, TimePerFrame = 1/18f } },
                }
            },
            #endregion
            #region Raon Launcher
            {
                MobileType.RaonLauncher,
                new Dictionary<ActorFlipbookState, AnimationInstance>()
                {
                    { ActorFlipbookState.Stand,           new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/20f } },
                    { ActorFlipbookState.StandLowHealth,  new AnimationInstance() { StartingFrame = 035, EndingFrame = 054, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.Moving,          new AnimationInstance() { StartingFrame = 020, EndingFrame = 034, TimePerFrame = 1/17f } },
                    { ActorFlipbookState.MovingLowHealth, new AnimationInstance() { StartingFrame = 035, EndingFrame = 054, TimePerFrame = 1/17f } },

                    { ActorFlipbookState.UnableToMove,    new AnimationInstance() { StartingFrame = 075, EndingFrame = 094, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Emotion1,        new AnimationInstance() { StartingFrame = 290, EndingFrame = 319, TimePerFrame = 1/29f } },
                    { ActorFlipbookState.Emotion2,        new AnimationInstance() { StartingFrame = 290, EndingFrame = 319, TimePerFrame = 1/29f } },

                    { ActorFlipbookState.BeingDamaged1,   new AnimationInstance() { StartingFrame = 225, EndingFrame = 249, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingDamaged2,   new AnimationInstance() { StartingFrame = 145, EndingFrame = 169, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingShocked,    new AnimationInstance() { StartingFrame = 120, EndingFrame = 144, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.BeingFrozen,     new AnimationInstance() { StartingFrame = 129, EndingFrame = 129, TimePerFrame = 1f } },

                    { ActorFlipbookState.ChargingS1,      new AnimationInstance() { StartingFrame = 191, EndingFrame = 200, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS1,      new AnimationInstance() { StartingFrame = 095, EndingFrame = 119, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.ChargingS2,      new AnimationInstance() { StartingFrame = 191, EndingFrame = 200, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS2,      new AnimationInstance() { StartingFrame = 095, EndingFrame = 119, TimePerFrame = 1/26f } },

                    { ActorFlipbookState.ChargingSS,      new AnimationInstance() { StartingFrame = 320, EndingFrame = 329, TimePerFrame = 1/14f } },
                    { ActorFlipbookState.ShootingSS,      new AnimationInstance() { StartingFrame = 330, EndingFrame = 354, TimePerFrame = 1/20f } },

                    { ActorFlipbookState.UsingItem,       new AnimationInstance() { StartingFrame = 201, EndingFrame = 224, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Dead,            new AnimationInstance() { StartingFrame = 186, EndingFrame = 190, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Falling,         new AnimationInstance() { StartingFrame = 170, EndingFrame = 185, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.All,             new AnimationInstance() { StartingFrame = 000, EndingFrame = 355, TimePerFrame = 1/18f } },
                }
            },
            #endregion
            #region Raon Launcher Mine
            {
                MobileType.RaonLauncherMineS2,
                new Dictionary<ActorFlipbookState, AnimationInstance>()
                {
                    //Normal
                    { ActorFlipbookState.Dormant,   new AnimationInstance(){ StartingFrame = 00, EndingFrame = 19, TimePerFrame = 1/20f, AnimationType = AnimationType.Foward } },
                    //Active
                    { ActorFlipbookState.Activated, new AnimationInstance(){ StartingFrame = 20, EndingFrame = 36, TimePerFrame = 1/20f, AnimationType = AnimationType.Foward } },
                    //Falling
                    { ActorFlipbookState.Falling,   new AnimationInstance(){ StartingFrame = 20, EndingFrame = 36, TimePerFrame = 1/20f, AnimationType = AnimationType.Foward } },
                    //Moving
                    { ActorFlipbookState.Moving,    new AnimationInstance(){ StartingFrame = 37, EndingFrame = 48, TimePerFrame = 1/20f, AnimationType = AnimationType.Foward } },
                }
            },
            #endregion
            #region Raon Launcher Mine
            {
                MobileType.RaonLauncherMineSS,
                new Dictionary<ActorFlipbookState, AnimationInstance>()
                {
                    //Moving
                    { ActorFlipbookState.Moving,    new AnimationInstance(){ StartingFrame = 0, EndingFrame = 11, TimePerFrame = 1/20f, AnimationType = AnimationType.Foward } },
                }
            },
            #endregion
            #region Trico
            {
                MobileType.Trico,
                new Dictionary<ActorFlipbookState, AnimationInstance>()
                {
                    //Done
                    { ActorFlipbookState.Stand,           new AnimationInstance() { StartingFrame = 070, EndingFrame = 084, TimePerFrame = 1/20f } },
                    { ActorFlipbookState.StandLowHealth,  new AnimationInstance() { StartingFrame = 190, EndingFrame = 209, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.Moving,          new AnimationInstance() { StartingFrame = 145, EndingFrame = 159, TimePerFrame = 1/17f } },
                    { ActorFlipbookState.MovingLowHealth, new AnimationInstance() { StartingFrame = 210, EndingFrame = 229, TimePerFrame = 1/17f } },

                    { ActorFlipbookState.UnableToMove,    new AnimationInstance() { StartingFrame = 160, EndingFrame = 174, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Emotion1,        new AnimationInstance() { StartingFrame = 115, EndingFrame = 129, TimePerFrame = 1/29f } },
                    { ActorFlipbookState.Emotion2,        new AnimationInstance() { StartingFrame = 115, EndingFrame = 129, TimePerFrame = 1/29f } },

                    { ActorFlipbookState.BeingDamaged1,   new AnimationInstance() { StartingFrame = 309, EndingFrame = 333, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingDamaged2,   new AnimationInstance() { StartingFrame = 334, EndingFrame = 358, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingShocked,    new AnimationInstance() { StartingFrame = 230, EndingFrame = 244, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.BeingFrozen,     new AnimationInstance() { StartingFrame = 137, EndingFrame = 137, TimePerFrame = 1f } },

                    { ActorFlipbookState.ChargingS1,      new AnimationInstance() { StartingFrame = 020, EndingFrame = 034, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS1,      new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.ChargingS2,      new AnimationInstance() { StartingFrame = 055, EndingFrame = 069, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS2,      new AnimationInstance() { StartingFrame = 035, EndingFrame = 054, TimePerFrame = 1/26f } },

                    { ActorFlipbookState.ChargingSS,      new AnimationInstance() { StartingFrame = 263, EndingFrame = 276, TimePerFrame = 1/14f } },
                    { ActorFlipbookState.ShootingSS,      new AnimationInstance() { StartingFrame = 278, EndingFrame = 293, TimePerFrame = 1/20f } },

                    { ActorFlipbookState.UsingItem,       new AnimationInstance() { StartingFrame = 245, EndingFrame = 262, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Dead,            new AnimationInstance() { StartingFrame = 175, EndingFrame = 189, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Falling,         new AnimationInstance() { StartingFrame = 294, EndingFrame = 308, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.All,             new AnimationInstance() { StartingFrame = 000, EndingFrame = 415, TimePerFrame = 1/18f } },
                }
            },
            #endregion
            #region Turtle
            {
                MobileType.Turtle,
                new Dictionary<ActorFlipbookState, AnimationInstance>()
                {
                    { ActorFlipbookState.Stand,           new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/20f } },
                    { ActorFlipbookState.StandLowHealth,  new AnimationInstance() { StartingFrame = 042, EndingFrame = 061, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.Moving,          new AnimationInstance() { StartingFrame = 020, EndingFrame = 041, TimePerFrame = 1/17f } },
                    { ActorFlipbookState.MovingLowHealth, new AnimationInstance() { StartingFrame = 062, EndingFrame = 083, TimePerFrame = 1/17f } },

                    { ActorFlipbookState.UnableToMove,    new AnimationInstance() { StartingFrame = 084, EndingFrame = 103, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Emotion1,        new AnimationInstance() { StartingFrame = 309, EndingFrame = 333, TimePerFrame = 1/29f } },
                    { ActorFlipbookState.Emotion2,        new AnimationInstance() { StartingFrame = 309, EndingFrame = 333, TimePerFrame = 1/29f } },

                    { ActorFlipbookState.BeingDamaged1,   new AnimationInstance() { StartingFrame = 374, EndingFrame = 398, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingDamaged2,   new AnimationInstance() { StartingFrame = 399, EndingFrame = 423, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingShocked,    new AnimationInstance() { StartingFrame = 249, EndingFrame = 268, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.BeingFrozen,     new AnimationInstance() { StartingFrame = 171, EndingFrame = 171, TimePerFrame = 1f } },

                    { ActorFlipbookState.ChargingS1,      new AnimationInstance() { StartingFrame = 189, EndingFrame = 208, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS1,      new AnimationInstance() { StartingFrame = 104, EndingFrame = 123, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.ChargingS2,      new AnimationInstance() { StartingFrame = 334, EndingFrame = 353, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS2,      new AnimationInstance() { StartingFrame = 354, EndingFrame = 373, TimePerFrame = 1/26f } },

                    { ActorFlipbookState.ChargingSS,      new AnimationInstance() { StartingFrame = 424, EndingFrame = 443, TimePerFrame = 1/14f } },
                    { ActorFlipbookState.ShootingSS,      new AnimationInstance() { StartingFrame = 444, EndingFrame = 463, TimePerFrame = 1/20f } },

                    { ActorFlipbookState.UsingItem,       new AnimationInstance() { StartingFrame = 269, EndingFrame = 288, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Dead,            new AnimationInstance() { StartingFrame = 289, EndingFrame = 308, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Falling,         new AnimationInstance() { StartingFrame = 164, EndingFrame = 183, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.All,             new AnimationInstance() { StartingFrame = 000, EndingFrame = 464, TimePerFrame = 1/18f } },
                }
            },
               #region lightning
               {
                MobileType.Lightning,
                new Dictionary<ActorFlipbookState, AnimationInstance>()
                {
                    { ActorFlipbookState.Stand,           new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/20f } },
                    { ActorFlipbookState.StandLowHealth,  new AnimationInstance() { StartingFrame = 040, EndingFrame = 059, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.Moving,          new AnimationInstance() { StartingFrame = 020, EndingFrame = 039, TimePerFrame = 1/17f } },
                    { ActorFlipbookState.MovingLowHealth, new AnimationInstance() { StartingFrame = 060, EndingFrame = 079, TimePerFrame = 1/17f } },

                    { ActorFlipbookState.UnableToMove,    new AnimationInstance() { StartingFrame = 080, EndingFrame = 099, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Emotion1,        new AnimationInstance() { StartingFrame = 325, EndingFrame = 349, TimePerFrame = 1/29f } },
                    { ActorFlipbookState.Emotion2,        new AnimationInstance() { StartingFrame = 325, EndingFrame = 349, TimePerFrame = 1/29f } },

                    { ActorFlipbookState.BeingDamaged1,   new AnimationInstance() { StartingFrame = 370, EndingFrame = 394, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingDamaged2,   new AnimationInstance() { StartingFrame = 395, EndingFrame = 419, TimePerFrame = 1/24f } },
                    { ActorFlipbookState.BeingShocked,    new AnimationInstance() { StartingFrame = 115, EndingFrame = 134, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.BeingFrozen,     new AnimationInstance() { StartingFrame = 156, EndingFrame = 156, TimePerFrame = 1f } },

                    { ActorFlipbookState.ChargingS1,      new AnimationInstance() { StartingFrame = 195, EndingFrame = 209, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS1,      new AnimationInstance() { StartingFrame = 100, EndingFrame = 114, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.ChargingS2,      new AnimationInstance() { StartingFrame = 295, EndingFrame = 309, TimePerFrame = 1/19f } },
                    { ActorFlipbookState.ShootingS2,      new AnimationInstance() { StartingFrame = 310, EndingFrame = 324, TimePerFrame = 1/26f } },

                    { ActorFlipbookState.ChargingSS,      new AnimationInstance() { StartingFrame = 195, EndingFrame = 209, TimePerFrame = 1/14f } },
                    { ActorFlipbookState.ShootingSS,      new AnimationInstance() { StartingFrame = 100, EndingFrame = 114, TimePerFrame = 1/20f } },

                    { ActorFlipbookState.UsingItem,       new AnimationInstance() { StartingFrame = 210, EndingFrame = 234, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Dead,            new AnimationInstance() { StartingFrame = 175, EndingFrame = 194, TimePerFrame = 1/19f } },

                    { ActorFlipbookState.Falling,         new AnimationInstance() { StartingFrame = 155, EndingFrame = 174, TimePerFrame = 1/18f } },

                    { ActorFlipbookState.All,             new AnimationInstance() { StartingFrame = 000, EndingFrame = 444, TimePerFrame = 1/18f } },
                }
            },
            #endregion
            #endregion
        };

        public ActorFlipbookState State { get; private set; }

        public Dictionary<ActorFlipbookState, AnimationInstance> StatePresets;

        public static MobileFlipbook CreateMobileFlipbook(MobileType MobileType, Vector2 Position)
        {
            MobileFlipbook mb = null;

            string spritePath = $"Graphics/Tank/{MobileType}/CharacterSpritesheet";

            switch (MobileType)
            {
                case MobileType.Random:
                    mb = new MobileFlipbook(
                        Position, new Vector2(14, 23.5f), 28, 47, spritePath,
                        new AnimationInstance(), DepthParameter.Mobile);
                    break;
                case MobileType.Armor:
                    mb = new MobileFlipbook(
                        Position, new Vector2(79, 88), 2900 / 20, 2990 / 23, spritePath,
                        new AnimationInstance(), DepthParameter.Mobile);
                    break;
                case MobileType.Bigfoot:
                    mb = new MobileFlipbook(
                        Position, new Vector2(75, 88), 2580 / 20, 2470 / 19, spritePath,
                        new AnimationInstance(), DepthParameter.Mobile);
                    break;
                case MobileType.Dragon:
                    mb = new MobileFlipbook(
                        Position, new Vector2(72.5f, 85f), 2500 / 20, 3240 / 27, spritePath,
                        new AnimationInstance(), DepthParameter.Mobile);
                    break;
                case MobileType.Mage:
                    mb = new MobileFlipbook(
                        Position, new Vector2(52f, 85f), 2180 / 20, 3150 / 21, spritePath,
                        new AnimationInstance(), DepthParameter.Mobile);
                    break;
                case MobileType.Ice:
                    mb = new MobileFlipbook(
                        Position, new Vector2(78f, 75f), 2960 / 20, 3150 / 21, spritePath,
                        new AnimationInstance(), DepthParameter.Mobile);
                    break;
                case MobileType.Knight:
                    mb = new MobileFlipbook(
                        Position, new Vector2(59, 88), 2160 / 20, 2413 / 19, spritePath,
                        new AnimationInstance(), DepthParameter.Mobile);
                    break;
                case MobileType.RaonLauncher:
                    mb = new MobileFlipbook(
                        Position, new Vector2(59, 95), 2100 / 20, 2340 / 18, spritePath,
                        new AnimationInstance(), DepthParameter.Mobile);
                    break;
                case MobileType.RaonLauncherMineS2:
                    mb = new MobileFlipbook(
                        Position, new Vector2(15.5f, 14.5f), 1519 / 49, 29, spritePath,
                        new AnimationInstance(), DepthParameter.Mobile);
                    break;
                case MobileType.RaonLauncherMineSS:
                    mb = new MobileFlipbook(
                        Position, new Vector2(19f, 17f), 456 / 12, 33, spritePath,
                        new AnimationInstance(), DepthParameter.Mobile);
                    break;
                case MobileType.Trico:
                    mb = new MobileFlipbook(
                        Position, new Vector2(65, 80), 2360 / 20, 2730 / 21, spritePath,
                        new AnimationInstance(), DepthParameter.Mobile);
                    break;
                case MobileType.Turtle:
                    mb = new MobileFlipbook(
                        Position, new Vector2(59, 88), 2300 / 20, 3000 / 24, spritePath,
                        new AnimationInstance(), DepthParameter.Mobile);
                    break;
                case MobileType.Lightning:
                    mb = new MobileFlipbook(
                        Position, new Vector2(56, 83), 2560 / 20, 3013 / 23, spritePath,
                        new AnimationInstance(), DepthParameter.Mobile);
                    break;
            }

            mb.StatePresets = MobileStatePresets[MobileType];
            mb.State = ActorFlipbookState.Moving;
            mb.ChangeState(ActorFlipbookState.Stand);

            return mb;
        }

        private MobileFlipbook(Vector2 position, Vector2 pivot, int spriteWidth, int spriteHeight,
            string texture2DPath, AnimationInstance animationInstance, float layerDepth,
            float rotation = 0)
            : base(position, pivot, spriteWidth, spriteHeight, texture2DPath, animationInstance, layerDepth, rotation)
        { }

        public void EnqueueAnimation(ActorFlipbookState NewState)
        {
            if (State == ActorFlipbookState.Dead) return;
                AppendAnimationIntoCycle(StatePresets[NewState]);
        }

        public void ChangeState(ActorFlipbookState NewState, bool Force = false)
        {
            AnimationInstance selectedPreset;

            if (StatePresets.ContainsKey(NewState))
                selectedPreset = StatePresets[NewState];
            else
                selectedPreset = StatePresets.First().Value;

            State = NewState;

            if (!selectedPreset.Equals(default(AnimationInstance)))
                AppendAnimationIntoCycle(selectedPreset, Force);
        }
    }
}
