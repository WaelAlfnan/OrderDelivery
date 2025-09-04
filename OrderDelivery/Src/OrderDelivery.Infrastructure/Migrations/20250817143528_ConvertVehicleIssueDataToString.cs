using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderDelivery.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertVehicleIssueDataToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VehicleIssueDate",
                table: "Vehicles",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "VehicleIssueDate",
                table: "Vehicles",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)",
                oldMaxLength: 4);
        }
    }
}
