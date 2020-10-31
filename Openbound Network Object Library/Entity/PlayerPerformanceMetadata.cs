using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBound_Network_Object_Library.Entity
{
    public class PlayerPerformanceMetadata
    {
        public uint DirectHitCounter;
        public uint ShotCounter;
        public uint FriendlyFireCounter;

        public uint HighAngleShotCounter;

        public int TotalEnemyDamageDealt;
        public int TotalAllyDamageDealt;

        public uint EnemyKillCount;
        public uint AllyKillCount;

        public int GoldAmount;
        public int ExpAmount;

        public PlayerPerformanceMetadata() { }
    }
}
