using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRIS.Api.Migrations
{
    /// <inheritdoc />
    public partial class BatchB_Content : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TRX_Assistance_Request",
                columns: table => new
                {
                    request_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    employee_id = table.Column<int>(type: "INTEGER", nullable: true),
                    category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    is_anonymous = table.Column<bool>(type: "INTEGER", nullable: false),
                    status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    handled_by_user_id = table.Column<int>(type: "INTEGER", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trx_assistance_request", x => x.request_id);
                    table.ForeignKey(
                        name: "fk_trx_assistance_request_mst_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_trx_assistance_request_sys_user_handled_by_user_id",
                        column: x => x.handled_by_user_id,
                        principalTable: "SYS_User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TRX_Learning_Material",
                columns: table => new
                {
                    material_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    document_id = table.Column<int>(type: "INTEGER", nullable: false),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trx_learning_material", x => x.material_id);
                    table.ForeignKey(
                        name: "fk_trx_learning_material_trx_document_document_id",
                        column: x => x.document_id,
                        principalTable: "TRX_Document",
                        principalColumn: "document_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TRX_Regulation",
                columns: table => new
                {
                    regulation_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    content = table.Column<string>(type: "TEXT", nullable: false),
                    sort_order = table.Column<int>(type: "INTEGER", nullable: false),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trx_regulation", x => x.regulation_id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_trx_assistance_request_employee_id",
                table: "TRX_Assistance_Request",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_assistance_request_handled_by_user_id",
                table: "TRX_Assistance_Request",
                column: "handled_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_learning_material_document_id",
                table: "TRX_Learning_Material",
                column: "document_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TRX_Assistance_Request");

            migrationBuilder.DropTable(
                name: "TRX_Learning_Material");

            migrationBuilder.DropTable(
                name: "TRX_Regulation");
        }
    }
}
