using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderDelivery.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeletedAdminEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserType",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanManageDrivers = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CanManageMerchants = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CanManageOrders = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CanViewReports = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_ApplicationUserId",
                table: "Admins",
                column: "ApplicationUserId",
                unique: true);
        }
    }
}
