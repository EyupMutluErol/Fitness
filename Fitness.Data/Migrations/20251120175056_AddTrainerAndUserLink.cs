using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitness.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainerAndUserLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_AppUserId",
                table: "Appointments");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Trainers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_AppUserId",
                table: "Trainers",
                column: "AppUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_AppUserId",
                table: "Appointments",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_AspNetUsers_AppUserId",
                table: "Trainers",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_AppUserId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_AspNetUsers_AppUserId",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Trainers_AppUserId",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Trainers");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_AppUserId",
                table: "Appointments",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
