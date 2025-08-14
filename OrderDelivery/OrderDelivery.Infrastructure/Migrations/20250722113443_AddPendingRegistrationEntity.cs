using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderDelivery.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingRegistrationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PendingRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPhoneVerified = table.Column<bool>(type: "bit", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BasicInfoJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchantInfoJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriverInfoJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VehicleInfoJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResidenceInfoJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Step = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingRegistrations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingRegistrations");
        }
    }
}
