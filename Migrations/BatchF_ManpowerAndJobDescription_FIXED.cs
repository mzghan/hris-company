using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRIS.Api.Migrations
{
    /// <inheritdoc />
    public partial class BatchF_ManpowerAndJobDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Competency_Assessment_TRX_Employee_Competency_employee_competency_id",
                table: "TRX_Competency_Assessment");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Employee_Competency_MST_Employee_employee_id",
                table: "TRX_Employee_Competency");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Employee_Competency_REF_Competency_competency_id",
                table: "TRX_Employee_Competency");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Employee_Competency_SYS_User_assigned_by_user_id",
                table: "TRX_Employee_Competency");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Employee_Evaluation_MST_Employee_Employment_employment_id",
                table: "TRX_Employee_Evaluation");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Employee_Evaluation_MST_Employee_employee_id",
                table: "TRX_Employee_Evaluation");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Employee_Evaluation_REF_Evaluation_Type_eval_type_id",
                table: "TRX_Employee_Evaluation");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Evaluation_Entry_TRX_Employee_Evaluation_evaluation_id",
                table: "TRX_Evaluation_Entry");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Evaluation_Score_SYS_User_scored_by_user_id",
                table: "TRX_Evaluation_Score");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Evaluation_Score_TRX_Employee_Evaluation_evaluation_id",
                table: "TRX_Evaluation_Score");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Performance_Plan_MST_Employee_employee_id",
                table: "TRX_Performance_Plan");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Performance_Plan_SYS_User_created_by_user_id",
                table: "TRX_Performance_Plan");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Personal_Action_MST_Employee_employee_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Personal_Action_MST_Employee_new_manager_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Personal_Action_REF_Grade_new_grade_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Personal_Action_REF_Job_Level_new_job_level_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Personal_Action_REF_Job_Title_new_job_title_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Personal_Action_REF_Location_new_location_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Personal_Action_REF_Organization_new_organization_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Plan_Task_SYS_User_assigned_by_user_id",
                table: "TRX_Plan_Task");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Plan_Task_TRX_Performance_Plan_plan_id",
                table: "TRX_Plan_Task");

            migrationBuilder.DropForeignKey(
                name: "FK_TRX_Plan_WorkLog_TRX_Performance_Plan_plan_id",
                table: "TRX_Plan_WorkLog");

            migrationBuilder.DropIndex(
                name: "ix_trx_room_booking_room_id",
                table: "TRX_Room_Booking");

            migrationBuilder.DropIndex(
                name: "ix_trx_personal_action_employee_id_status",
                table: "TRX_Personal_Action");

            migrationBuilder.DropIndex(
                name: "ix_trx_leave_encashment_employee_id_status",
                table: "TRX_Leave_Encashment");

            migrationBuilder.DropIndex(
                name: "ix_trx_leave_balance_employee_id",
                table: "TRX_Leave_Balance");

            migrationBuilder.DropIndex(
                name: "ix_trx_health_claim_employee_id_status",
                table: "TRX_Health_Claim");

            migrationBuilder.DropIndex(
                name: "ix_trx_employee_evaluation_employee_id_employment_id_eval_type_id",
                table: "TRX_Employee_Evaluation");

            migrationBuilder.DropIndex(
                name: "ix_trx_employee_competency_employee_id_competency_id",
                table: "TRX_Employee_Competency");

            migrationBuilder.DropIndex(
                name: "ix_trx_attendance_employee_id",
                table: "TRX_Attendance");

            migrationBuilder.DropIndex(
                name: "ix_ref_evaluation_type_name",
                table: "REF_Evaluation_Type");

            migrationBuilder.DropIndex(
                name: "ix_ref_competency_name",
                table: "REF_Competency");

            migrationBuilder.RenameColumn(
                name: "eval_type_id",
                table: "TRX_Employee_Evaluation",
                newName: "evaluation_type_id");

            migrationBuilder.AlterColumn<int>(
                name: "booking_id",
                table: "TRX_Room_Booking",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "manpower_id",
                table: "TRX_Manpower_Request",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "approval_phase",
                table: "TRX_Manpower_Request",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "business_plan_path",
                table: "TRX_Manpower_Request",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "job_description_id",
                table: "TRX_Manpower_Request",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "job_description_path",
                table: "TRX_Manpower_Request",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "manual_mrf_path",
                table: "TRX_Manpower_Request",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "replacement_for_employee_id",
                table: "TRX_Manpower_Request",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "report_to_employee_id",
                table: "TRX_Manpower_Request",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "request_type",
                table: "TRX_Manpower_Request",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "work_status",
                table: "TRX_Manpower_Request",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "encashment_id",
                table: "TRX_Leave_Encashment",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "balance_id",
                table: "TRX_Leave_Balance",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "claim_id",
                table: "TRX_Health_Claim",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "period_id",
                table: "TRX_Flex_Period",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "work_type_id",
                table: "REF_Work_Type",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "holiday_id",
                table: "REF_Public_Holiday",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "room_id",
                table: "REF_Meeting_Room",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "leave_type_id",
                table: "REF_Leave_Type",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateTable(
                name: "BANK_JOBDESC",
                columns: table => new
                {
                    bank_jobdesc_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    job_title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    snapshot_json = table.Column<string>(type: "TEXT", nullable: false),
                    status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bank_jobdesc", x => x.bank_jobdesc_id);
                });

            migrationBuilder.CreateTable(
                name: "MST_JOBDESC",
                columns: table => new
                {
                    mst_jobdesc_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    job_title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    snapshot_json = table.Column<string>(type: "TEXT", nullable: false),
                    finalized_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    job_holder_employee_id = table.Column<int>(type: "INTEGER", nullable: true),
                    immediate_manager_employee_id = table.Column<int>(type: "INTEGER", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mst_jobdesc", x => x.mst_jobdesc_id);
                    table.ForeignKey(
                        name: "fk_mst_jobdesc_mst_employee_immediate_manager_employee_id",
                        column: x => x.immediate_manager_employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id");
                    table.ForeignKey(
                        name: "fk_mst_jobdesc_mst_employee_job_holder_employee_id",
                        column: x => x.job_holder_employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id");
                });

            

            

            

            migrationBuilder.CreateTable(
                name: "TRN_Jobdesc",
                columns: table => new
                {
                    jobdesc_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    job_title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    organization_id = table.Column<int>(type: "INTEGER", nullable: true),
                    job_level_id = table.Column<int>(type: "INTEGER", nullable: true),
                    grade_id = table.Column<int>(type: "INTEGER", nullable: true),
                    job_holder_employee_id = table.Column<int>(type: "INTEGER", nullable: true),
                    immediate_manager_employee_id = table.Column<int>(type: "INTEGER", nullable: true),
                    purpose = table.Column<string>(type: "TEXT", nullable: false),
                    reporting_relationship = table.Column<string>(type: "TEXT", nullable: false),
                    dimensions = table.Column<string>(type: "TEXT", nullable: false),
                    kri_cico = table.Column<string>(type: "TEXT", nullable: false),
                    kri_compliance = table.Column<string>(type: "TEXT", nullable: false),
                    kri_areas = table.Column<string>(type: "TEXT", nullable: false),
                    stakeholders = table.Column<string>(type: "TEXT", nullable: false),
                    challenges = table.Column<string>(type: "TEXT", nullable: false),
                    qualifications = table.Column<string>(type: "TEXT", nullable: false),
                    experience = table.Column<string>(type: "TEXT", nullable: false),
                    competencies = table.Column<string>(type: "TEXT", nullable: false),
                    status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    approval_flag = table.Column<int>(type: "INTEGER", nullable: false),
                    approvers_json = table.Column<string>(type: "TEXT", nullable: false),
                    approver_history_json = table.Column<string>(type: "TEXT", nullable: false),
                    job_holder_signature_text = table.Column<string>(type: "TEXT", nullable: true),
                    date_sign_job_holder = table.Column<DateTime>(type: "TEXT", nullable: true),
                    manager_signature_text = table.Column<string>(type: "TEXT", nullable: true),
                    date_sign_manager = table.Column<DateTime>(type: "TEXT", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trn_jobdesc", x => x.jobdesc_id);
                    table.ForeignKey(
                        name: "fk_trn_jobdesc_mst_employee_immediate_manager_employee_id",
                        column: x => x.immediate_manager_employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id");
                    table.ForeignKey(
                        name: "fk_trn_jobdesc_mst_employee_job_holder_employee_id",
                        column: x => x.job_holder_employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id");
                    table.ForeignKey(
                        name: "fk_trn_jobdesc_ref_grade_grade_id",
                        column: x => x.grade_id,
                        principalTable: "REF_Grade",
                        principalColumn: "grade_id");
                    table.ForeignKey(
                        name: "fk_trn_jobdesc_ref_job_level_job_level_id",
                        column: x => x.job_level_id,
                        principalTable: "REF_Job_Level",
                        principalColumn: "job_level_id");
                    table.ForeignKey(
                        name: "fk_trn_jobdesc_ref_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "REF_Organization",
                        principalColumn: "organization_id");
                });

            

            

            

            

            

            

            

            

            

            migrationBuilder.CreateTable(
                name: "TRN_Revise_History_JD",
                columns: table => new
                {
                    revision_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    job_description_id = table.Column<int>(type: "INTEGER", nullable: false),
                    revision_no = table.Column<int>(type: "INTEGER", nullable: false),
                    snapshot_json = table.Column<string>(type: "TEXT", nullable: false),
                    change_reason = table.Column<string>(type: "TEXT", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trn_revise_history_jd", x => x.revision_id);
                    table.ForeignKey(
                        name: "fk_trn_revise_history_jd_trn_jobdesc_job_description_id",
                        column: x => x.job_description_id,
                        principalTable: "TRN_Jobdesc",
                        principalColumn: "jobdesc_id",
                        onDelete: ReferentialAction.Cascade);
                });

            

            

            migrationBuilder.CreateIndex(
                name: "ix_trx_plan_work_log_plan_id",
                table: "TRX_Plan_WorkLog",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_plan_task_assigned_by_user_id",
                table: "TRX_Plan_Task",
                column: "assigned_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_plan_task_plan_id",
                table: "TRX_Plan_Task",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_personal_action_employee_id",
                table: "TRX_Personal_Action",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_personal_action_new_grade_id",
                table: "TRX_Personal_Action",
                column: "new_grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_personal_action_new_job_level_id",
                table: "TRX_Personal_Action",
                column: "new_job_level_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_personal_action_new_job_title_id",
                table: "TRX_Personal_Action",
                column: "new_job_title_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_personal_action_new_location_id",
                table: "TRX_Personal_Action",
                column: "new_location_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_personal_action_new_manager_id",
                table: "TRX_Personal_Action",
                column: "new_manager_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_personal_action_new_organization_id",
                table: "TRX_Personal_Action",
                column: "new_organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_performance_plan_created_by_user_id",
                table: "TRX_Performance_Plan",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_performance_plan_employee_id",
                table: "TRX_Performance_Plan",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_manpower_request_job_description_id",
                table: "TRX_Manpower_Request",
                column: "job_description_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_manpower_request_replacement_for_employee_id",
                table: "TRX_Manpower_Request",
                column: "replacement_for_employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_manpower_request_report_to_employee_id",
                table: "TRX_Manpower_Request",
                column: "report_to_employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_evaluation_score_evaluation_id",
                table: "TRX_Evaluation_Score",
                column: "evaluation_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_evaluation_score_scored_by_user_id",
                table: "TRX_Evaluation_Score",
                column: "scored_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_evaluation_entry_evaluation_id",
                table: "TRX_Evaluation_Entry",
                column: "evaluation_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_employee_evaluation_employee_id",
                table: "TRX_Employee_Evaluation",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_employee_evaluation_employment_id",
                table: "TRX_Employee_Evaluation",
                column: "employment_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_employee_evaluation_evaluation_type_id",
                table: "TRX_Employee_Evaluation",
                column: "evaluation_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_employee_competency_assigned_by_user_id",
                table: "TRX_Employee_Competency",
                column: "assigned_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_employee_competency_competency_id",
                table: "TRX_Employee_Competency",
                column: "competency_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_employee_competency_employee_id",
                table: "TRX_Employee_Competency",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_competency_assessment_employee_competency_id",
                table: "TRX_Competency_Assessment",
                column: "employee_competency_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_jobdesc_immediate_manager_employee_id",
                table: "MST_JOBDESC",
                column: "immediate_manager_employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_jobdesc_job_holder_employee_id",
                table: "MST_JOBDESC",
                column: "job_holder_employee_id");

            

            

            migrationBuilder.CreateIndex(
                name: "ix_trn_jobdesc_code",
                table: "TRN_Jobdesc",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_trn_jobdesc_grade_id",
                table: "TRN_Jobdesc",
                column: "grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_trn_jobdesc_immediate_manager_employee_id",
                table: "TRN_Jobdesc",
                column: "immediate_manager_employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_trn_jobdesc_job_holder_employee_id",
                table: "TRN_Jobdesc",
                column: "job_holder_employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_trn_jobdesc_job_level_id",
                table: "TRN_Jobdesc",
                column: "job_level_id");

            migrationBuilder.CreateIndex(
                name: "ix_trn_jobdesc_organization_id",
                table: "TRN_Jobdesc",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_trn_revise_history_jd_job_description_id",
                table: "TRN_Revise_History_JD",
                column: "job_description_id");

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            migrationBuilder.AddForeignKey(
                name: "fk_trx_competency_assessment_trx_employee_competency_employee_competency_id",
                table: "TRX_Competency_Assessment",
                column: "employee_competency_id",
                principalTable: "TRX_Employee_Competency",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trx_employee_competency_mst_employee_employee_id",
                table: "TRX_Employee_Competency",
                column: "employee_id",
                principalTable: "MST_Employee",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trx_employee_competency_ref_competency_competency_id",
                table: "TRX_Employee_Competency",
                column: "competency_id",
                principalTable: "REF_Competency",
                principalColumn: "competency_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trx_employee_competency_sys_user_assigned_by_user_id",
                table: "TRX_Employee_Competency",
                column: "assigned_by_user_id",
                principalTable: "SYS_User",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trx_employee_evaluation_mst_employee_employee_id",
                table: "TRX_Employee_Evaluation",
                column: "employee_id",
                principalTable: "MST_Employee",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trx_employee_evaluation_mst_employee_employment_employment_id",
                table: "TRX_Employee_Evaluation",
                column: "employment_id",
                principalTable: "MST_Employee_Employment",
                principalColumn: "employment_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trx_employee_evaluation_ref_evaluation_type_evaluation_type_id",
                table: "TRX_Employee_Evaluation",
                column: "evaluation_type_id",
                principalTable: "REF_Evaluation_Type",
                principalColumn: "eval_type_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trx_evaluation_entry_trx_employee_evaluation_evaluation_id",
                table: "TRX_Evaluation_Entry",
                column: "evaluation_id",
                principalTable: "TRX_Employee_Evaluation",
                principalColumn: "evaluation_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trx_evaluation_score_sys_user_scored_by_user_id",
                table: "TRX_Evaluation_Score",
                column: "scored_by_user_id",
                principalTable: "SYS_User",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trx_evaluation_score_trx_employee_evaluation_evaluation_id",
                table: "TRX_Evaluation_Score",
                column: "evaluation_id",
                principalTable: "TRX_Employee_Evaluation",
                principalColumn: "evaluation_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trx_manpower_request_job_descriptions_job_description_id",
                table: "TRX_Manpower_Request",
                column: "job_description_id",
                principalTable: "TRN_Jobdesc",
                principalColumn: "jobdesc_id");

            migrationBuilder.AddForeignKey(
                name: "fk_trx_manpower_request_mst_employee_replacement_for_employee_id",
                table: "TRX_Manpower_Request",
                column: "replacement_for_employee_id",
                principalTable: "MST_Employee",
                principalColumn: "employee_id");

            migrationBuilder.AddForeignKey(
                name: "fk_trx_manpower_request_mst_employee_report_to_employee_id",
                table: "TRX_Manpower_Request",
                column: "report_to_employee_id",
                principalTable: "MST_Employee",
                principalColumn: "employee_id");

            migrationBuilder.AddForeignKey(
                name: "fk_trx_performance_plan_mst_employee_employee_id",
                table: "TRX_Performance_Plan",
                column: "employee_id",
                principalTable: "MST_Employee",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trx_performance_plan_sys_user_created_by_user_id",
                table: "TRX_Performance_Plan",
                column: "created_by_user_id",
                principalTable: "SYS_User",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trx_personal_action_mst_employee_employee_id",
                table: "TRX_Personal_Action",
                column: "employee_id",
                principalTable: "MST_Employee",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trx_personal_action_mst_employee_new_manager_id",
                table: "TRX_Personal_Action",
                column: "new_manager_id",
                principalTable: "MST_Employee",
                principalColumn: "employee_id");

            migrationBuilder.AddForeignKey(
                name: "fk_trx_personal_action_ref_grade_new_grade_id",
                table: "TRX_Personal_Action",
                column: "new_grade_id",
                principalTable: "REF_Grade",
                principalColumn: "grade_id");

            migrationBuilder.AddForeignKey(
                name: "fk_trx_personal_action_ref_job_level_new_job_level_id",
                table: "TRX_Personal_Action",
                column: "new_job_level_id",
                principalTable: "REF_Job_Level",
                principalColumn: "job_level_id");

            migrationBuilder.AddForeignKey(
                name: "fk_trx_personal_action_ref_job_title_new_job_title_id",
                table: "TRX_Personal_Action",
                column: "new_job_title_id",
                principalTable: "REF_Job_Title",
                principalColumn: "job_title_id");

            migrationBuilder.AddForeignKey(
                name: "fk_trx_personal_action_ref_location_new_location_id",
                table: "TRX_Personal_Action",
                column: "new_location_id",
                principalTable: "REF_Location",
                principalColumn: "location_id");

            migrationBuilder.AddForeignKey(
                name: "fk_trx_personal_action_ref_organization_new_organization_id",
                table: "TRX_Personal_Action",
                column: "new_organization_id",
                principalTable: "REF_Organization",
                principalColumn: "organization_id");

            migrationBuilder.AddForeignKey(
                name: "fk_trx_plan_task_sys_user_assigned_by_user_id",
                table: "TRX_Plan_Task",
                column: "assigned_by_user_id",
                principalTable: "SYS_User",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trx_plan_task_trx_performance_plan_plan_id",
                table: "TRX_Plan_Task",
                column: "plan_id",
                principalTable: "TRX_Performance_Plan",
                principalColumn: "plan_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trx_plan_work_log_trx_performance_plan_plan_id",
                table: "TRX_Plan_WorkLog",
                column: "plan_id",
                principalTable: "TRX_Performance_Plan",
                principalColumn: "plan_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_trx_competency_assessment_trx_employee_competency_employee_competency_id",
                table: "TRX_Competency_Assessment");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_employee_competency_mst_employee_employee_id",
                table: "TRX_Employee_Competency");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_employee_competency_ref_competency_competency_id",
                table: "TRX_Employee_Competency");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_employee_competency_sys_user_assigned_by_user_id",
                table: "TRX_Employee_Competency");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_employee_evaluation_mst_employee_employee_id",
                table: "TRX_Employee_Evaluation");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_employee_evaluation_mst_employee_employment_employment_id",
                table: "TRX_Employee_Evaluation");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_employee_evaluation_ref_evaluation_type_evaluation_type_id",
                table: "TRX_Employee_Evaluation");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_evaluation_entry_trx_employee_evaluation_evaluation_id",
                table: "TRX_Evaluation_Entry");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_evaluation_score_sys_user_scored_by_user_id",
                table: "TRX_Evaluation_Score");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_evaluation_score_trx_employee_evaluation_evaluation_id",
                table: "TRX_Evaluation_Score");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_manpower_request_job_descriptions_job_description_id",
                table: "TRX_Manpower_Request");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_manpower_request_mst_employee_replacement_for_employee_id",
                table: "TRX_Manpower_Request");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_manpower_request_mst_employee_report_to_employee_id",
                table: "TRX_Manpower_Request");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_performance_plan_mst_employee_employee_id",
                table: "TRX_Performance_Plan");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_performance_plan_sys_user_created_by_user_id",
                table: "TRX_Performance_Plan");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_personal_action_mst_employee_employee_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_personal_action_mst_employee_new_manager_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_personal_action_ref_grade_new_grade_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_personal_action_ref_job_level_new_job_level_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_personal_action_ref_job_title_new_job_title_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_personal_action_ref_location_new_location_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_personal_action_ref_organization_new_organization_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_plan_task_sys_user_assigned_by_user_id",
                table: "TRX_Plan_Task");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_plan_task_trx_performance_plan_plan_id",
                table: "TRX_Plan_Task");

            migrationBuilder.DropForeignKey(
                name: "fk_trx_plan_work_log_trx_performance_plan_plan_id",
                table: "TRX_Plan_WorkLog");

            migrationBuilder.DropTable(
                name: "BANK_JOBDESC");

            migrationBuilder.DropTable(
                name: "MST_JOBDESC");

            

            migrationBuilder.DropTable(
                name: "TRN_Revise_History_JD");

            

            

            

            

            

            

            

            

            

            migrationBuilder.DropTable(
                name: "TRN_Jobdesc");

            

            

            

            

            migrationBuilder.DropIndex(
                name: "ix_trx_plan_work_log_plan_id",
                table: "TRX_Plan_WorkLog");

            migrationBuilder.DropIndex(
                name: "ix_trx_plan_task_assigned_by_user_id",
                table: "TRX_Plan_Task");

            migrationBuilder.DropIndex(
                name: "ix_trx_plan_task_plan_id",
                table: "TRX_Plan_Task");

            migrationBuilder.DropIndex(
                name: "ix_trx_personal_action_employee_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropIndex(
                name: "ix_trx_personal_action_new_grade_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropIndex(
                name: "ix_trx_personal_action_new_job_level_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropIndex(
                name: "ix_trx_personal_action_new_job_title_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropIndex(
                name: "ix_trx_personal_action_new_location_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropIndex(
                name: "ix_trx_personal_action_new_manager_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropIndex(
                name: "ix_trx_personal_action_new_organization_id",
                table: "TRX_Personal_Action");

            migrationBuilder.DropIndex(
                name: "ix_trx_performance_plan_created_by_user_id",
                table: "TRX_Performance_Plan");

            migrationBuilder.DropIndex(
                name: "ix_trx_performance_plan_employee_id",
                table: "TRX_Performance_Plan");

            migrationBuilder.DropIndex(
                name: "ix_trx_manpower_request_job_description_id",
                table: "TRX_Manpower_Request");

            migrationBuilder.DropIndex(
                name: "ix_trx_manpower_request_replacement_for_employee_id",
                table: "TRX_Manpower_Request");

            migrationBuilder.DropIndex(
                name: "ix_trx_manpower_request_report_to_employee_id",
                table: "TRX_Manpower_Request");

            migrationBuilder.DropIndex(
                name: "ix_trx_evaluation_score_evaluation_id",
                table: "TRX_Evaluation_Score");

            migrationBuilder.DropIndex(
                name: "ix_trx_evaluation_score_scored_by_user_id",
                table: "TRX_Evaluation_Score");

            migrationBuilder.DropIndex(
                name: "ix_trx_evaluation_entry_evaluation_id",
                table: "TRX_Evaluation_Entry");

            migrationBuilder.DropIndex(
                name: "ix_trx_employee_evaluation_employee_id",
                table: "TRX_Employee_Evaluation");

            migrationBuilder.DropIndex(
                name: "ix_trx_employee_evaluation_employment_id",
                table: "TRX_Employee_Evaluation");

            migrationBuilder.DropIndex(
                name: "ix_trx_employee_evaluation_evaluation_type_id",
                table: "TRX_Employee_Evaluation");

            migrationBuilder.DropIndex(
                name: "ix_trx_employee_competency_assigned_by_user_id",
                table: "TRX_Employee_Competency");

            migrationBuilder.DropIndex(
                name: "ix_trx_employee_competency_competency_id",
                table: "TRX_Employee_Competency");

            migrationBuilder.DropIndex(
                name: "ix_trx_employee_competency_employee_id",
                table: "TRX_Employee_Competency");

            migrationBuilder.DropIndex(
                name: "ix_trx_competency_assessment_employee_competency_id",
                table: "TRX_Competency_Assessment");

            migrationBuilder.DropColumn(
                name: "approval_phase",
                table: "TRX_Manpower_Request");

            migrationBuilder.DropColumn(
                name: "business_plan_path",
                table: "TRX_Manpower_Request");

            migrationBuilder.DropColumn(
                name: "job_description_id",
                table: "TRX_Manpower_Request");

            migrationBuilder.DropColumn(
                name: "job_description_path",
                table: "TRX_Manpower_Request");

            migrationBuilder.DropColumn(
                name: "manual_mrf_path",
                table: "TRX_Manpower_Request");

            migrationBuilder.DropColumn(
                name: "replacement_for_employee_id",
                table: "TRX_Manpower_Request");

            migrationBuilder.DropColumn(
                name: "report_to_employee_id",
                table: "TRX_Manpower_Request");

            migrationBuilder.DropColumn(
                name: "request_type",
                table: "TRX_Manpower_Request");

            migrationBuilder.DropColumn(
                name: "work_status",
                table: "TRX_Manpower_Request");

            migrationBuilder.RenameColumn(
                name: "evaluation_type_id",
                table: "TRX_Employee_Evaluation",
                newName: "eval_type_id");

            migrationBuilder.AlterColumn<int>(
                name: "booking_id",
                table: "TRX_Room_Booking",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "manpower_id",
                table: "TRX_Manpower_Request",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "encashment_id",
                table: "TRX_Leave_Encashment",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "balance_id",
                table: "TRX_Leave_Balance",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "claim_id",
                table: "TRX_Health_Claim",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "period_id",
                table: "TRX_Flex_Period",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "work_type_id",
                table: "REF_Work_Type",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "holiday_id",
                table: "REF_Public_Holiday",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "room_id",
                table: "REF_Meeting_Room",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "leave_type_id",
                table: "REF_Leave_Type",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateIndex(
                name: "ix_trx_room_booking_room_id",
                table: "TRX_Room_Booking",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_personal_action_employee_id_status",
                table: "TRX_Personal_Action",
                columns: new[] { "employee_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_trx_leave_encashment_employee_id_status",
                table: "TRX_Leave_Encashment",
                columns: new[] { "employee_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_trx_leave_balance_employee_id",
                table: "TRX_Leave_Balance",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_health_claim_employee_id_status",
                table: "TRX_Health_Claim",
                columns: new[] { "employee_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_trx_employee_evaluation_employee_id_employment_id_eval_type_id",
                table: "TRX_Employee_Evaluation",
                columns: new[] { "employee_id", "employment_id", "eval_type_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_trx_employee_competency_employee_id_competency_id",
                table: "TRX_Employee_Competency",
                columns: new[] { "employee_id", "competency_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_trx_attendance_employee_id",
                table: "TRX_Attendance",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_ref_evaluation_type_name",
                table: "REF_Evaluation_Type",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ref_competency_name",
                table: "REF_Competency",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Competency_Assessment_TRX_Employee_Competency_employee_competency_id",
                table: "TRX_Competency_Assessment",
                column: "employee_competency_id",
                principalTable: "TRX_Employee_Competency",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Employee_Competency_MST_Employee_employee_id",
                table: "TRX_Employee_Competency",
                column: "employee_id",
                principalTable: "MST_Employee",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Employee_Competency_REF_Competency_competency_id",
                table: "TRX_Employee_Competency",
                column: "competency_id",
                principalTable: "REF_Competency",
                principalColumn: "competency_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Employee_Competency_SYS_User_assigned_by_user_id",
                table: "TRX_Employee_Competency",
                column: "assigned_by_user_id",
                principalTable: "SYS_User",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Employee_Evaluation_MST_Employee_Employment_employment_id",
                table: "TRX_Employee_Evaluation",
                column: "employment_id",
                principalTable: "MST_Employee_Employment",
                principalColumn: "employment_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Employee_Evaluation_MST_Employee_employee_id",
                table: "TRX_Employee_Evaluation",
                column: "employee_id",
                principalTable: "MST_Employee",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Employee_Evaluation_REF_Evaluation_Type_eval_type_id",
                table: "TRX_Employee_Evaluation",
                column: "eval_type_id",
                principalTable: "REF_Evaluation_Type",
                principalColumn: "eval_type_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Evaluation_Entry_TRX_Employee_Evaluation_evaluation_id",
                table: "TRX_Evaluation_Entry",
                column: "evaluation_id",
                principalTable: "TRX_Employee_Evaluation",
                principalColumn: "evaluation_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Evaluation_Score_SYS_User_scored_by_user_id",
                table: "TRX_Evaluation_Score",
                column: "scored_by_user_id",
                principalTable: "SYS_User",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Evaluation_Score_TRX_Employee_Evaluation_evaluation_id",
                table: "TRX_Evaluation_Score",
                column: "evaluation_id",
                principalTable: "TRX_Employee_Evaluation",
                principalColumn: "evaluation_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Performance_Plan_MST_Employee_employee_id",
                table: "TRX_Performance_Plan",
                column: "employee_id",
                principalTable: "MST_Employee",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Performance_Plan_SYS_User_created_by_user_id",
                table: "TRX_Performance_Plan",
                column: "created_by_user_id",
                principalTable: "SYS_User",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Personal_Action_MST_Employee_employee_id",
                table: "TRX_Personal_Action",
                column: "employee_id",
                principalTable: "MST_Employee",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Personal_Action_MST_Employee_new_manager_id",
                table: "TRX_Personal_Action",
                column: "new_manager_id",
                principalTable: "MST_Employee",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Personal_Action_REF_Grade_new_grade_id",
                table: "TRX_Personal_Action",
                column: "new_grade_id",
                principalTable: "REF_Grade",
                principalColumn: "grade_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Personal_Action_REF_Job_Level_new_job_level_id",
                table: "TRX_Personal_Action",
                column: "new_job_level_id",
                principalTable: "REF_Job_Level",
                principalColumn: "job_level_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Personal_Action_REF_Job_Title_new_job_title_id",
                table: "TRX_Personal_Action",
                column: "new_job_title_id",
                principalTable: "REF_Job_Title",
                principalColumn: "job_title_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Personal_Action_REF_Location_new_location_id",
                table: "TRX_Personal_Action",
                column: "new_location_id",
                principalTable: "REF_Location",
                principalColumn: "location_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Personal_Action_REF_Organization_new_organization_id",
                table: "TRX_Personal_Action",
                column: "new_organization_id",
                principalTable: "REF_Organization",
                principalColumn: "organization_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Plan_Task_SYS_User_assigned_by_user_id",
                table: "TRX_Plan_Task",
                column: "assigned_by_user_id",
                principalTable: "SYS_User",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Plan_Task_TRX_Performance_Plan_plan_id",
                table: "TRX_Plan_Task",
                column: "plan_id",
                principalTable: "TRX_Performance_Plan",
                principalColumn: "plan_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TRX_Plan_WorkLog_TRX_Performance_Plan_plan_id",
                table: "TRX_Plan_WorkLog",
                column: "plan_id",
                principalTable: "TRX_Performance_Plan",
                principalColumn: "plan_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
