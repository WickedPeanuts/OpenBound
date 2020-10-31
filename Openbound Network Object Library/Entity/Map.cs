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
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Extension;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound_Network_Object_Library.Entity
{
    public enum GameMap
    {
        Random,
        Adiumroot, CozyTower, Dragon,
        DummySlope, Dungeon, FourHeads,
        ILoveYou, Metamine, Metropolis,
        MiramoTown, Nirvana, PlanetLond,
        SeaOfHero, Stardust,
    }

    public enum GameMapType
    {
        A, B, C, D
    }

    public class Map
    {
        /*
         * Since there is no information source for weather, all maps commented with a 'No information available' had custom parameters set.
         * All maps has a custom weather chance for Weakness, Mirror and Random Weather
         */

        private static List<Map> MapList = new List<Map>()
        {
            //Donee
            new Map( 0, "Random",      GameMapType.A, GameMap.Random,     new int[]{ -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, new int[,]{ }),

            //Done
            new Map( 1, "Adiumroot",   GameMapType.A, GameMap.Adiumroot,  new int[]{  1,  3,  1,  0,  1,  1,  1,  1,  2,  1 }, new int[,] { { 0786, 0956 }, { 0581, 0964 }, { 0396, 0958 }, { 0206, 0905 }, { 0068, 0852 }, { 0875, 0950 }, { 0965, 0958 }, { 1195, 0964 }, { 1349, 0954 }, { 1575, 0891 }, { 1719, 0822 } },                                                                                                                                 42, new float[]{ 24.5f, 24.5f }),
            new Map( 2, "Adiumroot",   GameMapType.B, GameMap.Adiumroot,  new int[]{  2,  1,  2,  1,  1,  2,  0,  3,  1,  2 }, new int[,] { { 0846, 0552 }, { 1016, 0611 }, { 0976, 0662 }, { 0782, 0728 }, { 0846, 0770 }, { 1016, 0826 }, { 0976, 0877 }, { 0782, 1153 }, { 0846, 0985 }, { 1016, 1042 }, { 0976, 1099 }, { 0782, 1164 }, { 0846, 1202 }, { 1016, 1262 }, { 0976, 1317 }, { 0782, 1380 }, { 0846, 1424 }, { 1016, 1483 }, { 0917, 1555 } }, 42, new float[]{ 24.5f, 24.5f }), //No info available
            new Map( 3, "Adiumroot",   GameMapType.C, GameMap.Adiumroot,  new int[]{  2,  2,  1,  3,  1,  0,  0,  1,  0,  2 }, new int[,] { { 0172, 1132 }, { 0376, 1163 }, { 0531, 1140 }, { 0724, 1117 }, { 0866, 1094 }, { 0993, 1095 }, { 1137, 1115 }, { 1271, 1167 }, { 1433, 1137 }, { 1588, 1123 } },                                                                                                                                                 42, new float[]{ 24.5f, 24.5f }),
        
            //Done
            new Map( 4, "Cozy Tower",  GameMapType.A, GameMap.CozyTower,  new int[]{  2,  1,  1,  1,  1,  2,  0,  1,  1,  1 }, new int[,] { { 0140, 1381 }, { 0314, 1382 }, { 0426, 1437 }, { 0571, 1438 }, { 0716, 1495 }, { 0903, 1485 }, { 1075, 1495 }, { 1234, 1437 }, { 1381, 1436 }, { 1500, 1381 }, { 1676, 1381 } },                                                                                                                                 40, new float[]{   29f, 25.5f }),
            new Map( 5, "Cozy Tower",  GameMapType.B, GameMap.CozyTower,  new int[]{  2,  2,  1,  1,  1,  1,  0,  1,  1,  1 }, new int[,] { { 0206, 1264 }, { 0412, 1305 }, { 0615, 1403 }, { 0808, 1368 }, { 1000, 1404 }, { 1197, 1295 }, { 1398, 1342 }, { 1601, 1231 } },                                                                                                                                                                                 40, new float[]{   29f, 25.5f }),

            new Map( 6, "Dragon",      GameMapType.A, GameMap.Dragon,     new int[]{  3,  0,  2,  1,  1,  1,  1,  0,  0,  1 }, new int[,] { { 0136, 1565 }, { 0389, 1556 }, { 0576, 1692 }, { 0869, 1791 }, { 1180, 1785 }, { 1465, 1690 }, { 1643, 1558 }, { 1850, 1576 } },                                                                                                                                                                                 50, new float[]{ 27.5f, 28f }),
            new Map( 7, "Dragon",      GameMapType.B, GameMap.Dragon,     new int[]{  1,  1,  1,  1,  1,  3,  0,  1,  1,  0 }, new int[,] { { 0132, 0998 }, { 0423, 0727 }, { 0765, 0603 }, { 0586, 1014 }, { 0906, 1053 }, { 1217, 1015 }, { 1669, 0982 }, { 1052, 0637 }, { 1384, 0739 } },                                                                                                                                                                 50, new float[]{ 27.5f, 28f }), //No info available
            new Map( 8, "Dragon",      GameMapType.C, GameMap.Dragon,     new int[]{  3,  0,  2,  1,  1,  1,  1,  0,  0,  1 }, new int[,] { { 0085, 0928 }, { 0312, 1041 }, { 0170, 1245 }, { 0587, 1278 }, { 0892, 1199 }, { 1220, 1279 }, { 1645, 1236 }, { 1491, 1042 }, { 1761, 0920 } },                                                                                                                                                                 50, new float[]{ 27.5f, 28f }), //No info available

            new Map( 9, "Dummy Slope", GameMapType.A, GameMap.DummySlope, new int[]{  2,  2,  0,  1,  0,  1,  1,  1,  1,  2 }, new int[,] { { 0147, 1068 }, { 0307, 1126 }, { 0514, 1174 }, { 0686, 1134 }, { 0905, 1128 }, { 1095, 1157 }, { 1282, 1129 }, { 1449, 1035 } },                                                                                                                                                                                 44, new float[]{ 27.5f, 27.5f }),
            new Map(10, "Dummy Slope", GameMapType.B, GameMap.DummySlope, new int[]{  1,  1,  3,  2,  1,  1,  1,  1,  2,  1 }, new int[,] { { 0268, 1091 }, { 0312, 0974 }, { 0439, 1185 }, { 0476, 1083 }, { 0660, 1203 }, { 0713, 1081 }, { 1324, 1090 }, { 1276, 0977 }, { 1171, 1188 }, { 1120, 1083 }, { 0935, 1203 }, { 0883, 1084 } },                                                                                                                 44, new float[]{ 27.5f, 27.5f }), //No info available
            new Map(11, "Dummy Slope", GameMapType.C, GameMap.DummySlope, new int[]{  2,  0,  2,  0,  0,  2,  0,  2,  2,  1 }, new int[,] { { 0394, 1130 }, { 0450, 1331 }, { 0632, 1268 }, { 0811, 1334 }, { 0916, 1178 }, { 1037, 1313 }, { 1194, 1270 }, { 1370, 1334 }, { 1460, 1124 } },                                                                                                                                                                 44, new float[]{ 27.5f, 27.5f }), //No info available

            new Map(12, "Dungeon",     GameMapType.A, GameMap.Dungeon,    new int[]{  0,  2,  1,  2,  1,  1,  1,  1,  1,  1 }, new int[,] { { 0178, 1111 }, { 0480, 1154 }, { 0595, 1229 }, { 0769, 1230 }, { 0911, 1202 }, { 1070, 1228 }, { 1236, 1245 }, { 1345, 1153 }, { 1634, 1115 } },                                                                                                                                                                 62, new float[]{ 22.5f, 22.5f }), //No info available
            
            new Map(13, "Four Heads",  GameMapType.A, GameMap.FourHeads,  new int[]{  1,  3,  0,  2,  1,  1,  0,  0,  0,  1 }, new int[,] { { 0074, 1088 }, { 0288, 1056 }, { 0460, 1006 }, { 0639, 0983 }, { 0873, 0963 }, { 1078, 1039 }, { 1242, 0905 }, { 1372, 0996 }, { 1467, 1102 } },                                                                                                                                                                 40, new float[]{ 24.5f, 21f }), //No info available

            new Map(14, "I Love You",  GameMapType.A, GameMap.ILoveYou,   new int[]{  1,  0,  3,  1,  2,  1,  0,  0,  0,  2 }, new int[,] { { 0496, 0777 }, { 1305, 0777 }, { 0296, 0954 }, { 1507, 0955 }, { 0422, 1159 }, { 1378, 1159 }, { 0428, 1345 }, { 1381, 1345 }, { 0643, 1358 }, { 1154, 1359 }, { 0904, 1440 }, { 0668, 1507 }, { 1135, 1506 } },                                                                                                 40, new float[]{ 24.5f, 21f }), //No info available
            new Map(15, "I Love You",  GameMapType.B, GameMap.ILoveYou,   new int[]{  2,  1,  0,  3,  1,  1,  0,  2,  3,  0 }, new int[,] { { 0464, 0650 }, { 0705, 0641 }, { 1091, 0640 }, { 1327, 0651 }, { 0411, 0899 }, { 1381, 0899 }, { 0803, 1021 }, { 0994, 1021 }, { 0897, 1241 }, { 0415, 1322 }, { 0653, 1322 }, { 1136, 1321 }, { 1382, 1321 } },                                                                                                 40, new float[]{ 24.5f, 21f }), //No info available

            //Done
            new Map(16, "Meta Mine",   GameMapType.A, GameMap.Metamine,   new int[]{  0,  2,  0,  3,  0,  1,  2,  1,  1,  1 }, new int[,] { { 0827, 1004 }, { 0973, 1004 }, { 0738, 0987 }, { 1062, 0987 }, { 0600, 0965 }, { 1200, 0965 }, { 0438, 0988 }, { 1362, 0988 }, { 0236, 0944 }, { 1562, 0944 } },                                                                                                                                                 42, new float[]{   34f, 34f }),
            new Map(17, "Meta Mine",   GameMapType.B, GameMap.Metamine,   new int[]{  2,  3,  0,  0,  1,  1,  0,  2,  1,  1 }, new int[,] { { 0175, 0816 }, { 0669, 0860 }, { 1115, 0852 }, { 1623, 0816 }, { 0303, 1285 }, { 0483, 1279 }, { 0835, 1293 }, { 1005, 1303 }, { 1303, 1295 }, { 1531, 1303 } },                                                                                                                                                 42, new float[]{   34f, 34f }),

            new Map(18, "Metropolis",  GameMapType.A, GameMap.Metropolis, new int[]{  1,  1,  3,  1,  2,  1,  0,  1,  2,  2 }, new int[,] { { 0126, 1119 }, { 0389, 1202 }, { 0548, 1534 }, { 0581, 1353 }, { 0738, 1439 }, { 1061, 1440 }, { 1217, 1353 }, { 1251, 1534 }, { 1404, 1203 }, { 1672, 1119 } },                                                                                                                                                 54, new float[]{   23f, 22.5f }),
            new Map(19, "Metropolis",  GameMapType.B, GameMap.Metropolis, new int[]{  1,  0,  4,  1,  1,  0,  0,  2,  0,  1 }, new int[,] { { 0327, 1150 }, { 0512, 1267 }, { 0518, 1036 }, { 0690, 1134 }, { 0880, 1038 }, { 0883, 1252 }, { 1057, 1154 }, { 1221, 1037 }, { 1235, 1265 }, { 1417, 1133 } },                                                                                                                                                 54, new float[]{   23f, 22.5f }), //No info available

            new Map(20, "Miramo Town", GameMapType.A, GameMap.MiramoTown, new int[]{  2,  1,  2,  1,  0,  2,  0,  0,  0,  1 }, new int[,] { { 0148, 1439 }, { 0325, 1410 }, { 0448, 1374 }, { 0596, 1392 }, { 0743, 1341 }, { 0883, 1396 }, { 1041, 1354 }, { 1192, 1409 }, { 1363, 1374 }, { 1515, 1394 }, { 1673, 1400 } },                                                                                                                                 60, new float[]{   24f, 24f }),
            new Map(21, "Miramo Town", GameMapType.B, GameMap.MiramoTown, new int[]{  2,  1,  3,  1,  0,  0,  0,  1,  0,  1 }, new int[,] { { 0338, 1538 }, { 0524, 1532 }, { 0693, 1500 }, { 0829, 1475 }, { 0976, 1472 }, { 1158, 1496 }, { 1314, 1490 }, { 1508, 1523 }, { 0493, 1737 }, { 1286, 1702 } },                                                                                                                                                 60, new float[]{   24f, 24f }), //No info available
            new Map(22, "Miramo Town", GameMapType.C, GameMap.MiramoTown, new int[]{  1,  0,  1,  1,  1,  1,  1,  1,  2,  2 }, new int[,] { { 0272, 1452 }, { 0479, 1395 }, { 0673, 1397 }, { 0836, 1406 }, { 0990, 1410 }, { 1116, 1460 }, { 1235, 1369 }, { 1389, 1422 }, { 1504, 1494 } },                                                                                                                                                                 60, new float[]{   24f, 24f }), //No info available

            new Map(23, "Nirvana",     GameMapType.A, GameMap.Nirvana,    new int[]{  0,  1,  2,  2,  1,  1,  1,  1,  1,  0 }, new int[,] { { 0068, 1113 }, { 0309, 1165 }, { 0519, 1116 }, { 0726, 1117 }, { 0927, 1116 }, { 1122, 1170 }, { 1309, 1064 }, { 1549, 1074 } },                                                                                                                                                                                 64, new float[]{ 24.5f, 30.5f }),
            new Map(24, "Nirvana",     GameMapType.B, GameMap.Nirvana,    new int[]{  0,  0,  4,  1,  1,  0,  3,  0,  0,  0 }, new int[,] { { 0559, 0689 }, { 0793, 0663 }, { 0964, 0709 }, { 1177, 0664 }, { 0684, 0817 }, { 0966, 0822 }, { 1183, 0831 }, { 0581, 0984 }, { 0829, 0935 }, { 1022, 1030 }, { 1142, 1102 }, { 0510, 1115 }, { 0732, 1124 }, { 0943, 1175 }, { 0652, 1300 }, { 0875, 1337 }, { 1097, 1293 } },                                 64, new float[]{ 24.5f, 30.5f }),

            new Map(25, "Planet Lond", GameMapType.A, GameMap.PlanetLond, new int[]{  0,  1,  1,  2,  3,  0,  1,  1,  0,  1 }, new int[,] { { 0346, 0960 }, { 0538, 0854 }, { 0903, 0908 }, { 1263, 0855 }, { 1451, 0960 }, { 0366, 1212 }, { 0579, 1099 }, { 1219, 1102 }, { 1435, 1211 }, { 0658, 1328 }, { 1143, 1327 } },                                                                                                                                 44, new float[]{   29f, 25.5f }), //No info available
            new Map(26, "Planet Lond", GameMapType.B, GameMap.PlanetLond, new int[]{  0,  1,  3,  2,  2,  0,  0,  1,  1,  0 }, new int[,] { { 0288, 1086 }, { 0599, 1158 }, { 0813, 1161 }, { 1022, 1155 }, { 1192, 1175 }, { 1490, 1085 }, { 0614, 0842 }, { 0868, 0798 }, { 0941, 0802 }, { 1188, 0842 } },                                                                                                                                                 44, new float[]{   29f, 25.5f }), //No info available

            new Map(27, "Sea of Hero", GameMapType.A, GameMap.SeaOfHero,  new int[]{  2,  0,  2,  1,  0,  3,  0,  0,  0,  2 }, new int[,] { { 0059, 0728 }, { 0252, 0805 }, { 0486, 0962 }, { 0708, 0920 }, { 0897, 0920 }, { 1068, 0972 }, { 1359, 0804 }, { 1557, 0704 } },                                                                                                                                                                                 44, new float[]{ 27.5f, 27.5f }),
            new Map(28, "Sea of Hero", GameMapType.B, GameMap.SeaOfHero,  new int[]{  4,  0,  2,  1,  0,  1,  0,  0,  1,  0 }, new int[,] { { 0101, 1112 }, { 0254, 1048 }, { 0412, 0961 }, { 0630, 1007 }, { 0794, 1049 }, { 0952, 1006 }, { 1150, 0958 }, { 1335, 1049 }, { 1488, 1112 } },                                                                                                                                                                 44, new float[]{ 27.5f, 27.5f }), //No info available
            new Map(29, "Sea of Hero", GameMapType.C, GameMap.SeaOfHero,  new int[]{  2,  3,  1,  1,  1,  1,  2,  1,  3,  1 }, new int[,] { { 0043, 0933 }, { 0275, 1010 }, { 0460, 0873 }, { 0692, 0863 }, { 0852, 1029 }, { 1134, 1061 }, { 1281, 0781 }, { 1522, 0732 } },                                                                                                                                                                                 44, new float[]{ 27.5f, 27.5f }),

            new Map(30, "Stardust",    GameMapType.A, GameMap.Stardust,   new int[]{  1,  2,  2,  1,  0,  0,  1,  1,  1,  1 }, new int[,] { { 0323, 1095 }, { 0452, 1011 }, { 0592, 0963 }, { 0732, 0938 }, { 0903, 0921 }, { 1073, 0940 }, { 1213, 0965 }, { 1351, 1010 }, { 1485, 1094 }, { 0557, 1104 }, { 0717, 1078 }, { 0903, 1104 }, { 1094, 1078 }, { 1256, 1103 } },                                                                                 64, new float[]{  29f, 30f }),
            new Map(31, "Stardust",    GameMapType.B, GameMap.Stardust,   new int[]{  0,  0,  0,  3,  2,  3,  2,  0,  1,  1 }, new int[,] { { 0164, 1520 }, { 0382, 1527 }, { 0650, 1590 }, { 0826, 1574 }, { 1069, 1604 }, { 1197, 1603 }, { 1460, 1535 }, { 1638, 1523 } },                                                                                                                                                                                 64, new float[]{  29f, 30f }), //No info available
        };

        public int ID;
        [JsonProperty("NA")] public string Name;
        [JsonProperty("GMT")] public GameMapType GameMapType;
        [JsonProperty("GM")] public GameMap GameMap;

        [JsonIgnore] public float[] GroundParticlePivot;

        [JsonIgnore] public int GroundParticleNumberOfFrames;

        [JsonIgnore] public List<int[]> SpawnPoints;

        #region Weather Effects
        private int[] weatherPreset;
        [JsonProperty("WP")] public int[] WeatherPreset
        {
            get
            {
                if (weatherPreset == null)
                {
                    weatherPreset = MapList.Find((x) => x.GameMap == GameMap && x.GameMapType == GameMapType).WeatherPreset;
                }

                return weatherPreset;
            }
        }

        [JsonIgnore] public int Force { get => WeatherPreset[0]; }
        [JsonIgnore] public int Tornado { get => WeatherPreset[1]; }
        [JsonIgnore] public int Electricity { get => WeatherPreset[2]; }
        [JsonIgnore] public int Wind { get => WeatherPreset[3]; }
        [JsonIgnore] public int Weakness { get => WeatherPreset[4]; }
        [JsonIgnore] public int Protection { get => WeatherPreset[5]; }
        [JsonIgnore] public int Ignorance { get => WeatherPreset[6]; }
        [JsonIgnore] public int Thor { get => WeatherPreset[7]; }
        [JsonIgnore] public int Mirror { get => WeatherPreset[8]; }
        [JsonIgnore] public int Random { get => WeatherPreset[9]; }
        #endregion

        public Map() { }

        private Map(int id, string name, GameMapType gameMapType, GameMap gameMap, int[] weatherPreset, int[,] spawnPoints, int groundParticleNumberOfFrames = 0, float[] groundParticlePivot = null)
        {
            ID = id;
            Name = name;
            GameMapType = gameMapType;
            GameMap = gameMap;

            SpawnPoints = spawnPoints?.ToMatrix().ToList();

            this.weatherPreset = weatherPreset;
            GroundParticleNumberOfFrames = groundParticleNumberOfFrames;
            GroundParticlePivot = groundParticlePivot;
        }

        public static Map GetMap(GameMapType gameMapType, GameMap gameMap)
        {
            return MapList.First((x) => x.GameMapType == gameMapType && x.GameMap == gameMap);
        }

        public static Map GetMap(int mapID)
        {
            return MapList.First((x) => x.ID == mapID);
        }

        public static Map GetRandomMap(GameMapType gameMapType)
        {
            List<Map> mapList = MapList.Where((x) => x.GameMapType == gameMapType && x.SpawnPoints != null && x.SpawnPoints.Count > 1).ToList();
            return mapList[NetworkObjectParameters.Random.Next(mapList.Count)];
        }

        public static Map GetPreviousMap(Map map)
        {
            int nextID = map.ID;

            while (true)
            {
                nextID--;

                if (nextID < 0)
                    nextID = MapList.Last().ID;

                Map nextMap = MapList.Find((x) => x.ID == nextID);

                if (nextMap != null && nextMap.SpawnPoints != null)
                    return nextMap;
            }
        }

        public static Map GetNextMap(Map map)
        {
            int nextID = map.ID;

            while (true)
            {
                nextID++;

                if (nextID > MapList.Last().ID)
                    nextID = 0;

                Map nextMap = MapList.Find((x) => x.ID == nextID);

                if (nextMap != null && nextMap.SpawnPoints != null)
                    return nextMap;
            }
        }
    }
}
