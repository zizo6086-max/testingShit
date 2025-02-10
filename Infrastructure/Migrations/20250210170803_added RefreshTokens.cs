using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedRefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevokeReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Token);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1ff73d56-a910-4f02-9ab7-d75ca49d9279", new DateTimeOffset(new DateTime(2025, 2, 10, 17, 8, 3, 202, DateTimeKind.Unspecified).AddTicks(1391), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEL5ssURHn1VcWaLmCT2SAMmH2bM4pZksQ2EAbbXE0OPIVbhA0SYQ7pVJzcQMQySqgA==", "bb8306a8-4c26-4dd7-90fc-f1bce83e7a1b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "864991aa-e569-4faf-b39d-5d6cce8548f9", new DateTimeOffset(new DateTime(2025, 2, 10, 17, 8, 3, 266, DateTimeKind.Unspecified).AddTicks(4320), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEDF7b+BdlpJdxAVGs+yZjrvsht+ZcSIfbXR8rhVrTahyPQwm1sfFkTyNQbX5JvkXpw==", "19305808-dc7a-49e1-b39f-95ba786f6f8b" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "03123900-6181-4268-8e72-ab16e92d3c5c", new DateTimeOffset(new DateTime(2025, 2, 10, 12, 12, 11, 827, DateTimeKind.Unspecified).AddTicks(6516), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEIqxxV2gN3Aw8FCYRZzXmo/q1vJ+8edmN9gbXUc8rPwmsy9Vr4ZuxK8HcL96QwChXA==", "617f8a94-e6e6-4ab9-9cb2-d0abbf6a522e" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "457b0061-1f84-4c5e-9003-431fc0d03b19", new DateTimeOffset(new DateTime(2025, 2, 10, 12, 12, 11, 891, DateTimeKind.Unspecified).AddTicks(7689), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEPYWmDylrE2KySTgfOurpnUVW2efAp95UbIRvLSRrhcjQW4Sue444oXDXeZjFqqHhw==", "5f7e6c2c-907f-4935-acb0-a1ded0106e00" });
        }
    }
}
