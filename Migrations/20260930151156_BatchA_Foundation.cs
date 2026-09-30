using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRIS.Api.Migrations
{
    /// <inheritdoc />
    public partial class BatchA_Foundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TRX_Leave_Approval");

            migrationBuilder.RenameColumn(
                name: "current_level",
                table: "TRX_Leave_Request",
                newName: "days");

            migrationBuilder.AddColumn<int>(
                name: "created_by",
                table: "TRX_Leave_Request",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "TRX_Leave_Request",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "updated_by",
                table: "TRX_Leave_Request",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "REF_Approval_Flow_Step",
                columns: table => new
                {
                    flow_step_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    request_type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    level = table.Column<int>(type: "INTEGER", nullable: false),
                    approver_type = table.Column<string>(type: "TEXT", nullable: false),
                    chain_depth = table.Column<int>(type: "INTEGER", nullable: true),
                    role_id = table.Column<int>(type: "INTEGER", nullable: true),
                    approver_employee_id = table.Column<int>(type: "INTEGER", nullable: true),
                    min_requested_days = table.Column<int>(type: "INTEGER", nullable: true),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_approval_flow_step", x => x.flow_step_id);
                    table.ForeignKey(
                        name: "fk_ref_approval_flow_step_employees_approver_employee_id",
                        column: x => x.approver_employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ref_approval_flow_step_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "SYS_Role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "REF_Document_Category",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    parent_id = table.Column<int>(type: "INTEGER", nullable: true),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_document_category", x => x.category_id);
                    table.ForeignKey(
                        name: "fk_ref_document_category_ref_document_category_parent_id",
                        column: x => x.parent_id,
                        principalTable: "REF_Document_Category",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SYS_Audit_Log",
                columns: table => new
                {
                    log_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<int>(type: "INTEGER", nullable: true),
                    action = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    entity_type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    entity_id = table.Column<int>(type: "INTEGER", nullable: true),
                    detail = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sys_audit_log", x => x.log_id);
                    table.ForeignKey(
                        name: "fk_sys_audit_log_sys_user_user_id",
                        column: x => x.user_id,
                        principalTable: "SYS_User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TRX_Approval_Request",
                columns: table => new
                {
                    approval_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    request_type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    request_ref_id = table.Column<int>(type: "INTEGER", nullable: false),
                    requester_id = table.Column<int>(type: "INTEGER", nullable: false),
                    summary = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    current_level = table.Column<int>(type: "INTEGER", nullable: false),
                    completed_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trx_approval_request", x => x.approval_id);
                    table.ForeignKey(
                        name: "fk_trx_approval_request_mst_employee_requester_id",
                        column: x => x.requester_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TRX_Email_Outbox",
                columns: table => new
                {
                    email_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    to_address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    to_name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    subject = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    body = table.Column<string>(type: "TEXT", nullable: false),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    attempts = table.Column<int>(type: "INTEGER", nullable: false),
                    last_error = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    next_attempt_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    sent_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trx_email_outbox", x => x.email_id);
                });

            migrationBuilder.CreateTable(
                name: "TRX_Notification",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    message = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    link_url = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    is_read = table.Column<bool>(type: "INTEGER", nullable: false),
                    read_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trx_notification", x => x.notification_id);
                    table.ForeignKey(
                        name: "fk_trx_notification_sys_user_user_id",
                        column: x => x.user_id,
                        principalTable: "SYS_User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TRX_Document",
                columns: table => new
                {
                    document_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    category_id = table.Column<int>(type: "INTEGER", nullable: false),
                    title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    file_name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    stored_path = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    content_type = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    size_bytes = table.Column<long>(type: "INTEGER", nullable: false),
                    owner_employee_id = table.Column<int>(type: "INTEGER", nullable: true),
                    uploaded_by_user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trx_document", x => x.document_id);
                    table.ForeignKey(
                        name: "fk_trx_document_mst_employee_owner_employee_id",
                        column: x => x.owner_employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trx_document_ref_document_category_category_id",
                        column: x => x.category_id,
                        principalTable: "REF_Document_Category",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trx_document_sys_user_uploaded_by_user_id",
                        column: x => x.uploaded_by_user_id,
                        principalTable: "SYS_User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TRX_Approval_Step",
                columns: table => new
                {
                    step_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    approval_id = table.Column<int>(type: "INTEGER", nullable: false),
                    level = table.Column<int>(type: "INTEGER", nullable: false),
                    approver_type = table.Column<string>(type: "TEXT", nullable: false),
                    approver_role_id = table.Column<int>(type: "INTEGER", nullable: true),
                    approver_employee_id = table.Column<int>(type: "INTEGER", nullable: true),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    acted_by_user_id = table.Column<int>(type: "INTEGER", nullable: true),
                    is_support_override = table.Column<bool>(type: "INTEGER", nullable: false),
                    acted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trx_approval_step", x => x.step_id);
                    table.ForeignKey(
                        name: "fk_trx_approval_step_mst_employee_approver_employee_id",
                        column: x => x.approver_employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trx_approval_step_sys_role_approver_role_id",
                        column: x => x.approver_role_id,
                        principalTable: "SYS_Role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trx_approval_step_sys_user_acted_by_user_id",
                        column: x => x.acted_by_user_id,
                        principalTable: "SYS_User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trx_approval_step_trx_approval_request_approval_id",
                        column: x => x.approval_id,
                        principalTable: "TRX_Approval_Request",
                        principalColumn: "approval_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ref_approval_flow_step_approver_employee_id",
                table: "REF_Approval_Flow_Step",
                column: "approver_employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_ref_approval_flow_step_request_type_level",
                table: "REF_Approval_Flow_Step",
                columns: new[] { "request_type", "level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ref_approval_flow_step_role_id",
                table: "REF_Approval_Flow_Step",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_ref_document_category_parent_id",
                table: "REF_Document_Category",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_sys_audit_log_user_id",
                table: "SYS_Audit_Log",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_approval_request_request_type_request_ref_id",
                table: "TRX_Approval_Request",
                columns: new[] { "request_type", "request_ref_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_trx_approval_request_requester_id",
                table: "TRX_Approval_Request",
                column: "requester_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_approval_step_acted_by_user_id",
                table: "TRX_Approval_Step",
                column: "acted_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_approval_step_approval_id_level",
                table: "TRX_Approval_Step",
                columns: new[] { "approval_id", "level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_trx_approval_step_approver_employee_id",
                table: "TRX_Approval_Step",
                column: "approver_employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_approval_step_approver_role_id",
                table: "TRX_Approval_Step",
                column: "approver_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_document_category_id",
                table: "TRX_Document",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_document_owner_employee_id",
                table: "TRX_Document",
                column: "owner_employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_document_uploaded_by_user_id",
                table: "TRX_Document",
                column: "uploaded_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_email_outbox_status_next_attempt_at",
                table: "TRX_Email_Outbox",
                columns: new[] { "status", "next_attempt_at" });

            migrationBuilder.CreateIndex(
                name: "ix_trx_notification_user_id_is_read",
                table: "TRX_Notification",
                columns: new[] { "user_id", "is_read" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "REF_Approval_Flow_Step");

            migrationBuilder.DropTable(
                name: "SYS_Audit_Log");

            migrationBuilder.DropTable(
                name: "TRX_Approval_Step");

            migrationBuilder.DropTable(
                name: "TRX_Document");

            migrationBuilder.DropTable(
                name: "TRX_Email_Outbox");

            migrationBuilder.DropTable(
                name: "TRX_Notification");

            migrationBuilder.DropTable(
                name: "TRX_Approval_Request");

            migrationBuilder.DropTable(
                name: "REF_Document_Category");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "TRX_Leave_Request");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "TRX_Leave_Request");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "TRX_Leave_Request");

            migrationBuilder.RenameColumn(
                name: "days",
                table: "TRX_Leave_Request",
                newName: "current_level");

            migrationBuilder.CreateTable(
                name: "TRX_Leave_Approval",
                columns: table => new
                {
                    leave_approval_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    approver_id = table.Column<int>(type: "INTEGER", nullable: false),
                    leave_request_id = table.Column<int>(type: "INTEGER", nullable: false),
                    acted_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    level = table.Column<int>(type: "INTEGER", nullable: false),
                    note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trx_leave_approval", x => x.leave_approval_id);
                    table.ForeignKey(
                        name: "fk_trx_leave_approval_mst_employee_approver_id",
                        column: x => x.approver_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trx_leave_approval_trx_leave_request_leave_request_id",
                        column: x => x.leave_request_id,
                        principalTable: "TRX_Leave_Request",
                        principalColumn: "leave_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_trx_leave_approval_approver_id",
                table: "TRX_Leave_Approval",
                column: "approver_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_leave_approval_leave_request_id",
                table: "TRX_Leave_Approval",
                column: "leave_request_id");
        }
    }
}
