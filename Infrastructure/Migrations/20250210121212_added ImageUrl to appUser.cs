using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedImageUrltoappUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "ImageUrl", "PasswordHash", "SecurityStamp" },
                values: new object[] { "03123900-6181-4268-8e72-ab16e92d3c5c", new DateTimeOffset(new DateTime(2025, 2, 10, 12, 12, 11, 827, DateTimeKind.Unspecified).AddTicks(6516), new TimeSpan(0, 0, 0, 0, 0)), null, "AQAAAAIAAYagAAAAEIqxxV2gN3Aw8FCYRZzXmo/q1vJ+8edmN9gbXUc8rPwmsy9Vr4ZuxK8HcL96QwChXA==", "617f8a94-e6e6-4ab9-9cb2-d0abbf6a522e" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "ImageUrl", "PasswordHash", "SecurityStamp" },
                values: new object[] { "457b0061-1f84-4c5e-9003-431fc0d03b19", new DateTimeOffset(new DateTime(2025, 2, 10, 12, 12, 11, 891, DateTimeKind.Unspecified).AddTicks(7689), new TimeSpan(0, 0, 0, 0, 0)), null, "AQAAAAIAAYagAAAAEPYWmDylrE2KySTgfOurpnUVW2efAp95UbIRvLSRrhcjQW4Sue444oXDXeZjFqqHhw==", "5f7e6c2c-907f-4935-acb0-a1ded0106e00" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cae43450-de75-49b0-b0d1-a5de385467d0", new DateTimeOffset(new DateTime(2025, 2, 8, 14, 48, 30, 960, DateTimeKind.Unspecified).AddTicks(854), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEPakAxdzlyzFYDmHM/9lxEo+L2WIkU+sKLk45oQZ8bMTFUkzVr39R9m8kGkHP7mTug==", "aef5d0c9-5267-42fd-a9b8-5e8e2e31ec69" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedOn", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c6cb5cdb-57b3-4817-99ce-86766c4f8f2c", new DateTimeOffset(new DateTime(2025, 2, 8, 14, 48, 31, 60, DateTimeKind.Unspecified).AddTicks(4857), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEKztcplAUK+LrRk7CeG47W54BxE8u6C8zR91sNdilkDzOYmn1iaMMGnL6sbdrUHieQ==", "c9d852f8-c87e-4a08-9313-c8ef58a17d08" });
        }
    }
}
