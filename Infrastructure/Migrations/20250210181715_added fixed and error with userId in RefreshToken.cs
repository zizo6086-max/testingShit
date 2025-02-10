using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedfixedanderrorwithuserIdinRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "RefreshTokens",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5f4169f3-e93e-4da7-bfa9-e648c2409e41", new DateTimeOffset(new DateTime(2025, 2, 10, 18, 17, 14, 768, DateTimeKind.Unspecified).AddTicks(9948), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEEY5t9H7AoIO0DCl0XJGrlJlhVranHJ+yF9cTr7d7XFAuaEUsAk2JoGbD6eR3HIVqA==", "d0d163df-479b-4ccb-a8df-5c2ab3cfaf33" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c2894e3d-b8d9-42d7-b136-892284cca3ff", new DateTimeOffset(new DateTime(2025, 2, 10, 18, 17, 14, 831, DateTimeKind.Unspecified).AddTicks(1666), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAECgNTEnulj2L7oHJ6PGUBRSkCY2WRxXip6jPKOtLK0+LaEr9GjPtQjW25gEMiX0+eQ==", "50259a4d-ecde-48e4-97bd-b6fb932d600c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

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
    }
}
