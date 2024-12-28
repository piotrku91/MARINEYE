using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MARINEYE.Migrations
{
    /// <inheritdoc />
    public partial class Boats4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "BoatModel");

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "BoatModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "BoatModel");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "BoatModel",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
