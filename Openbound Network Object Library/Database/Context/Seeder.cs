using Microsoft.EntityFrameworkCore;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenBound_Network_Object_Library.Database.Context
{
    public class Seeder
    {
        public void SeedPlayer(ModelBuilder modelBuilder)
        {
            string defaultPassword = "$2y$12$ZcVa8MvpkEUx5LlQ5BNjNOezed07s8b71I5OcYq5vf1q52tjASjki";

            List<Player> pList = new List<Player>()
            {
                new Player { ID = 1, Nickname = "Winged", Email = "dev00@dev.com", Password = defaultPassword, Gold = 30000, Cash = 30000, Token = 30000 },
                new Player { ID = 2, Nickname = "Wicked", Email = "dev01@dev.com", Password = defaultPassword, Gold = 30000, Cash = 30000, Token = 30000 },
                new Player { ID = 3, Nickname = "Willow", Email = "dev02@dev.com", Password = defaultPassword, Gold = 30000, Cash = 30000, Token = 30000 },
                new Player { ID = 4, Nickname = "Vinny", Email = "dev03@dev.com", Password = defaultPassword, Gold = 30000, Cash = 30000, Token = 30000 },
                new Player { ID = 5, Nickname = "Test", Email = "test0@dev.com", Password = defaultPassword, Gold = 30000, Cash = 30000, Token = 30000 }
            };

            List<PlayerAvatarMetadata> pAM = new List<PlayerAvatarMetadata>();

            for (int i = 0; i < pList.Count; i++) {
                pAM.AddRange(new List<PlayerAvatarMetadata>() {
                    new PlayerAvatarMetadata(pList[i], new AvatarMetadata() { ID = 0, AvatarCategory = AvatarCategory.Hat,     Gender = pList[i].Gender }, PaymentMethod.Free),
                    new PlayerAvatarMetadata(pList[i], new AvatarMetadata() { ID = 0, AvatarCategory = AvatarCategory.Body,    Gender = pList[i].Gender }, PaymentMethod.Free),
                    new PlayerAvatarMetadata(pList[i], new AvatarMetadata() { ID = 0, AvatarCategory = AvatarCategory.Extra,   Gender = Gender.Unissex }, PaymentMethod.Free),
                    new PlayerAvatarMetadata(pList[i], new AvatarMetadata() { ID = 0, AvatarCategory = AvatarCategory.Flag,    Gender = Gender.Unissex }, PaymentMethod.Free),
                    new PlayerAvatarMetadata(pList[i], new AvatarMetadata() { ID = 0, AvatarCategory = AvatarCategory.Goggles, Gender = Gender.Unissex }, PaymentMethod.Free),
                    new PlayerAvatarMetadata(pList[i], new AvatarMetadata() { ID = 0, AvatarCategory = AvatarCategory.Misc,    Gender = Gender.Unissex }, PaymentMethod.Free),
                    new PlayerAvatarMetadata(pList[i], new AvatarMetadata() { ID = 0, AvatarCategory = AvatarCategory.Pet,     Gender = Gender.Unissex }, PaymentMethod.Free),
                    new PlayerAvatarMetadata(pList[i], new AvatarMetadata() { ID = 0, AvatarCategory = AvatarCategory.ExItem,  Gender = Gender.Unissex }, PaymentMethod.Free),
                });
            }

            modelBuilder.Entity<Player>().HasData(pList);
            modelBuilder.Entity<PlayerAvatarMetadata>().HasData(pAM);
        }

        public void SeedAvatarMetadata(ModelBuilder modelBuilder)
        {
            try
            {
                List<AvatarMetadata> list = ObjectWrapper.DeserializeCommentedJSONFile<List<AvatarMetadata>>(
                $"{Directory.GetCurrentDirectory()}/DatabaseSeed/AvatarMetadata.json");

                modelBuilder.Entity<AvatarMetadata>().HasData(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        
        }
    }
}
