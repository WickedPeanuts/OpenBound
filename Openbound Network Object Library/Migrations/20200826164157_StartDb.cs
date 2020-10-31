using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenBound_Network_Object_Library.Migrations
{
    public partial class StartDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvatarMetadata",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    AvatarCategory = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    GoldPrice = table.Column<int>(nullable: false),
                    CashPrice = table.Column<int>(nullable: false),
                    PivotX = table.Column<float>(nullable: false),
                    PivotY = table.Column<float>(nullable: false),
                    FrameDimensionX = table.Column<int>(nullable: false),
                    FrameDimensionY = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvatarMetadata", x => new { x.ID, x.Gender, x.AvatarCategory });
                });

            migrationBuilder.CreateTable(
                name: "Guild",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tag = table.Column<string>(maxLength: 6, nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guild", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nickname = table.Column<string>(maxLength: 30, nullable: false),
                    Password = table.Column<string>(maxLength: 172, nullable: false),
                    Email = table.Column<string>(maxLength: 60, nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    Gold = table.Column<int>(nullable: false),
                    Cash = table.Column<int>(nullable: false),
                    Experience = table.Column<int>(nullable: false),
                    GuildId = table.Column<int>(nullable: true),
                    MatchesPlayed = table.Column<long>(nullable: false),
                    MatchesLeft = table.Column<long>(nullable: false),
                    AllyKill = table.Column<long>(nullable: false),
                    EnemyKill = table.Column<long>(nullable: false),
                    DirectHit = table.Column<long>(nullable: false),
                    FriendlyFire = table.Column<long>(nullable: false),
                    HighAngleShots = table.Column<long>(nullable: false),
                    ShotCounter = table.Column<long>(nullable: false),
                    PrimaryMobile = table.Column<int>(nullable: false),
                    SecondaryMobile = table.Column<int>(nullable: false),
                    EquippedAvatarHat = table.Column<int>(nullable: false),
                    EquippedAvatarBody = table.Column<int>(nullable: false),
                    EquippedAvatarGoggles = table.Column<int>(nullable: false),
                    EquippedAvatarFlag = table.Column<int>(nullable: false),
                    EquippedAvatarExItem = table.Column<int>(nullable: false),
                    EquippedAvatarPet = table.Column<int>(nullable: false),
                    EquippedAvatarMisc = table.Column<int>(nullable: false),
                    EquippedAvatarExtra = table.Column<int>(nullable: false),
                    Attack = table.Column<int>(nullable: false),
                    Health = table.Column<int>(nullable: false),
                    Defense = table.Column<int>(nullable: false),
                    Regeneration = table.Column<int>(nullable: false),
                    AttackDelay = table.Column<int>(nullable: false),
                    ItemDelay = table.Column<int>(nullable: false),
                    Dig = table.Column<int>(nullable: false),
                    Popularity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Player_Guild_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guild",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerAvatarMetadata",
                columns: table => new
                {
                    Player_ID = table.Column<int>(nullable: false),
                    AvatarMetadata_ID = table.Column<int>(nullable: false),
                    AvatarMetadata_Gender = table.Column<int>(nullable: false),
                    AvatarMetadata_AvatarCategory = table.Column<int>(nullable: false),
                    PaymentMethod = table.Column<int>(nullable: false),
                    BoughtAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAvatarMetadata", x => new { x.Player_ID, x.AvatarMetadata_ID });
                    table.ForeignKey(
                        name: "FK_PlayerAvatarMetadata_Player_Player_ID",
                        column: x => x.Player_ID,
                        principalTable: "Player",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerAvatarMetadata_AvatarMetadata_AvatarMetadata_ID_AvatarMetadata_Gender_AvatarMetadata_AvatarCategory",
                        columns: x => new { x.AvatarMetadata_ID, x.AvatarMetadata_Gender, x.AvatarMetadata_AvatarCategory },
                        principalTable: "AvatarMetadata",
                        principalColumns: new[] { "ID", "Gender", "AvatarCategory" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerRelationship",
                columns: table => new
                {
                    Player_A_ID = table.Column<int>(nullable: false),
                    Player_B_ID = table.Column<int>(nullable: false),
                    PlayerRelationshipStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerRelationship", x => new { x.Player_A_ID, x.Player_B_ID });
                    table.ForeignKey(
                        name: "FK_PlayerRelationship_Player_Player_A_ID",
                        column: x => x.Player_A_ID,
                        principalTable: "Player",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PlayerRelationship_Player_Player_B_ID",
                        column: x => x.Player_B_ID,
                        principalTable: "Player",
                        principalColumn: "ID");
                });

            migrationBuilder.InsertData(
                table: "AvatarMetadata",
                columns: new[] { "ID", "Gender", "AvatarCategory", "CashPrice", "Date", "FrameDimensionX", "FrameDimensionY", "GoldPrice", "Name", "PivotX", "PivotY" },
                values: new object[,]
                {
                    { 0, 0, 0, 0, new DateTime(2020, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 38, 24, 0, "Basic", 25f, 12f },
                    { 0, 2, 4, 0, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 0, "No ExItem", 0f, 0f },
                    { 1, 2, 5, 99999999, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 28, 99999999, "Red Parrot", 14f, 24f },
                    { 0, 2, 5, 0, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 0, "No Pets", 0f, 0f },
                    { 1, 2, 6, 100, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 90, 72, 100, "Bats", 45f, 36f },
                    { 0, 2, 6, 0, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 0, "No Misc", 0f, 0f },
                    { 1, 2, 7, 100, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 88, 72, 100, "Woods", 44f, 36f },
                    { 0, 2, 7, 0, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 0, "No Extra", 0f, 0f },
                    { 1, 2, 3, 100, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 40, 34, 100, "Blue Flag", 11f, 26f },
                    { 0, 2, 2, 0, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 0, "No Goggles", 0f, 0f },
                    { 0, 1, 1, 0, new DateTime(2020, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 34, 28, 0, "Basic", 22f, 13f },
                    { 0, 2, 3, 0, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 0, "No Flag", 0f, 0f },
                    { 1, 0, 2, 100, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 26, 13, 100, "Pilot Goggles", 18f, 5f },
                    { 3, 0, 1, 100, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 34, 34, 100, "Punk", 15f, 15f },
                    { 2, 0, 1, 100, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 36, 34, 100, "Miner", 16f, 15f },
                    { 1, 0, 1, 100, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 34, 34, 100, "Space Marine", 15f, 15f },
                    { 0, 0, 1, 0, new DateTime(2020, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 44, 28, 0, "Basic", 22f, 12f },
                    { 138, 0, 0, 99999999, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 39, 48, 99999999, "Kappa", 18f, 27f },
                    { 3, 0, 0, 100, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 38, 39, 100, "Punk", 23f, 22f },
                    { 2, 0, 0, 100, new DateTime(2020, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 136, 78, 100, "Miner", 95f, 52f },
                    { 1, 0, 0, 100, new DateTime(2020, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 32, 28, 100, "Space Marine", 18f, 14f },
                    { 0, 1, 0, 0, new DateTime(2020, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 41, 33, 0, "Basic", 28f, 14f }
                });

            migrationBuilder.InsertData(
                table: "Player",
                columns: new[] { "ID", "AllyKill", "Attack", "AttackDelay", "Cash", "Defense", "Dig", "DirectHit", "Email", "EnemyKill", "EquippedAvatarBody", "EquippedAvatarExItem", "EquippedAvatarExtra", "EquippedAvatarFlag", "EquippedAvatarGoggles", "EquippedAvatarHat", "EquippedAvatarMisc", "EquippedAvatarPet", "Experience", "FriendlyFire", "Gender", "Gold", "GuildId", "Health", "HighAngleShots", "ItemDelay", "MatchesLeft", "MatchesPlayed", "Nickname", "Password", "Popularity", "PrimaryMobile", "Regeneration", "SecondaryMobile", "ShotCounter" },
                values: new object[,]
                {
                    { 4, 0L, 0, 0, 0, 0, 0, 0L, "dev03@dev.com", 0L, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0L, 0, 0, null, 0, 0L, 0, 0L, 0L, "Vinny", "$2y$12$ZcVa8MvpkEUx5LlQ5BNjNOezed07s8b71I5OcYq5vf1q52tjASjki", 0, 0, 0, 0, 0L },
                    { 1, 0L, 0, 0, 0, 0, 0, 0L, "dev00@dev.com", 0L, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0L, 0, 0, null, 0, 0L, 0, 0L, 0L, "Winged", "$2y$12$ZcVa8MvpkEUx5LlQ5BNjNOezed07s8b71I5OcYq5vf1q52tjASjki", 0, 0, 0, 0, 0L },
                    { 2, 0L, 0, 0, 0, 0, 0, 0L, "dev01@dev.com", 0L, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0L, 0, 0, null, 0, 0L, 0, 0L, 0L, "Wicked", "$2y$12$ZcVa8MvpkEUx5LlQ5BNjNOezed07s8b71I5OcYq5vf1q52tjASjki", 0, 0, 0, 0, 0L },
                    { 3, 0L, 0, 0, 0, 0, 0, 0L, "dev02@dev.com", 0L, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0L, 0, 0, null, 0, 0L, 0, 0L, 0L, "Willow", "$2y$12$ZcVa8MvpkEUx5LlQ5BNjNOezed07s8b71I5OcYq5vf1q52tjASjki", 0, 0, 0, 0, 0L },
                    { 5, 0L, 0, 0, 0, 0, 0, 0L, "test0@dev.com", 0L, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0L, 0, 0, null, 0, 0L, 0, 0L, 0L, "Test", "$2y$12$ZcVa8MvpkEUx5LlQ5BNjNOezed07s8b71I5OcYq5vf1q52tjASjki", 0, 0, 0, 0, 0L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guild_Tag",
                table: "Guild",
                column: "Tag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Player_GuildId",
                table: "Player",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_Nickname_Email",
                table: "Player",
                columns: new[] { "Nickname", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAvatarMetadata_AvatarMetadata_ID_AvatarMetadata_Gender_AvatarMetadata_AvatarCategory",
                table: "PlayerAvatarMetadata",
                columns: new[] { "AvatarMetadata_ID", "AvatarMetadata_Gender", "AvatarMetadata_AvatarCategory" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRelationship_Player_B_ID",
                table: "PlayerRelationship",
                column: "Player_B_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerAvatarMetadata");

            migrationBuilder.DropTable(
                name: "PlayerRelationship");

            migrationBuilder.DropTable(
                name: "AvatarMetadata");

            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropTable(
                name: "Guild");
        }
    }
}
