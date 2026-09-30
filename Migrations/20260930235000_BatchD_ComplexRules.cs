using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRIS.Api.Migrations;

public partial class BatchD_ComplexRules : Migration
{
    protected override void Up(MigrationBuilder m)
    {
        m.CreateTable(name:"REF_Work_Type", columns:t=>new
        {
            work_type_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),
            name=t.Column<string>(type:"TEXT",maxLength:50,nullable:false)
        }, constraints:t=>t.PrimaryKey("PK_REF_Work_Type",x=>x.work_type_id));

        m.CreateTable(name:"REF_Public_Holiday", columns:t=>new
        {
            holiday_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),
            date=t.Column<DateOnly>(type:"TEXT",nullable:false),
            name=t.Column<string>(type:"TEXT",maxLength:150,nullable:false)
        }, constraints:t=>t.PrimaryKey("PK_REF_Public_Holiday",x=>x.holiday_id));

        m.CreateTable(name:"REF_Leave_Type", columns:t=>new
        {
            leave_type_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),
            name=t.Column<string>(type:"TEXT",maxLength:80,nullable:false),
            is_sellable=t.Column<bool>(type:"INTEGER",nullable:false)
        }, constraints:t=>t.PrimaryKey("PK_REF_Leave_Type",x=>x.leave_type_id));

        m.InsertData(table:"REF_Work_Type", columns:new[]{"work_type_id","name"}, values:new object[,]{{1,"WFO"},{2,"WFH"},{3,"WFH with Note"}});
        m.InsertData(table:"REF_Leave_Type", columns:new[]{"leave_type_id","name","is_sellable"}, values:new object[,]{{1,"Annual",true},{2,"Sick",false},{3,"Other",false}});

        m.CreateTable(name:"REF_Meeting_Room", columns:t=>new
        {
            room_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),
            name=t.Column<string>(type:"TEXT",maxLength:120,nullable:false),
            capacity=t.Column<int>(type:"INTEGER",nullable:false),
            location_id=t.Column<int>(type:"INTEGER",nullable:false),
            is_active=t.Column<bool>(type:"INTEGER",nullable:false)
        }, constraints:t=>{
            t.PrimaryKey("PK_REF_Meeting_Room",x=>x.room_id);
            t.ForeignKey("FK_REF_Meeting_Room_REF_Location_location_id",x=>x.location_id,"REF_Location","location_id",onDelete:ReferentialAction.Restrict);
        });

        m.CreateTable(name:"TRX_Flex_Period", columns:t=>new
        {
            period_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),
            name=t.Column<string>(type:"TEXT",maxLength:120,nullable:false),
            start_date=t.Column<DateOnly>(type:"TEXT",nullable:false),
            end_date=t.Column<DateOnly>(type:"TEXT",nullable:false),
            is_open=t.Column<bool>(type:"INTEGER",nullable:false),
            created_at=t.Column<DateTime>(type:"TEXT",nullable:false),
            created_by=t.Column<int>(type:"INTEGER",nullable:true),
            updated_at=t.Column<DateTime>(type:"TEXT",nullable:true),
            updated_by=t.Column<int>(type:"INTEGER",nullable:true)
        }, constraints:t=>t.PrimaryKey("PK_TRX_Flex_Period",x=>x.period_id));

        m.CreateTable(name:"TRX_Leave_Balance", columns:t=>new
        {
            balance_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),
            employee_id=t.Column<int>(type:"INTEGER",nullable:false),
            leave_type_id=t.Column<int>(type:"INTEGER",nullable:false),
            year=t.Column<int>(type:"INTEGER",nullable:false),
            entitlement=t.Column<int>(type:"INTEGER",nullable:false),
            used=t.Column<int>(type:"INTEGER",nullable:false),
            sold=t.Column<int>(type:"INTEGER",nullable:false),
            carried_over=t.Column<int>(type:"INTEGER",nullable:false),
            created_at=t.Column<DateTime>(type:"TEXT",nullable:false),
            created_by=t.Column<int>(type:"INTEGER",nullable:true),
            updated_at=t.Column<DateTime>(type:"TEXT",nullable:true),
            updated_by=t.Column<int>(type:"INTEGER",nullable:true)
        }, constraints:t=>{
            t.PrimaryKey("PK_TRX_Leave_Balance",x=>x.balance_id);
            t.ForeignKey("FK_TRX_Leave_Balance_MST_Employee_employee_id",x=>x.employee_id,"MST_Employee","employee_id",onDelete:ReferentialAction.Cascade);
            t.ForeignKey("FK_TRX_Leave_Balance_REF_Leave_Type_leave_type_id",x=>x.leave_type_id,"REF_Leave_Type","leave_type_id",onDelete:ReferentialAction.Restrict);
        });

        m.CreateTable(name:"TRX_Leave_Encashment", columns:t=>new
        {
            encashment_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),
            employee_id=t.Column<int>(type:"INTEGER",nullable:false),
            period_id=t.Column<int>(type:"INTEGER",nullable:false),
            leave_type_id=t.Column<int>(type:"INTEGER",nullable:false),
            days=t.Column<int>(type:"INTEGER",nullable:false),
            status=t.Column<string>(type:"TEXT",maxLength:30,nullable:false),
            created_at=t.Column<DateTime>(type:"TEXT",nullable:false),
            created_by=t.Column<int>(type:"INTEGER",nullable:true),
            updated_at=t.Column<DateTime>(type:"TEXT",nullable:true),
            updated_by=t.Column<int>(type:"INTEGER",nullable:true)
        }, constraints:t=>{
            t.PrimaryKey("PK_TRX_Leave_Encashment",x=>x.encashment_id);
            t.ForeignKey("FK_TRX_Leave_Encashment_MST_Employee_employee_id",x=>x.employee_id,"MST_Employee","employee_id",onDelete:ReferentialAction.Restrict);
            t.ForeignKey("FK_TRX_Leave_Encashment_TRX_Flex_Period_period_id",x=>x.period_id,"TRX_Flex_Period","period_id",onDelete:ReferentialAction.Restrict);
            t.ForeignKey("FK_TRX_Leave_Encashment_REF_Leave_Type_leave_type_id",x=>x.leave_type_id,"REF_Leave_Type","leave_type_id",onDelete:ReferentialAction.Restrict);
        });

        m.CreateTable(name:"TRX_Health_Claim", columns:t=>new
        {
            claim_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),
            employee_id=t.Column<int>(type:"INTEGER",nullable:false),
            period_id=t.Column<int>(type:"INTEGER",nullable:false),
            amount=t.Column<long>(type:"INTEGER",nullable:false),
            description=t.Column<string>(type:"TEXT",maxLength:500,nullable:true),
            status=t.Column<string>(type:"TEXT",maxLength:30,nullable:false),
            created_at=t.Column<DateTime>(type:"TEXT",nullable:false),
            created_by=t.Column<int>(type:"INTEGER",nullable:true),
            updated_at=t.Column<DateTime>(type:"TEXT",nullable:true),
            updated_by=t.Column<int>(type:"INTEGER",nullable:true)
        }, constraints:t=>{
            t.PrimaryKey("PK_TRX_Health_Claim",x=>x.claim_id);
            t.ForeignKey("FK_TRX_Health_Claim_MST_Employee_employee_id",x=>x.employee_id,"MST_Employee","employee_id",onDelete:ReferentialAction.Restrict);
            t.ForeignKey("FK_TRX_Health_Claim_TRX_Flex_Period_period_id",x=>x.period_id,"TRX_Flex_Period","period_id",onDelete:ReferentialAction.Restrict);
        });

        m.CreateTable(name:"TRX_Room_Booking", columns:t=>new
        {
            booking_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),
            room_id=t.Column<int>(type:"INTEGER",nullable:false),
            booked_by=t.Column<int>(type:"INTEGER",nullable:false),
            title=t.Column<string>(type:"TEXT",maxLength:200,nullable:false),
            start_time=t.Column<DateTime>(type:"TEXT",nullable:false),
            end_time=t.Column<DateTime>(type:"TEXT",nullable:false),
            status=t.Column<string>(type:"TEXT",maxLength:30,nullable:false),
            is_priority=t.Column<bool>(type:"INTEGER",nullable:false),
            created_at=t.Column<DateTime>(type:"TEXT",nullable:false),
            created_by=t.Column<int>(type:"INTEGER",nullable:true),
            updated_at=t.Column<DateTime>(type:"TEXT",nullable:true),
            updated_by=t.Column<int>(type:"INTEGER",nullable:true)
        }, constraints:t=>{
            t.PrimaryKey("PK_TRX_Room_Booking",x=>x.booking_id);
            t.ForeignKey("FK_TRX_Room_Booking_REF_Meeting_Room_room_id",x=>x.room_id,"REF_Meeting_Room","room_id",onDelete:ReferentialAction.Restrict);
            t.ForeignKey("FK_TRX_Room_Booking_MST_Employee_booked_by",x=>x.booked_by,"MST_Employee","employee_id",onDelete:ReferentialAction.Restrict);
        });

        m.CreateTable(name:"TRX_Manpower_Request", columns:t=>new
        {
            manpower_id=t.Column<int>(type:"INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),
            requested_by=t.Column<int>(type:"INTEGER",nullable:false),
            organization_id=t.Column<int>(type:"INTEGER",nullable:false),
            job_title_id=t.Column<int>(type:"INTEGER",nullable:false),
            job_level_id=t.Column<int>(type:"INTEGER",nullable:false),
            employment_type_id=t.Column<int>(type:"INTEGER",nullable:false),
            headcount=t.Column<int>(type:"INTEGER",nullable:false),
            reason=t.Column<string>(type:"TEXT",maxLength:1000,nullable:false),
            target_date=t.Column<DateOnly>(type:"TEXT",nullable:false),
            status=t.Column<string>(type:"TEXT",maxLength:30,nullable:false),
            created_at=t.Column<DateTime>(type:"TEXT",nullable:false),
            created_by=t.Column<int>(type:"INTEGER",nullable:true),
            updated_at=t.Column<DateTime>(type:"TEXT",nullable:true),
            updated_by=t.Column<int>(type:"INTEGER",nullable:true)
        }, constraints:t=>{
            t.PrimaryKey("PK_TRX_Manpower_Request",x=>x.manpower_id);
            t.ForeignKey("FK_TRX_Manpower_Request_MST_Employee_requested_by",x=>x.requested_by,"MST_Employee","employee_id",onDelete:ReferentialAction.Restrict);
            t.ForeignKey("FK_TRX_Manpower_Request_REF_Organization_organization_id",x=>x.organization_id,"REF_Organization","organization_id",onDelete:ReferentialAction.Restrict);
            t.ForeignKey("FK_TRX_Manpower_Request_REF_Job_Title_job_title_id",x=>x.job_title_id,"REF_Job_Title","job_title_id",onDelete:ReferentialAction.Restrict);
            t.ForeignKey("FK_TRX_Manpower_Request_REF_Job_Level_job_level_id",x=>x.job_level_id,"REF_Job_Level","job_level_id",onDelete:ReferentialAction.Restrict);
            t.ForeignKey("FK_TRX_Manpower_Request_REF_Employment_Type_employment_type_id",x=>x.employment_type_id,"REF_Employment_Type","employment_type_id",onDelete:ReferentialAction.Restrict);
        });

        m.AddColumn<int>(name:"work_type_id",table:"TRX_Attendance",type:"INTEGER",nullable:false,defaultValue:1);
        m.AddColumn<string>(name:"note",table:"TRX_Attendance",type:"TEXT",nullable:true);
        m.AddColumn<int>(name:"leave_type_id",table:"TRX_Leave_Request",type:"INTEGER",nullable:false,defaultValue:1);

        m.CreateIndex(name:"IX_REF_Work_Type_name",table:"REF_Work_Type",column:"name",unique:true);
        m.CreateIndex(name:"IX_REF_Public_Holiday_date",table:"REF_Public_Holiday",column:"date",unique:true);
        m.CreateIndex(name:"IX_REF_Leave_Type_name",table:"REF_Leave_Type",column:"name",unique:true);
        m.CreateIndex(name:"IX_REF_Meeting_Room_name",table:"REF_Meeting_Room",column:"name",unique:true);
        m.CreateIndex(name:"IX_REF_Meeting_Room_location_id",table:"REF_Meeting_Room",column:"location_id");
        m.CreateIndex(name:"IX_TRX_Leave_Balance_employee_id",table:"TRX_Leave_Balance",column:"employee_id");
        m.CreateIndex(name:"IX_TRX_Leave_Balance_leave_type_id",table:"TRX_Leave_Balance",column:"leave_type_id");
        m.CreateIndex(name:"IX_TRX_Leave_Balance_employee_id_leave_type_id_year",table:"TRX_Leave_Balance",columns:new[]{"employee_id","leave_type_id","year"},unique:true);
        m.CreateIndex(name:"IX_TRX_Leave_Encashment_employee_id",table:"TRX_Leave_Encashment",column:"employee_id");
        m.CreateIndex(name:"IX_TRX_Leave_Encashment_period_id",table:"TRX_Leave_Encashment",column:"period_id");
        m.CreateIndex(name:"IX_TRX_Leave_Encashment_leave_type_id",table:"TRX_Leave_Encashment",column:"leave_type_id");
        m.CreateIndex(name:"IX_TRX_Health_Claim_employee_id",table:"TRX_Health_Claim",column:"employee_id");
        m.CreateIndex(name:"IX_TRX_Health_Claim_period_id",table:"TRX_Health_Claim",column:"period_id");
        m.CreateIndex(name:"IX_TRX_Room_Booking_room_id",table:"TRX_Room_Booking",column:"room_id");
        m.CreateIndex(name:"IX_TRX_Room_Booking_booked_by_employee_id",table:"TRX_Room_Booking",column:"booked_by");
        m.CreateIndex(name:"IX_TRX_Room_Booking_room_id_start_time_end_time",table:"TRX_Room_Booking",columns:new[]{"room_id","start_time","end_time"});
        m.CreateIndex(name:"IX_TRX_Manpower_Request_requested_by_employee_id",table:"TRX_Manpower_Request",column:"requested_by");
        m.CreateIndex(name:"IX_TRX_Manpower_Request_organization_id",table:"TRX_Manpower_Request",column:"organization_id");
        m.CreateIndex(name:"IX_TRX_Manpower_Request_job_title_id",table:"TRX_Manpower_Request",column:"job_title_id");
        m.CreateIndex(name:"IX_TRX_Manpower_Request_job_level_id",table:"TRX_Manpower_Request",column:"job_level_id");
        m.CreateIndex(name:"IX_TRX_Manpower_Request_employment_type_id",table:"TRX_Manpower_Request",column:"employment_type_id");
        m.CreateIndex(name:"IX_TRX_Attendance_employee_id_date",table:"TRX_Attendance",columns:new[]{"employee_id","date"},unique:true);
        m.CreateIndex(name:"IX_TRX_Leave_Request_leave_type_id",table:"TRX_Leave_Request",column:"leave_type_id");

        m.CreateIndex(name:"IX_TRX_Leave_Encashment_employee_id_status",table:"TRX_Leave_Encashment",columns:new[]{"employee_id","status"});
        m.CreateIndex(name:"IX_TRX_Health_Claim_employee_id_status",table:"TRX_Health_Claim",columns:new[]{"employee_id","status"});
        m.AddForeignKey(name:"FK_TRX_Attendance_REF_Work_Type_work_type_id",table:"TRX_Attendance",column:"work_type_id",principalTable:"REF_Work_Type",principalColumn:"work_type_id",onDelete:ReferentialAction.Restrict);
        m.AddForeignKey(name:"FK_TRX_Leave_Request_REF_Leave_Type_leave_type_id",table:"TRX_Leave_Request",column:"leave_type_id",principalTable:"REF_Leave_Type",principalColumn:"leave_type_id",onDelete:ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder m)
    {
        m.DropForeignKey("FK_TRX_Attendance_REF_Work_Type_work_type_id","TRX_Attendance");
        m.DropForeignKey("FK_TRX_Leave_Request_REF_Leave_Type_leave_type_id","TRX_Leave_Request");
        m.DropColumn("work_type_id","TRX_Attendance");
        m.DropColumn("note","TRX_Attendance");
        m.DropColumn("leave_type_id","TRX_Leave_Request");
        m.DropTable("TRX_Manpower_Request");
        m.DropTable("TRX_Room_Booking");
        m.DropTable("TRX_Health_Claim");
        m.DropTable("TRX_Leave_Encashment");
        m.DropTable("TRX_Leave_Balance");
        m.DropTable("TRX_Flex_Period");
        m.DropTable("REF_Meeting_Room");
        m.DropTable("REF_Leave_Type");
        m.DropTable("REF_Public_Holiday");
        m.DropTable("REF_Work_Type");
    }
}
