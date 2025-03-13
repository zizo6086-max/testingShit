using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGoogleAuthProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoogleId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExternalAccount",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "GoogleId", "IsExternalAccount", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9eb3becd-b478-4a58-b24d-cc8c39998ed5", new DateTimeOffset(new DateTime(2025, 3, 13, 18, 59, 7, 612, DateTimeKind.Unspecified).AddTicks(4467), new TimeSpan(0, 0, 0, 0, 0)), null, false, "AQAAAAIAAYagAAAAEJO7taQfKQShbXkWzjR4vt6EbEv7jCxCxzcjrXtd2aR9BQwgNDfbpI8h8L+QKYG3Rw==", "c21b056d-5d53-4c9e-b3e0-97ff95f17a94" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "GoogleId", "IsExternalAccount", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f821a9ea-6d00-46d8-977a-278c75bf006d", new DateTimeOffset(new DateTime(2025, 3, 13, 18, 59, 7, 681, DateTimeKind.Unspecified).AddTicks(3786), new TimeSpan(0, 0, 0, 0, 0)), null, false, "AQAAAAIAAYagAAAAEN/bTrP2pJqys1bD8hbXPgz1tMBkqFy8VjMP4budaRoEpXMwTQ42aXv+7lz5F6KY7w==", "df087d96-3b40-4af3-abdd-6c4e06d99555" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsExternalAccount",
                table: "AspNetUsers");

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
    }
}
