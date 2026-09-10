using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRIS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceCameraGeo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckInPhotoPath",
                table: "Attendances",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CheckInLatitude",
                table: "Attendances",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CheckInLongitude",
                table: "Attendances",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckOutPhotoPath",
                table: "Attendances",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CheckOutLatitude",
                table: "Attendances",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CheckOutLongitude",
                table: "Attendances",
                type: "REAL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "CheckInPhotoPath", table: "Attendances");
            migrationBuilder.DropColumn(name: "CheckInLatitude", table: "Attendances");
            migrationBuilder.DropColumn(name: "CheckInLongitude", table: "Attendances");
            migrationBuilder.DropColumn(name: "CheckOutPhotoPath", table: "Attendances");
            migrationBuilder.DropColumn(name: "CheckOutLatitude", table: "Attendances");
            migrationBuilder.DropColumn(name: "CheckOutLongitude", table: "Attendances");
        }
    }
}
