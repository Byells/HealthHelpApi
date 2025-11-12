using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHelp.Api.Migrations
{
    /// <inheritdoc />
    public partial class initial_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HealthHelp_Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthHelp_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HealthHelp_Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UserName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthHelp_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HealthHelp_RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    RoleId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ClaimValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthHelp_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthHelp_RoleClaims_HealthHelp_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "HealthHelp_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HealthHelp_UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ClaimValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthHelp_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthHelp_UserClaims_HealthHelp_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "HealthHelp_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HealthHelp_UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthHelp_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_HealthHelp_UserLogins_HealthHelp_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "HealthHelp_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HealthHelp_UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RoleId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthHelp_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_HealthHelp_UserRoles_HealthHelp_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "HealthHelp_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HealthHelp_UserRoles_HealthHelp_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "HealthHelp_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HealthHelp_UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Value = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthHelp_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_HealthHelp_UserTokens_HealthHelp_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "HealthHelp_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoutineEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Category = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Hours = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutineEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutineEntries_HealthHelp_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "HealthHelp_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HealthHelp_RoleClaims_RoleId",
                table: "HealthHelp_RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "HealthHelp_RoleNameIndex",
                table: "HealthHelp_Roles",
                column: "NormalizedName",
                unique: true,
                filter: "\"NormalizedName\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HealthHelp_UserClaims_UserId",
                table: "HealthHelp_UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthHelp_UserLogins_UserId",
                table: "HealthHelp_UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthHelp_UserRoles_RoleId",
                table: "HealthHelp_UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "HealthHelp_EmailIndex",
                table: "HealthHelp_Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "HealthHelp_UserNameIndex",
                table: "HealthHelp_Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "\"NormalizedUserName\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineEntries_ApplicationUserId",
                table: "RoutineEntries",
                column: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HealthHelp_RoleClaims");

            migrationBuilder.DropTable(
                name: "HealthHelp_UserClaims");

            migrationBuilder.DropTable(
                name: "HealthHelp_UserLogins");

            migrationBuilder.DropTable(
                name: "HealthHelp_UserRoles");

            migrationBuilder.DropTable(
                name: "HealthHelp_UserTokens");

            migrationBuilder.DropTable(
                name: "RoutineEntries");

            migrationBuilder.DropTable(
                name: "HealthHelp_Roles");

            migrationBuilder.DropTable(
                name: "HealthHelp_Users");
        }
    }
}
