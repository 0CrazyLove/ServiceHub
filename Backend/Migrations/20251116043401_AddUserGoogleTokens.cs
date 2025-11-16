using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <summary>
    /// Add UserGoogleTokens table for Google OAuth integration.
    /// 
    /// Creates the UserGoogleTokens table to securely store Google refresh tokens
    /// for users authenticated via Google Sign-In. This enables the platform to:
    /// - Maintain long-lived user sessions without re-authentication
    /// - Perform background operations on behalf of authenticated users
    /// - Support seamless OAuth integration
    /// 
    /// Table columns:
    /// - Id: Primary key (auto-incrementing)
    /// - UserId: Foreign reference to AspNetUsers.Id
    /// - RefreshToken: OAuth refresh token value (encrypted in practice)
    /// - ExpiresAt: UTC timestamp when token expires
    /// - CreatedAt: UTC timestamp when record was created
    /// 
    /// Generated: 2025-11-16 04:34:01 UTC
    /// </summary>
    /// <inheritdoc />
    public partial class AddUserGoogleTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserGoogleTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGoogleTokens", x => x.Id);
                });
        }

        /// <summary>
        /// Revert the migration by removing the UserGoogleTokens table.
        /// 
        /// Dropping this table will remove all stored Google refresh tokens.
        /// This is typically used when rolling back to a version without Google OAuth support.
        /// </summary>
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGoogleTokens");
        }
    }
}
