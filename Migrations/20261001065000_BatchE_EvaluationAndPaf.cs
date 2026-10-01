using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace HRIS.Api.Migrations;
public partial class BatchE_EvaluationAndPaf : Migration
{
 protected override void Up(MigrationBuilder m)
 {
  m.CreateTable(name:"REF_Evaluation_Type",columns:t=>new{
   eval_type_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),
   name=t.Column<string>(type:"TEXT",maxLength:100,nullable:false),
   month_offset=t.Column<int>(type:"INTEGER",nullable:true)
  },constraints:t=>t.PrimaryKey("PK_REF_Evaluation_Type",x=>x.eval_type_id));
  m.CreateTable(name:"REF_Competency",columns:t=>new{
   competency_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),
   name=t.Column<string>(type:"TEXT",maxLength:150,nullable:false),
   description=t.Column<string>(type:"TEXT",maxLength:1000,nullable:true)
  },constraints:t=>t.PrimaryKey("PK_REF_Competency",x=>x.competency_id));

  m.CreateTable(name:"TRX_Employee_Evaluation",columns:t=>new{
   evaluation_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),
   employee_id=t.Column<int>(type:"INTEGER",nullable:false),
   employment_id=t.Column<int>(type:"INTEGER",nullable:false),
   eval_type_id=t.Column<int>(type:"INTEGER",nullable:false),
   due_date=t.Column<DateOnly>(type:"TEXT",nullable:false),
   status=t.Column<string>(type:"TEXT",maxLength:30,nullable:false),
   completed_at=t.Column<DateTime>(type:"TEXT",nullable:true),
   created_at=t.Column<DateTime>(type:"TEXT",nullable:false),
   created_by=t.Column<int>(type:"INTEGER",nullable:true),
   updated_at=t.Column<DateTime>(type:"TEXT",nullable:true),
   updated_by=t.Column<int>(type:"INTEGER",nullable:true)
  },constraints:t=>{t.PrimaryKey("PK_TRX_Employee_Evaluation",x=>x.evaluation_id);
   t.ForeignKey("FK_TRX_Employee_Evaluation_MST_Employee_employee_id",x=>x.employee_id,"MST_Employee","employee_id",onDelete:ReferentialAction.Restrict);
   t.ForeignKey("FK_TRX_Employee_Evaluation_MST_Employee_Employment_employment_id",x=>x.employment_id,"MST_Employee_Employment","employment_id",onDelete:ReferentialAction.Restrict);
   t.ForeignKey("FK_TRX_Employee_Evaluation_REF_Evaluation_Type_eval_type_id",x=>x.eval_type_id,"REF_Evaluation_Type","eval_type_id",onDelete:ReferentialAction.Restrict);});
  m.CreateTable(name:"TRX_Performance_Plan",columns:t=>new{
   plan_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),
   employee_id=t.Column<int>(type:"INTEGER",nullable:false),start_date=t.Column<DateOnly>(type:"TEXT",nullable:false),end_date=t.Column<DateOnly>(type:"TEXT",nullable:false),
   status=t.Column<string>(type:"TEXT",maxLength:30,nullable:false),created_by_user_id=t.Column<int>(type:"INTEGER",nullable:false),
   created_at=t.Column<DateTime>(type:"TEXT",nullable:false),created_by=t.Column<int>(type:"INTEGER",nullable:true),updated_at=t.Column<DateTime>(type:"TEXT",nullable:true),updated_by=t.Column<int>(type:"INTEGER",nullable:true)
  },constraints:t=>{t.PrimaryKey("PK_TRX_Performance_Plan",x=>x.plan_id);t.ForeignKey("FK_TRX_Performance_Plan_MST_Employee_employee_id",x=>x.employee_id,"MST_Employee","employee_id",onDelete:ReferentialAction.Restrict);t.ForeignKey("FK_TRX_Performance_Plan_SYS_User_created_by_user_id",x=>x.created_by_user_id,"SYS_User","user_id",onDelete:ReferentialAction.Restrict);});
  m.CreateTable(name:"TRX_Employee_Competency",columns:t=>new{
   id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),employee_id=t.Column<int>(type:"INTEGER",nullable:false),competency_id=t.Column<int>(type:"INTEGER",nullable:false),target_level=t.Column<int>(type:"INTEGER",nullable:false),assigned_by_user_id=t.Column<int>(type:"INTEGER",nullable:false),
   created_at=t.Column<DateTime>(type:"TEXT",nullable:false),created_by=t.Column<int>(type:"INTEGER",nullable:true),updated_at=t.Column<DateTime>(type:"TEXT",nullable:true),updated_by=t.Column<int>(type:"INTEGER",nullable:true)
  },constraints:t=>{t.PrimaryKey("PK_TRX_Employee_Competency",x=>x.id);t.ForeignKey("FK_TRX_Employee_Competency_MST_Employee_employee_id",x=>x.employee_id,"MST_Employee","employee_id",onDelete:ReferentialAction.Restrict);t.ForeignKey("FK_TRX_Employee_Competency_REF_Competency_competency_id",x=>x.competency_id,"REF_Competency","competency_id",onDelete:ReferentialAction.Restrict);t.ForeignKey("FK_TRX_Employee_Competency_SYS_User_assigned_by_user_id",x=>x.assigned_by_user_id,"SYS_User","user_id",onDelete:ReferentialAction.Restrict);});
  m.CreateTable(name:"TRX_Personal_Action",columns:t=>new{
   paf_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),employee_id=t.Column<int>(type:"INTEGER",nullable:false),action_type=t.Column<string>(type:"TEXT",maxLength:30,nullable:false),effective_date=t.Column<DateOnly>(type:"TEXT",nullable:false),reason=t.Column<string>(type:"TEXT",maxLength:1000,nullable:false),status=t.Column<string>(type:"TEXT",maxLength:30,nullable:false),
   new_organization_id=t.Column<int>(type:"INTEGER",nullable:true),new_job_title_id=t.Column<int>(type:"INTEGER",nullable:true),new_job_level_id=t.Column<int>(type:"INTEGER",nullable:true),new_grade_id=t.Column<int>(type:"INTEGER",nullable:true),new_location_id=t.Column<int>(type:"INTEGER",nullable:true),new_manager_id=t.Column<int>(type:"INTEGER",nullable:true),
   created_at=t.Column<DateTime>(type:"TEXT",nullable:false),created_by=t.Column<int>(type:"INTEGER",nullable:true),updated_at=t.Column<DateTime>(type:"TEXT",nullable:true),updated_by=t.Column<int>(type:"INTEGER",nullable:true)
  },constraints:t=>{t.PrimaryKey("PK_TRX_Personal_Action",x=>x.paf_id);t.ForeignKey("FK_TRX_Personal_Action_MST_Employee_employee_id",x=>x.employee_id,"MST_Employee","employee_id",onDelete:ReferentialAction.Restrict);t.ForeignKey("FK_TRX_Personal_Action_REF_Organization_new_organization_id",x=>x.new_organization_id,"REF_Organization","organization_id",onDelete:ReferentialAction.Restrict);t.ForeignKey("FK_TRX_Personal_Action_REF_Job_Title_new_job_title_id",x=>x.new_job_title_id,"REF_Job_Title","job_title_id",onDelete:ReferentialAction.Restrict);t.ForeignKey("FK_TRX_Personal_Action_REF_Job_Level_new_job_level_id",x=>x.new_job_level_id,"REF_Job_Level","job_level_id",onDelete:ReferentialAction.Restrict);t.ForeignKey("FK_TRX_Personal_Action_REF_Grade_new_grade_id",x=>x.new_grade_id,"REF_Grade","grade_id",onDelete:ReferentialAction.Restrict);t.ForeignKey("FK_TRX_Personal_Action_REF_Location_new_location_id",x=>x.new_location_id,"REF_Location","location_id",onDelete:ReferentialAction.Restrict);t.ForeignKey("FK_TRX_Personal_Action_MST_Employee_new_manager_id",x=>x.new_manager_id,"MST_Employee","employee_id",onDelete:ReferentialAction.Restrict);});

  m.CreateTable(name:"TRX_Evaluation_Entry",columns:t=>new{
   entry_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),evaluation_id=t.Column<int>(type:"INTEGER",nullable:false),work_description=t.Column<string>(type:"TEXT",maxLength:2000,nullable:false),
   created_at=t.Column<DateTime>(type:"TEXT",nullable:false),created_by=t.Column<int>(type:"INTEGER",nullable:true),updated_at=t.Column<DateTime>(type:"TEXT",nullable:true),updated_by=t.Column<int>(type:"INTEGER",nullable:true)
  },constraints:t=>{t.PrimaryKey("PK_TRX_Evaluation_Entry",x=>x.entry_id);t.ForeignKey("FK_TRX_Evaluation_Entry_TRX_Employee_Evaluation_evaluation_id",x=>x.evaluation_id,"TRX_Employee_Evaluation","evaluation_id",onDelete:ReferentialAction.Cascade);});
  m.CreateTable(name:"TRX_Evaluation_Score",columns:t=>new{
   score_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),evaluation_id=t.Column<int>(type:"INTEGER",nullable:false),scored_by_user_id=t.Column<int>(type:"INTEGER",nullable:false),score=t.Column<int>(type:"INTEGER",nullable:false),note=t.Column<string>(type:"TEXT",maxLength:1000,nullable:true),scored_at=t.Column<DateTime>(type:"TEXT",nullable:false),
   created_at=t.Column<DateTime>(type:"TEXT",nullable:false),created_by=t.Column<int>(type:"INTEGER",nullable:true),updated_at=t.Column<DateTime>(type:"TEXT",nullable:true),updated_by=t.Column<int>(type:"INTEGER",nullable:true)
  },constraints:t=>{t.PrimaryKey("PK_TRX_Evaluation_Score",x=>x.score_id);t.ForeignKey("FK_TRX_Evaluation_Score_TRX_Employee_Evaluation_evaluation_id",x=>x.evaluation_id,"TRX_Employee_Evaluation","evaluation_id",onDelete:ReferentialAction.Cascade);t.ForeignKey("FK_TRX_Evaluation_Score_SYS_User_scored_by_user_id",x=>x.scored_by_user_id,"SYS_User","user_id",onDelete:ReferentialAction.Restrict);});
  m.CreateTable(name:"TRX_Plan_Task",columns:t=>new{
   task_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),plan_id=t.Column<int>(type:"INTEGER",nullable:false),title=t.Column<string>(type:"TEXT",maxLength:250,nullable:false),assigned_by_user_id=t.Column<int>(type:"INTEGER",nullable:false),due_date=t.Column<DateOnly>(type:"TEXT",nullable:true),status=t.Column<string>(type:"TEXT",maxLength:30,nullable:false),
   created_at=t.Column<DateTime>(type:"TEXT",nullable:false),created_by=t.Column<int>(type:"INTEGER",nullable:true),updated_at=t.Column<DateTime>(type:"TEXT",nullable:true),updated_by=t.Column<int>(type:"INTEGER",nullable:true)
  },constraints:t=>{t.PrimaryKey("PK_TRX_Plan_Task",x=>x.task_id);t.ForeignKey("FK_TRX_Plan_Task_TRX_Performance_Plan_plan_id",x=>x.plan_id,"TRX_Performance_Plan","plan_id",onDelete:ReferentialAction.Cascade);t.ForeignKey("FK_TRX_Plan_Task_SYS_User_assigned_by_user_id",x=>x.assigned_by_user_id,"SYS_User","user_id",onDelete:ReferentialAction.Restrict);});
  m.CreateTable(name:"TRX_Plan_WorkLog",columns:t=>new{
   log_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),plan_id=t.Column<int>(type:"INTEGER",nullable:false),description=t.Column<string>(type:"TEXT",maxLength:2000,nullable:false),logged_at=t.Column<DateTime>(type:"TEXT",nullable:false),
   created_at=t.Column<DateTime>(type:"TEXT",nullable:false),created_by=t.Column<int>(type:"INTEGER",nullable:true),updated_at=t.Column<DateTime>(type:"TEXT",nullable:true),updated_by=t.Column<int>(type:"INTEGER",nullable:true)
  },constraints:t=>{t.PrimaryKey("PK_TRX_Plan_WorkLog",x=>x.log_id);t.ForeignKey("FK_TRX_Plan_WorkLog_TRX_Performance_Plan_plan_id",x=>x.plan_id,"TRX_Performance_Plan","plan_id",onDelete:ReferentialAction.Cascade);});
  m.CreateTable(name:"TRX_Competency_Assessment",columns:t=>new{
   assessment_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),employee_competency_id=t.Column<int>(type:"INTEGER",nullable:false),self_level=t.Column<int>(type:"INTEGER",nullable:false),note=t.Column<string>(type:"TEXT",maxLength:1000,nullable:true),assessed_at=t.Column<DateTime>(type:"TEXT",nullable:false),
   created_at=t.Column<DateTime>(type:"TEXT",nullable:false),created_by=t.Column<int>(type:"INTEGER",nullable:true),updated_at=t.Column<DateTime>(type:"TEXT",nullable:true),updated_by=t.Column<int>(type:"INTEGER",nullable:true)
  },constraints:t=>{t.PrimaryKey("PK_TRX_Competency_Assessment",x=>x.assessment_id);t.ForeignKey("FK_TRX_Competency_Assessment_TRX_Employee_Competency_employee_competency_id",x=>x.employee_competency_id,"TRX_Employee_Competency","id",onDelete:ReferentialAction.Cascade);});

  m.CreateIndex(name:"IX_REF_Evaluation_Type_name",table:"REF_Evaluation_Type",column:"name",unique:true);
  m.CreateIndex(name:"IX_REF_Competency_name",table:"REF_Competency",column:"name",unique:true);
  m.CreateIndex(name:"IX_TRX_Employee_Evaluation_employee_id_employment_id_eval_type_id",table:"TRX_Employee_Evaluation",columns:new[]{"employee_id","employment_id","eval_type_id"},unique:true);
  m.CreateIndex(name:"IX_TRX_Employee_Competency_employee_id_competency_id",table:"TRX_Employee_Competency",columns:new[]{"employee_id","competency_id"},unique:true);
  m.CreateIndex(name:"IX_TRX_Personal_Action_employee_id_status",table:"TRX_Personal_Action",columns:new[]{"employee_id","status"});
 }
 protected override void Down(MigrationBuilder m)
 {
  m.DropTable("TRX_Competency_Assessment");m.DropTable("TRX_Evaluation_Score");m.DropTable("TRX_Evaluation_Entry");m.DropTable("TRX_Plan_WorkLog");m.DropTable("TRX_Plan_Task");m.DropTable("TRX_Personal_Action");m.DropTable("TRX_Employee_Competency");m.DropTable("TRX_Performance_Plan");m.DropTable("TRX_Employee_Evaluation");m.DropTable("REF_Competency");m.DropTable("REF_Evaluation_Type");
 }
}
