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

using Microsoft.EntityFrameworkCore;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.Extension;
using OpenBound_Network_Object_Library.Common;

namespace OpenBound_Network_Object_Library.Database.Context
{
    public class OpenBoundDatabaseContext : DbContext
    {
        private readonly string _connectionString;
        public OpenBoundDatabaseContext() : base()
        {
            _connectionString = $"Data Source={NetworkObjectParameters.DatabaseAddress};Initial Catalog={NetworkObjectParameters.DatabaseName};Persist Security Info=True;User ID={NetworkObjectParameters.DatabaseLogin};Password={NetworkObjectParameters.DatabasePassword};PersistSecurityInfo=True";
            //_connectionString = "Data Source=localhost,1433;User ID=sa;Password=my-secret-pw-xD;Database=Openbound;Initial Catalog=Openbound;Connect Timeout=10;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Seeder seeder = new Seeder();

            modelBuilder.RemovePluralizingTableNameConvention();

            modelBuilder.Entity<Player>().HasIndex(x => new { x.Nickname, x.Email }).IsUnique(true);

            //Avatar composite key
            modelBuilder.Entity<AvatarMetadata>().HasKey(x => new { x.ID, x.Gender, x.AvatarCategory });

            //Relationship Player <-> Avatar
            modelBuilder.Entity<PlayerAvatarMetadata>()
                .HasKey(x => new { x.Player_ID, x.AvatarMetadata_ID, x.AvatarMetadata_Gender, x.AvatarMetadata_AvatarCategory });

            modelBuilder.Entity<PlayerAvatarMetadata>()
                .HasOne(sc => sc.Player)
                .WithMany(s => s.PlayerAvatarMetadataList)
                .HasForeignKey(sc => sc.Player_ID);

            modelBuilder.Entity<PlayerAvatarMetadata>()
                .HasOne(sc => sc.AvatarMetadata)
                .WithMany(s => s.PlayerAvatarMetadata)
                .HasForeignKey(sc => new { sc.AvatarMetadata_ID, sc.AvatarMetadata_Gender, sc.AvatarMetadata_AvatarCategory });

            //Relationship Player <-> Player (Friend List)
            modelBuilder.Entity<PlayerRelationship>()
                .HasOne(x => x.Player)
                .WithMany(c => c.PlayerRelationshipList)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Guild>().HasIndex(x => x.Tag).IsUnique();   

            // Seeders
            seeder.SeedAvatarMetadata(modelBuilder);
            seeder.SeedPlayer(modelBuilder);
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Guild> Guilds { get; set; }
        public DbSet<AvatarMetadata> AvatarMetadata { get; set; }
        public DbSet<PlayerAvatarMetadata> PlayerAvatarMetadata { get; set; }
        public DbSet<PlayerRelationship> PlayerFriendships { get; set; }
        public DbSet<DonationTransaction> DonationTransactions { get; set; }
    }
}
