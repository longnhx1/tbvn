using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab_03.Migrations
{
    /// <inheritdoc />
    public partial class BookingPaymentPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DownPaymentAmount",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "InstallmentMonths",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyInterestPercent",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PaidInstallmentCount",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMode",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "RefundAmount",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPaid",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DownPaymentAmount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "InstallmentMonths",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "MonthlyInterestPercent",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaidInstallmentCount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaymentMode",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RefundAmount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TotalPaid",
                table: "Bookings");
        }
    }
}
