using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MARINEYE.Migrations
{
    /// <inheritdoc />
    public partial class Dues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DueTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClubDueId = table.Column<int>(type: "int", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DueTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DueTransactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DueTransactions_ClubDueModel_ClubDueId",
                        column: x => x.ClubDueId,
                        principalTable: "ClubDueModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DueTransactions_ClubDueId",
                table: "DueTransactions",
                column: "ClubDueId");

            migrationBuilder.CreateIndex(
                name: "IX_DueTransactions_UserId",
                table: "DueTransactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DueTransactions");
        }
    }
}
