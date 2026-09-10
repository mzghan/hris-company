using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRIS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddKpi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KpiCriteria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Weight = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KpiCriteria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KpiPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KpiPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeKpiScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KpiPeriodId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    CriteriaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Score = table.Column<decimal>(type: "TEXT", nullable: false),
                    FilledByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    FilledAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeKpiScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeKpiScores_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeKpiScores_KpiCriteria_CriteriaId",
                        column: x => x.CriteriaId,
                        principalTable: "KpiCriteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeKpiScores_KpiPeriods_KpiPeriodId",
                        column: x => x.KpiPeriodId,
                        principalTable: "KpiPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeKpiScores_Users_FilledByUserId",
                        column: x => x.FilledByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KpiScoreRevisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeKpiScoreId = table.Column<int>(type: "INTEGER", nullable: false),
                    PreviousScore = table.Column<decimal>(type: "TEXT", nullable: false),
                    NewScore = table.Column<decimal>(type: "TEXT", nullable: false),
                    RevisedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    RevisedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KpiScoreRevisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KpiScoreRevisions_EmployeeKpiScores_EmployeeKpiScoreId",
                        column: x => x.EmployeeKpiScoreId,
                        principalTable: "EmployeeKpiScores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KpiScoreRevisions_Users_RevisedByUserId",
                        column: x => x.RevisedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KpiPeriods_Name_Year",
                table: "KpiPeriods",
                columns: new[] { "Name", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeKpiScores_KpiPeriodId_EmployeeId_CriteriaId",
                table: "EmployeeKpiScores",
                columns: new[] { "KpiPeriodId", "EmployeeId", "CriteriaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeKpiScores_EmployeeId",
                table: "EmployeeKpiScores",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeKpiScores_CriteriaId",
                table: "EmployeeKpiScores",
                column: "CriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeKpiScores_FilledByUserId",
                table: "EmployeeKpiScores",
                column: "FilledByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_KpiScoreRevisions_EmployeeKpiScoreId",
                table: "KpiScoreRevisions",
                column: "EmployeeKpiScoreId");

            migrationBuilder.CreateIndex(
                name: "IX_KpiScoreRevisions_RevisedByUserId",
                table: "KpiScoreRevisions",
                column: "RevisedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KpiScoreRevisions");

            migrationBuilder.DropTable(
                name: "EmployeeKpiScores");

            migrationBuilder.DropTable(
                name: "KpiCriteria");

            migrationBuilder.DropTable(
                name: "KpiPeriods");
        }
    }
}
