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

using Newtonsoft.Json;
using OpenBound_Network_Object_Library.Models;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound_Network_Object_Library.Entity
{
    /// <summary>
    /// "Translation" to the original GB files:
    /// <para> n = S1 </para>
    /// <para> nl = S1 SFX </para>
    /// <para> s = S2 </para>
    /// <para> sl = S2 SFX </para>
    /// <para> p = SS </para>
    /// <para> pl = SS SFX </para>
    /// </summary>
    public enum ShotType
    {
        S1, S2, SS,
        Satellite,
        Dummy, // This state should only be used for DummyProjectile constructor's identification
        TeleportationBeacon,
    }

    public enum MobileAttackArchetype
    {
        Hit,
        Electrical,
        Laser,
        Fire,
    }

    public enum MobileArmorArchetype
    {
        Bionic,
        Steel,
        Shield,
    }

    public struct AimPreset
    {
        public int AimTrueRotationMin;
        public int AimTrueRotationMax;
        public int AimFalseRotationMin;
        public int AimFalseRotationMax;

        public AimPreset(int aimTrueRotationMin, int aimTrueRotationMax, int aimFalseRotationMin, int aimFalseRotationMax)
        {
            AimTrueRotationMin = aimTrueRotationMin;
            AimTrueRotationMax = aimTrueRotationMax;
            AimFalseRotationMin = aimFalseRotationMin;
            AimFalseRotationMax = aimFalseRotationMax;
        }
    }

    public struct DelayPreset
    {
        public int Base;
        public int Shot1;
        public int Shot2;
        public int SS;
    }

    public class MobileStatus
    {
        public int Attack;
        public int Defence;
        public int Energy;
        public int Mobility;
        public int Delay;
        public MobileArmorArchetype ArmorArchetype;
        public MobileAttackArchetype AttackArchetypeS1, AttackArchetypeS2, AttackArchetypeSS;

        public MobileStatus(int attack, int defence, int energy, int mobility, int delay,
            MobileArmorArchetype armorArchetype = MobileArmorArchetype.Steel, MobileAttackArchetype attackArchetypeS1 = MobileAttackArchetype.Hit,
            MobileAttackArchetype attackArchetypeS2 = MobileAttackArchetype.Hit, MobileAttackArchetype attackArchetypeSS = MobileAttackArchetype.Hit)
        {
            Attack = attack;
            Defence = defence;
            Energy = energy;
            Mobility = mobility;
            Delay = delay;
            ArmorArchetype = armorArchetype;
            AttackArchetypeS1 = attackArchetypeS1;
            AttackArchetypeS2 = attackArchetypeS2;
            AttackArchetypeSS = attackArchetypeSS;
        }
    }

    public class MobileMetadata
    {
        // Crosshair Presets, this variable contains all the information about
        // all mobiles crosshairs and supported angles, source: http://creedo.gbgl-hq.com/mobiles.htm
        public static readonly Dictionary<MobileType, Dictionary<ShotType, AimPreset>> AimPresets = new Dictionary<MobileType, Dictionary<ShotType, AimPreset>>()
        {
            //Aduka
            {
                MobileType.Aduka,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(10, 50, 10, 50) },
                    { ShotType.S2, new AimPreset(10, 50, 10, 50) },
                    { ShotType.SS, new AimPreset(10, 50, 10, 50) }
                }
            },
            //Armor
            {
                MobileType.Armor,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(25, 40, 10, 55) },
                    { ShotType.S2, new AimPreset(25, 40, 10, 50) },
                    { ShotType.SS, new AimPreset(10, 50, 10, 50) },
                }
            },
            //ASate
            {
                MobileType.ASate,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(20, 60, 20, 60) },
                    { ShotType.S2, new AimPreset(20, 60, 20, 60) },
                    { ShotType.SS, new AimPreset(25, 55, 25, 55) },
                }
            },
            //Bigfoot
            {
                MobileType.Bigfoot,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(18, 40, 18, 40) },
                    { ShotType.S2, new AimPreset(18, 40, 18, 40) },
                    { ShotType.SS, new AimPreset(23, 35, 23, 35) },
                }
            },
            //Boomer
            {
                MobileType.Boomer,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(35, 65, 10, 90) },
                    { ShotType.S2, new AimPreset(35, 65, 10, 90) },
                    { ShotType.SS, new AimPreset(35, 65, 10, 90) },
                }
            },
            //Dragon
            {
                MobileType.Dragon,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(30, 60, 30, 60) },
                    { ShotType.S2, new AimPreset(30, 60, 30, 60) },
                    { ShotType.SS, new AimPreset(30, 60, 30, 60) },
                }
            },
            //Grub
            {
                MobileType.Grub,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(35, 55, 30, 60) },
                    { ShotType.S2, new AimPreset(35, 55, 30, 60) },
                    { ShotType.SS, new AimPreset(30, 60, 30, 60) },
                }
            },
            //Ice
            {
                MobileType.Ice,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(30, 60, 20, 70) },
                    { ShotType.S2, new AimPreset(30, 60, 20, 70) },
                    { ShotType.SS, new AimPreset(30, 60, 30, 60) },
                }
            },
            //JD
            {
                MobileType.JD,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(10, 40, 10, 40) },
                    { ShotType.S2, new AimPreset(10, 40, 10, 40) },
                    { ShotType.SS, new AimPreset(10, 40, 10, 40) },
                }
            },
            //JFrog - NEED SOURCE
            {
                MobileType.JFrog,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(10, 60, 10, 60) },
                    { ShotType.S2, new AimPreset(10, 60, 10, 60) },
                    { ShotType.SS, new AimPreset(10, 60, 10, 60) },
                }
            },
            //Kalsiddon - NEED SOURCE
            {
                MobileType.Kalsiddon,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(30, 60, 20, 70) },
                    { ShotType.S2, new AimPreset(30, 60, 20, 70) },
                    { ShotType.SS, new AimPreset(30, 60, 30, 60) },
                }
            },
            //Knight
            {
                MobileType.Knight,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(10, 80, 10, 80) },
                    { ShotType.S2, new AimPreset(10, 80, 10, 80) },
                    { ShotType.SS, new AimPreset(10, 80, 10, 80) },
                }
            },
            //Lightning
            {
                MobileType.Lightning,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(23, 35, 18, 40) },
                    { ShotType.S2, new AimPreset(23, 35, 18, 40) },
                    { ShotType.SS, new AimPreset(23, 35, 23, 35) },
                }
            },
            //Mage
            {
                MobileType.Mage,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(15, 50, 15, 50) },
                    { ShotType.S2, new AimPreset(15, 50, 15, 50) },
                    { ShotType.SS, new AimPreset(15, 50, 15, 50) },
                }
            },
            //Nak
            {
                MobileType.Nak,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(10, 50, 10, 50) },
                    { ShotType.S2, new AimPreset(10, 50, 10, 50) },
                    { ShotType.SS, new AimPreset(10, 50, 10, 50) },
                }
            },
            //RaonLauncher
            {
                MobileType.RaonLauncher,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(10, 50, 10, 50) },
                    { ShotType.S2, new AimPreset(10, 50, 10, 50) },
                    { ShotType.SS, new AimPreset(10, 50, 10, 50) },
                }
            },
            //Trico
            {
                MobileType.Trico,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(20, 40, 10, 50) },
                    { ShotType.S2, new AimPreset(20, 40, 10, 50) },
                    { ShotType.SS, new AimPreset(20, 40, 20, 40) },
                }
            },
            //Turtle
            {
                MobileType.Turtle,
                new Dictionary<ShotType, AimPreset>()
                {
                    { ShotType.S1, new AimPreset(25, 50, 25, 50) },
                    { ShotType.S2, new AimPreset(25, 50, 25, 50) },
                    { ShotType.SS, new AimPreset(25, 50, 25, 50) },
                }
            },
        };

        //Crosshair Presets, this variable contains all the information about all mobile crosshairs and supported angles,
        public static readonly Dictionary<MobileType, DelayPreset> DelayPresets = new Dictionary<MobileType, DelayPreset>()
        {
            { MobileType.Aduka,        new DelayPreset() { Base = 710, Shot1 = 760, Shot2 = 910, SS = 1310 } }, // Aduka     710 760 910 1310
            { MobileType.Armor,        new DelayPreset() { Base = 720, Shot1 = 770, Shot2 = 960, SS = 1320 } }, // Armor     720 770 960 1320
            { MobileType.ASate,        new DelayPreset() { Base = 680, Shot1 = 730, Shot2 = 880, SS = 1280 } }, // A.Sate    680 730 880 1280
            { MobileType.Bigfoot,      new DelayPreset() { Base = 720, Shot1 = 770, Shot2 = 920, SS = 1320 } }, // Bigfoot   720 770 920 1320
            { MobileType.Boomer,       new DelayPreset() { Base = 680, Shot1 = 730, Shot2 = 880, SS = 1280 } }, // Boomer    680 730 880 1280
            { MobileType.Dragon,       new DelayPreset() { Base = 710, Shot1 = 760, Shot2 = 910, SS = 1310 } }, // Dragon    710 760 910 1310 // Same as Aduka
            { MobileType.Grub,         new DelayPreset() { Base = 690, Shot1 = 740, Shot2 = 890, SS = 1290 } }, // Grub      690 740 890 1290
            { MobileType.Ice,          new DelayPreset() { Base = 690, Shot1 = 740, Shot2 = 890, SS = 1290 } }, // Ice       690 740 890 1290
            { MobileType.JD,           new DelayPreset() { Base = 690, Shot1 = 740, Shot2 = 830, SS = 1290 } }, // JD        690 740 830 1290
            { MobileType.JFrog,        new DelayPreset() { Base = 690, Shot1 = 740, Shot2 = 890, SS = 1290 } }, // JFrog     690 740 890 1290 // Same as Grub
            { MobileType.Kalsiddon,    new DelayPreset() { Base = 710, Shot1 = 760, Shot2 = 910, SS = 1310 } }, // Kalsiddon 710 760 910 1310 // Same as Aduka
            { MobileType.Knight,       new DelayPreset() { Base = 690, Shot1 = 740, Shot2 = 890, SS = 1290 } }, // Knight    690 740 890 1290 // Same as Ice
            { MobileType.Lightning,    new DelayPreset() { Base = 700, Shot1 = 750, Shot2 = 800, SS = 1300 } }, // Lightning 700 750 800 1300
            { MobileType.Mage,         new DelayPreset() { Base = 700, Shot1 = 750, Shot2 = 900, SS = 1300 } }, // Mage      700 750 900 1300
            { MobileType.Nak,          new DelayPreset() { Base = 710, Shot1 = 760, Shot2 = 910, SS = 1310 } }, // Nak       710 760 910 1310
            { MobileType.RaonLauncher, new DelayPreset() { Base = 700, Shot1 = 750, Shot2 = 900, SS = 1300 } }, // Raon      700 750 900 1300
            { MobileType.Trico,        new DelayPreset() { Base = 690, Shot1 = 740, Shot2 = 890, SS = 1290 } }, // Trico     690 740 890 1290
            { MobileType.Turtle,       new DelayPreset() { Base = 690, Shot1 = 740, Shot2 = 890, SS = 1290 } }, // Turtle    690 740 890 1290
        };

        //Archetypes sources: https://strategywiki.org/wiki/Gunbound
        public static readonly Dictionary<MobileType, MobileStatus> MobileStatusPresets = new Dictionary<MobileType, MobileStatus>()
        {
            { MobileType.Aduka,        new MobileStatus(30, 40, 30, 40, 50, MobileArmorArchetype.Steel,  MobileAttackArchetype.Electrical, MobileAttackArchetype.Laser,      MobileAttackArchetype.Laser) },
            { MobileType.Armor,        new MobileStatus(50, 40, 30, 30, 50, MobileArmorArchetype.Steel,  MobileAttackArchetype.Fire,       MobileAttackArchetype.Fire,       MobileAttackArchetype.Fire) },
            { MobileType.ASate,        new MobileStatus(50, 50, 20, 50, 30, MobileArmorArchetype.Shield, MobileAttackArchetype.Laser,      MobileAttackArchetype.Laser,      MobileAttackArchetype.Laser) },
            { MobileType.Assassin,     new MobileStatus(30, 30, 50, 40, 50) },
            { MobileType.Bigfoot,      new MobileStatus(50, 30, 40, 40, 30, MobileArmorArchetype.Steel,  MobileAttackArchetype.Fire,       MobileAttackArchetype.Fire,       MobileAttackArchetype.Fire) },

            { MobileType.Boomer,       new MobileStatus(50, 40, 40, 40, 30, MobileArmorArchetype.Bionic, MobileAttackArchetype.Hit,        MobileAttackArchetype.Hit,        MobileAttackArchetype.Hit) },
            { MobileType.Carrior,      new MobileStatus(30, 40, 30, 50, 30) },
            { MobileType.Dragon,       new MobileStatus(60, 30, 40, 50, 30, MobileArmorArchetype.Bionic, MobileAttackArchetype.Fire,       MobileAttackArchetype.Fire,       MobileAttackArchetype.Fire) },
            { MobileType.Frank,        new MobileStatus(50, 40, 30, 30, 50) },
            { MobileType.Grub,         new MobileStatus(40, 30, 40, 20, 40, MobileArmorArchetype.Bionic, MobileAttackArchetype.Electrical, MobileAttackArchetype.Electrical, MobileAttackArchetype.Fire) },

            { MobileType.Ice,          new MobileStatus(40, 30, 40, 40, 40, MobileArmorArchetype.Bionic, MobileAttackArchetype.Hit,        MobileAttackArchetype.Hit,        MobileAttackArchetype.Hit) },
            { MobileType.JD,           new MobileStatus(40, 50, 20, 50, 40, MobileArmorArchetype.Shield, MobileAttackArchetype.Electrical, MobileAttackArchetype.Electrical, MobileAttackArchetype.Electrical) },
            { MobileType.JFrog,        new MobileStatus(50, 30, 40, 40, 40, MobileArmorArchetype.Bionic, MobileAttackArchetype.Laser,      MobileAttackArchetype.Laser,      MobileAttackArchetype.Laser) },
            { MobileType.Kalsiddon,    new MobileStatus(60, 30, 40, 50, 30, MobileArmorArchetype.Steel,  MobileAttackArchetype.Fire,       MobileAttackArchetype.Fire,       MobileAttackArchetype.Fire) },
            { MobileType.Knight,       new MobileStatus(60, 40, 30, 40, 40, MobileArmorArchetype.Steel,  MobileAttackArchetype.Laser,      MobileAttackArchetype.Laser,      MobileAttackArchetype.Laser) },

            { MobileType.Lightning,    new MobileStatus(40, 50, 20, 30, 30, MobileArmorArchetype.Shield, MobileAttackArchetype.Electrical, MobileAttackArchetype.Electrical, MobileAttackArchetype.Electrical) },
            { MobileType.Mage,         new MobileStatus(40, 50, 50, 40, 40, MobileArmorArchetype.Shield, MobileAttackArchetype.Laser,      MobileAttackArchetype.Laser,      MobileAttackArchetype.Laser) },
            { MobileType.Maya,         new MobileStatus(40, 40, 30, 40, 40, MobileArmorArchetype.Shield, MobileAttackArchetype.Hit,        MobileAttackArchetype.Hit,        MobileAttackArchetype.Hit) },
            { MobileType.Nak,          new MobileStatus(50, 40, 30, 40, 40, MobileArmorArchetype.Steel,  MobileAttackArchetype.Hit,        MobileAttackArchetype.Hit,        MobileAttackArchetype.Hit) },
            { MobileType.Phoenix,      new MobileStatus(30, 40, 30, 50, 30) },

            { MobileType.Princess,     new MobileStatus(30, 50, 20, 40, 50) },
            { MobileType.Random,       new MobileStatus(30, 30, 30, 30, 30) },
            { MobileType.RaonLauncher, new MobileStatus(30, 40, 30, 40, 60, MobileArmorArchetype.Steel,  MobileAttackArchetype.Laser,      MobileAttackArchetype.Laser,      MobileAttackArchetype.Laser) },
            { MobileType.ShadowWalker, new MobileStatus(40, 20, 50, 40, 30) },
            { MobileType.Tiburon,      new MobileStatus(40, 40, 30, 50, 40) },

            { MobileType.Trico,        new MobileStatus(50, 30, 40, 40, 30, MobileArmorArchetype.Bionic, MobileAttackArchetype.Fire,       MobileAttackArchetype.Fire,       MobileAttackArchetype.Fire) },
            { MobileType.Turtle,       new MobileStatus(40, 30, 40, 40, 40, MobileArmorArchetype.Bionic, MobileAttackArchetype.Hit,        MobileAttackArchetype.Hit,        MobileAttackArchetype.Hit) },
            { MobileType.Wolf,         new MobileStatus(30, 30, 40, 40, 60) },
        };

        public static readonly MobileStatus BestMobileStatus
            = new MobileStatus(
                MobileStatusPresets.Max((x) => x.Value.Attack),
                MobileStatusPresets.Max((x) => x.Value.Defence),
                MobileStatusPresets.Max((x) => x.Value.Delay),
                MobileStatusPresets.Max((x) => x.Value.Energy),
                MobileStatusPresets.Max((x) => x.Value.Mobility));

        public static MobileMetadata BuildMobileMetadata(Player player, MobileType mobileType)
        {
            switch (mobileType)
            {
                case MobileType.Aduka:        return new MobileMetadata(player, mobileType, 1000,  0, 35,   0,  0);
                case MobileType.Armor:        return new MobileMetadata(player, mobileType, 1000,  0, 35,   0,  0);
                case MobileType.ASate:        return new MobileMetadata(player, mobileType,  760,  0, 35, 220, 20);
                case MobileType.Bigfoot:      return new MobileMetadata(player, mobileType, 1100,  0, 35,   0,  0);
                case MobileType.Boomer:       return new MobileMetadata(player, mobileType, 1000, 10, 70,   0,  0);
                case MobileType.Dragon:       return new MobileMetadata(player, mobileType, 1000, 10, 70,   0,  0);
                case MobileType.Grub:         return new MobileMetadata(player, mobileType, 1000, 10, 70,   0,  0);
                case MobileType.Ice:          return new MobileMetadata(player, mobileType, 1200, 10, 70,   0,  0);
                case MobileType.JD:           return new MobileMetadata(player, mobileType,  750,  0, 35, 250, 20);
                case MobileType.JFrog:        return new MobileMetadata(player, mobileType,  930, 10, 70,   0,  0);
                case MobileType.Kalsiddon:    return new MobileMetadata(player, mobileType, 1100,  0, 35,   0,  0);
                case MobileType.Knight:       return new MobileMetadata(player, mobileType, 1000,  0, 35,   0,  0);
                case MobileType.Lightning:    return new MobileMetadata(player, mobileType,  760,  0, 35, 220, 20);
                case MobileType.Mage:         return new MobileMetadata(player, mobileType,  760,  0, 35, 220, 20);
                case MobileType.Nak:          return new MobileMetadata(player, mobileType, 1100,  0, 35,   0,  0);
                case MobileType.RaonLauncher: return new MobileMetadata(player, mobileType, 1000,  0, 35,   0,  0);
                case MobileType.Trico:        return new MobileMetadata(player, mobileType, 1100, 10, 70,   0,  0);
                case MobileType.Turtle:       return new MobileMetadata(player, mobileType,  950, 10, 70,   0,  0);
            }

            return null;
        }

        //Mobile Information
        [JsonProperty("CH")] public int CurrentHealth;

        [JsonIgnore] public int BaseHealth;
        [JsonIgnore] public int HealthRegeneration;
        [JsonIgnore] public int HealthRegenerationProtection;

        [JsonProperty("CS")] public int CurrentShield;

        [JsonIgnore] public int BaseShield;
        [JsonIgnore] public int ShieldRegeneration;

        [JsonIgnore] public MobileType MobileType;
        [JsonIgnore] public Dictionary<ShotType, AimPreset> MobileAimPreset => AimPresets[MobileType];

        [JsonIgnore] public DelayPreset DelayPreset => DelayPresets[MobileType];

        private MobileMetadata(Player player, MobileType mobileType, int baseHealth, int healthRegeneration, int healthRegenerationProtection, int baseShield, int shieldRegeneration)
        {
            MobileType = mobileType;
            CurrentHealth = BaseHealth = (int)(baseHealth * (1 + player.Health / 100f));
            HealthRegeneration = healthRegeneration;
            HealthRegenerationProtection = healthRegenerationProtection;

            CurrentShield = BaseShield = baseShield;
            ShieldRegeneration = shieldRegeneration;
        }

        public MobileMetadata() { }

        public int GetDelay(Player player, ShotType shotType)
        {
            int delay;
            switch (shotType)
            {
                case ShotType.S1: delay = DelayPreset.Shot1; break;
                case ShotType.S2: delay = DelayPreset.Shot2; break;
                case ShotType.SS: delay = DelayPreset.SS; break;
                default: return 0;
            };

            return (int)(delay * (1 - player.AttackDelay / 100f));
        }
    }
}
