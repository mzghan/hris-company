using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRIS.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "REF_Account_Type",
                columns: table => new
                {
                    account_type_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    account_type_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_account_type", x => x.account_type_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Address_Type",
                columns: table => new
                {
                    address_type_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    address_type_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_address_type", x => x.address_type_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Contact_Type",
                columns: table => new
                {
                    contact_type_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    contact_type_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_contact_type", x => x.contact_type_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Country",
                columns: table => new
                {
                    country_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    country_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    country_code = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_country", x => x.country_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Education_Degree",
                columns: table => new
                {
                    degree_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    degree_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    degree_description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_education_degree", x => x.degree_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Education_Title",
                columns: table => new
                {
                    title_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    title_name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    major_name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_education_title", x => x.title_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Employment_Status",
                columns: table => new
                {
                    employment_status_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    employment_status_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_employment_status", x => x.employment_status_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Employment_Type",
                columns: table => new
                {
                    employment_type_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    employment_type_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    employment_type_description = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_employment_type", x => x.employment_type_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_End_Reason",
                columns: table => new
                {
                    end_reason_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    end_reason_name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_end_reason", x => x.end_reason_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Gender",
                columns: table => new
                {
                    gender_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    gender_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_gender", x => x.gender_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Grade",
                columns: table => new
                {
                    grade_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    grade_description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    grade_level = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_grade", x => x.grade_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Hierarchy_Type",
                columns: table => new
                {
                    hierarchy_type_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    hierarchy_type_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_hierarchy_type", x => x.hierarchy_type_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Identity_Type",
                columns: table => new
                {
                    identity_type_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    identity_type_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_identity_type", x => x.identity_type_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Industry",
                columns: table => new
                {
                    industry_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    industry_name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_industry", x => x.industry_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Job_Level",
                columns: table => new
                {
                    job_level_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    job_level_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    level_order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_job_level", x => x.job_level_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Job_Title",
                columns: table => new
                {
                    job_title_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    job_title = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_job_title", x => x.job_title_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Location",
                columns: table => new
                {
                    location_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    location_name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_location", x => x.location_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Marital_Status",
                columns: table => new
                {
                    marital_status_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    marital_status_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_marital_status", x => x.marital_status_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Organization",
                columns: table => new
                {
                    organization_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    organization_name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    parent_id = table.Column<int>(type: "INTEGER", nullable: true),
                    organization_level = table.Column<int>(type: "INTEGER", nullable: false),
                    path = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_organization", x => x.organization_id);
                    table.ForeignKey(
                        name: "fk_ref_organization_ref_organization_parent_id",
                        column: x => x.parent_id,
                        principalTable: "REF_Organization",
                        principalColumn: "organization_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "REF_Relationship",
                columns: table => new
                {
                    relationship_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    relationship_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_relationship", x => x.relationship_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Religion",
                columns: table => new
                {
                    religion_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    religion_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_religion", x => x.religion_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_University",
                columns: table => new
                {
                    university_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    university_name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_university", x => x.university_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Vendor",
                columns: table => new
                {
                    vendor_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    vendor_name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_vendor", x => x.vendor_id);
                });

            migrationBuilder.CreateTable(
                name: "SYS_Role",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    role_name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sys_role", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "TRX_Kpi_Criteria",
                columns: table => new
                {
                    criteria_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    weight = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trx_kpi_criteria", x => x.criteria_id);
                });

            migrationBuilder.CreateTable(
                name: "TRX_Kpi_Period",
                columns: table => new
                {
                    kpi_period_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    year = table.Column<int>(type: "INTEGER", nullable: false),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trx_kpi_period", x => x.kpi_period_id);
                });

            migrationBuilder.CreateTable(
                name: "REF_Province",
                columns: table => new
                {
                    province_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    province_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    country_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_province", x => x.province_id);
                    table.ForeignKey(
                        name: "fk_ref_province_ref_country_country_id",
                        column: x => x.country_id,
                        principalTable: "REF_Country",
                        principalColumn: "country_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MST_Employee",
                columns: table => new
                {
                    employee_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    employee_number = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    full_name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    join_date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    nationality_country_id = table.Column<int>(type: "INTEGER", nullable: true),
                    religion_id = table.Column<int>(type: "INTEGER", nullable: true),
                    gender_id = table.Column<int>(type: "INTEGER", nullable: true),
                    marital_status_id = table.Column<int>(type: "INTEGER", nullable: true),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mst_employee", x => x.employee_id);
                    table.ForeignKey(
                        name: "fk_mst_employee_ref_country_nationality_country_id",
                        column: x => x.nationality_country_id,
                        principalTable: "REF_Country",
                        principalColumn: "country_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_ref_gender_gender_id",
                        column: x => x.gender_id,
                        principalTable: "REF_Gender",
                        principalColumn: "gender_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_ref_marital_status_marital_status_id",
                        column: x => x.marital_status_id,
                        principalTable: "REF_Marital_Status",
                        principalColumn: "marital_status_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_ref_religion_religion_id",
                        column: x => x.religion_id,
                        principalTable: "REF_Religion",
                        principalColumn: "religion_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "REF_City",
                columns: table => new
                {
                    city_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    city_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    province_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ref_city", x => x.city_id);
                    table.ForeignKey(
                        name: "fk_ref_city_ref_province_province_id",
                        column: x => x.province_id,
                        principalTable: "REF_Province",
                        principalColumn: "province_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MST_Employee_Contact",
                columns: table => new
                {
                    contact_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    employee_id = table.Column<int>(type: "INTEGER", nullable: false),
                    contact_type_id = table.Column<int>(type: "INTEGER", nullable: false),
                    contact_value = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    is_primary = table.Column<bool>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mst_employee_contact", x => x.contact_id);
                    table.ForeignKey(
                        name: "fk_mst_employee_contact_mst_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mst_employee_contact_ref_contact_type_contact_type_id",
                        column: x => x.contact_type_id,
                        principalTable: "REF_Contact_Type",
                        principalColumn: "contact_type_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MST_Employee_Education",
                columns: table => new
                {
                    education_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    employee_id = table.Column<int>(type: "INTEGER", nullable: false),
                    degree_id = table.Column<int>(type: "INTEGER", nullable: false),
                    title_id = table.Column<int>(type: "INTEGER", nullable: true),
                    university_id = table.Column<int>(type: "INTEGER", nullable: true),
                    start_year = table.Column<int>(type: "INTEGER", nullable: true),
                    end_year = table.Column<int>(type: "INTEGER", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mst_employee_education", x => x.education_id);
                    table.ForeignKey(
                        name: "fk_mst_employee_education_mst_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mst_employee_education_ref_education_degree_degree_id",
                        column: x => x.degree_id,
                        principalTable: "REF_Education_Degree",
                        principalColumn: "degree_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_education_ref_education_title_title_id",
                        column: x => x.title_id,
                        principalTable: "REF_Education_Title",
                        principalColumn: "title_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_education_ref_university_university_id",
                        column: x => x.university_id,
                        principalTable: "REF_University",
                        principalColumn: "university_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MST_Employee_Employment",
                columns: table => new
                {
                    employment_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    employee_id = table.Column<int>(type: "INTEGER", nullable: false),
                    employment_status_id = table.Column<int>(type: "INTEGER", nullable: false),
                    is_fte = table.Column<bool>(type: "INTEGER", nullable: false),
                    is_sales = table.Column<bool>(type: "INTEGER", nullable: false),
                    employment_type_id = table.Column<int>(type: "INTEGER", nullable: false),
                    vendor_id = table.Column<int>(type: "INTEGER", nullable: true),
                    organization_id = table.Column<int>(type: "INTEGER", nullable: false),
                    location_id = table.Column<int>(type: "INTEGER", nullable: true),
                    job_level_id = table.Column<int>(type: "INTEGER", nullable: false),
                    job_title_id = table.Column<int>(type: "INTEGER", nullable: false),
                    grade_id = table.Column<int>(type: "INTEGER", nullable: true),
                    start_date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    end_date = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    contract_end_date = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    end_reason_id = table.Column<int>(type: "INTEGER", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mst_employee_employment", x => x.employment_id);
                    table.ForeignKey(
                        name: "fk_mst_employee_employment_mst_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mst_employee_employment_ref_employment_status_employment_status_id",
                        column: x => x.employment_status_id,
                        principalTable: "REF_Employment_Status",
                        principalColumn: "employment_status_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_employment_ref_employment_type_employment_type_id",
                        column: x => x.employment_type_id,
                        principalTable: "REF_Employment_Type",
                        principalColumn: "employment_type_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_employment_ref_end_reason_end_reason_id",
                        column: x => x.end_reason_id,
                        principalTable: "REF_End_Reason",
                        principalColumn: "end_reason_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_employment_ref_grade_grade_id",
                        column: x => x.grade_id,
                        principalTable: "REF_Grade",
                        principalColumn: "grade_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_employment_ref_job_level_job_level_id",
                        column: x => x.job_level_id,
                        principalTable: "REF_Job_Level",
                        principalColumn: "job_level_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_employment_ref_job_title_job_title_id",
                        column: x => x.job_title_id,
                        principalTable: "REF_Job_Title",
                        principalColumn: "job_title_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_employment_ref_location_location_id",
                        column: x => x.location_id,
                        principalTable: "REF_Location",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_employment_ref_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "REF_Organization",
                        principalColumn: "organization_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_employment_ref_vendor_vendor_id",
                        column: x => x.vendor_id,
                        principalTable: "REF_Vendor",
                        principalColumn: "vendor_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MST_Employee_Family",
                columns: table => new
                {
                    family_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    employee_id = table.Column<int>(type: "INTEGER", nullable: false),
                    relationship_id = table.Column<int>(type: "INTEGER", nullable: false),
                    gender_id = table.Column<int>(type: "INTEGER", nullable: true),
                    family_name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    is_dependent = table.Column<bool>(type: "INTEGER", nullable: false),
                    is_same_company = table.Column<bool>(type: "INTEGER", nullable: false),
                    wedding_date = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mst_employee_family", x => x.family_id);
                    table.ForeignKey(
                        name: "fk_mst_employee_family_mst_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mst_employee_family_ref_gender_gender_id",
                        column: x => x.gender_id,
                        principalTable: "REF_Gender",
                        principalColumn: "gender_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_family_ref_relationship_relationship_id",
                        column: x => x.relationship_id,
                        principalTable: "REF_Relationship",
                        principalColumn: "relationship_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MST_Employee_FinancialAccount",
                columns: table => new
                {
                    account_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    employee_id = table.Column<int>(type: "INTEGER", nullable: false),
                    account_type_id = table.Column<int>(type: "INTEGER", nullable: false),
                    account_number = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    account_name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    provider_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    is_primary = table.Column<bool>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mst_employee_financial_account", x => x.account_id);
                    table.ForeignKey(
                        name: "fk_mst_employee_financial_account_mst_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mst_employee_financial_account_ref_account_type_account_type_id",
                        column: x => x.account_type_id,
                        principalTable: "REF_Account_Type",
                        principalColumn: "account_type_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MST_Employee_Hierarchy",
                columns: table => new
                {
                    hierarchy_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    employee_id = table.Column<int>(type: "INTEGER", nullable: false),
                    manager_id = table.Column<int>(type: "INTEGER", nullable: false),
                    hierarchy_type_id = table.Column<int>(type: "INTEGER", nullable: false),
                    start_date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    end_date = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mst_employee_hierarchy", x => x.hierarchy_id);
                    table.ForeignKey(
                        name: "fk_mst_employee_hierarchy_mst_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mst_employee_hierarchy_mst_employee_manager_id",
                        column: x => x.manager_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_hierarchy_ref_hierarchy_type_hierarchy_type_id",
                        column: x => x.hierarchy_type_id,
                        principalTable: "REF_Hierarchy_Type",
                        principalColumn: "hierarchy_type_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MST_Employee_Identity",
                columns: table => new
                {
                    identity_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    employee_id = table.Column<int>(type: "INTEGER", nullable: false),
                    identity_type_id = table.Column<int>(type: "INTEGER", nullable: false),
                    identity_number = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    valid_until = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    is_primary = table.Column<bool>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mst_employee_identity", x => x.identity_id);
                    table.ForeignKey(
                        name: "fk_mst_employee_identity_mst_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mst_employee_identity_ref_identity_type_identity_type_id",
                        column: x => x.identity_type_id,
                        principalTable: "REF_Identity_Type",
                        principalColumn: "identity_type_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MST_Employee_WorkExperience",
                columns: table => new
                {
                    experience_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    employee_id = table.Column<int>(type: "INTEGER", nullable: false),
                    industry_id = table.Column<int>(type: "INTEGER", nullable: true),
                    company_name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    job_title = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    location = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    job_level_id = table.Column<int>(type: "INTEGER", nullable: true),
                    start_date = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    end_date = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mst_employee_work_experience", x => x.experience_id);
                    table.ForeignKey(
                        name: "fk_mst_employee_work_experience_mst_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mst_employee_work_experience_ref_industry_industry_id",
                        column: x => x.industry_id,
                        principalTable: "REF_Industry",
                        principalColumn: "industry_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_work_experience_ref_job_level_job_level_id",
                        column: x => x.job_level_id,
                        principalTable: "REF_Job_Level",
                        principalColumn: "job_level_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SYS_User",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "TEXT", nullable: false),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false),
                    last_login_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    employee_id = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sys_user", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_sys_user_mst_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TRX_Attendance",
                columns: table => new
                {
                    attendance_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    employee_id = table.Column<int>(type: "INTEGER", nullable: false),
                    date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    check_in = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    check_out = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    check_in_photo_path = table.Column<string>(type: "TEXT", nullable: true),
                    check_in_latitude = table.Column<double>(type: "REAL", nullable: true),
                    check_in_longitude = table.Column<double>(type: "REAL", nullable: true),
                    check_out_photo_path = table.Column<string>(type: "TEXT", nullable: true),
                    check_out_latitude = table.Column<double>(type: "REAL", nullable: true),
                    check_out_longitude = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trx_attendance", x => x.attendance_id);
                    table.ForeignKey(
                        name: "fk_trx_attendance_mst_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TRX_Leave_Request",
                columns: table => new
                {
                    leave_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    employee_id = table.Column<int>(type: "INTEGER", nullable: false),
                    start_date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    end_date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    reason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    current_level = table.Column<int>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trx_leave_request", x => x.leave_id);
                    table.ForeignKey(
                        name: "fk_trx_leave_request_mst_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MST_Employee_Address",
                columns: table => new
                {
                    address_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    employee_id = table.Column<int>(type: "INTEGER", nullable: false),
                    address_type_id = table.Column<int>(type: "INTEGER", nullable: false),
                    address_detail = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    city_id = table.Column<int>(type: "INTEGER", nullable: true),
                    is_primary = table.Column<bool>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    updated_by = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mst_employee_address", x => x.address_id);
                    table.ForeignKey(
                        name: "fk_mst_employee_address_mst_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mst_employee_address_ref_address_type_address_type_id",
                        column: x => x.address_type_id,
                        principalTable: "REF_Address_Type",
                        principalColumn: "address_type_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_mst_employee_address_ref_city_city_id",
                        column: x => x.city_id,
                        principalTable: "REF_City",
                        principalColumn: "city_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SYS_User_Role",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    role_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sys_user_role", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_sys_user_role_sys_role_role_id",
                        column: x => x.role_id,
                        principalTable: "SYS_Role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_sys_user_role_sys_user_user_id",
                        column: x => x.user_id,
                        principalTable: "SYS_User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TRX_Employee_Kpi_Score",
                columns: table => new
                {
                    score_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    kpi_period_id = table.Column<int>(type: "INTEGER", nullable: false),
                    employee_id = table.Column<int>(type: "INTEGER", nullable: false),
                    criteria_id = table.Column<int>(type: "INTEGER", nullable: false),
                    score = table.Column<decimal>(type: "TEXT", nullable: false),
                    filled_by_user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    filled_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trx_employee_kpi_score", x => x.score_id);
                    table.ForeignKey(
                        name: "fk_trx_employee_kpi_score_mst_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "MST_Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trx_employee_kpi_score_sys_user_filled_by_user_id",
                        column: x => x.filled_by_user_id,
                        principalTable: "SYS_User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trx_employee_kpi_score_trx_kpi_criteria_criteria_id",
                        column: x => x.criteria_id,
                        principalTable: "TRX_Kpi_Criteria",
                        principalColumn: "criteria_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trx_employee_kpi_score_trx_kpi_period_kpi_period_id",
                        column: x => x.kpi_period_id,
                        principalTable: "TRX_Kpi_Period",
                        principalColumn: "kpi_period_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TRX_Leave_Approval",
                columns: table => new
                {
                    leave_approval_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    leave_request_id = table.Column<int>(type: "INTEGER", nullable: false),
                    approver_id = table.Column<int>(type: "INTEGER", nullable: false),
                    level = table.Column<int>(type: "INTEGER", nullable: false),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    acted_at = table.Column<DateTime>(type: "TEXT", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "TRX_Kpi_Score_Revision",
                columns: table => new
                {
                    revision_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    employee_kpi_score_id = table.Column<int>(type: "INTEGER", nullable: false),
                    previous_score = table.Column<decimal>(type: "TEXT", nullable: false),
                    new_score = table.Column<decimal>(type: "TEXT", nullable: false),
                    revised_by_user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    revised_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trx_kpi_score_revision", x => x.revision_id);
                    table.ForeignKey(
                        name: "fk_trx_kpi_score_revision_sys_user_revised_by_user_id",
                        column: x => x.revised_by_user_id,
                        principalTable: "SYS_User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trx_kpi_score_revision_trx_employee_kpi_score_employee_kpi_score_id",
                        column: x => x.employee_kpi_score_id,
                        principalTable: "TRX_Employee_Kpi_Score",
                        principalColumn: "score_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_employee_number",
                table: "MST_Employee",
                column: "employee_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_gender_id",
                table: "MST_Employee",
                column: "gender_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_marital_status_id",
                table: "MST_Employee",
                column: "marital_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_nationality_country_id",
                table: "MST_Employee",
                column: "nationality_country_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_religion_id",
                table: "MST_Employee",
                column: "religion_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_address_address_type_id",
                table: "MST_Employee_Address",
                column: "address_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_address_city_id",
                table: "MST_Employee_Address",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_address_employee_id",
                table: "MST_Employee_Address",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_contact_contact_type_id",
                table: "MST_Employee_Contact",
                column: "contact_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_contact_employee_id",
                table: "MST_Employee_Contact",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_education_degree_id",
                table: "MST_Employee_Education",
                column: "degree_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_education_employee_id",
                table: "MST_Employee_Education",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_education_title_id",
                table: "MST_Employee_Education",
                column: "title_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_education_university_id",
                table: "MST_Employee_Education",
                column: "university_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_employment_employee_id",
                table: "MST_Employee_Employment",
                column: "employee_id",
                unique: true,
                filter: "end_date IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_employment_employment_status_id",
                table: "MST_Employee_Employment",
                column: "employment_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_employment_employment_type_id",
                table: "MST_Employee_Employment",
                column: "employment_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_employment_end_reason_id",
                table: "MST_Employee_Employment",
                column: "end_reason_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_employment_grade_id",
                table: "MST_Employee_Employment",
                column: "grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_employment_job_level_id",
                table: "MST_Employee_Employment",
                column: "job_level_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_employment_job_title_id",
                table: "MST_Employee_Employment",
                column: "job_title_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_employment_location_id",
                table: "MST_Employee_Employment",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_employment_organization_id",
                table: "MST_Employee_Employment",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_employment_vendor_id",
                table: "MST_Employee_Employment",
                column: "vendor_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_family_employee_id",
                table: "MST_Employee_Family",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_family_gender_id",
                table: "MST_Employee_Family",
                column: "gender_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_family_relationship_id",
                table: "MST_Employee_Family",
                column: "relationship_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_financial_account_account_type_id",
                table: "MST_Employee_FinancialAccount",
                column: "account_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_financial_account_employee_id",
                table: "MST_Employee_FinancialAccount",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_hierarchy_employee_id_manager_id_hierarchy_type_id",
                table: "MST_Employee_Hierarchy",
                columns: new[] { "employee_id", "manager_id", "hierarchy_type_id" },
                unique: true,
                filter: "end_date IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_hierarchy_hierarchy_type_id",
                table: "MST_Employee_Hierarchy",
                column: "hierarchy_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_hierarchy_manager_id",
                table: "MST_Employee_Hierarchy",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_identity_employee_id",
                table: "MST_Employee_Identity",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_identity_identity_type_id",
                table: "MST_Employee_Identity",
                column: "identity_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_work_experience_employee_id",
                table: "MST_Employee_WorkExperience",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_work_experience_industry_id",
                table: "MST_Employee_WorkExperience",
                column: "industry_id");

            migrationBuilder.CreateIndex(
                name: "ix_mst_employee_work_experience_job_level_id",
                table: "MST_Employee_WorkExperience",
                column: "job_level_id");

            migrationBuilder.CreateIndex(
                name: "ix_ref_city_province_id",
                table: "REF_City",
                column: "province_id");

            migrationBuilder.CreateIndex(
                name: "ix_ref_country_country_code",
                table: "REF_Country",
                column: "country_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ref_organization_parent_id",
                table: "REF_Organization",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_ref_province_country_id",
                table: "REF_Province",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "ix_sys_role_role_name",
                table: "SYS_Role",
                column: "role_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sys_user_employee_id",
                table: "SYS_User",
                column: "employee_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sys_user_username",
                table: "SYS_User",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sys_user_role_role_id",
                table: "SYS_User_Role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_attendance_employee_id",
                table: "TRX_Attendance",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_employee_kpi_score_criteria_id",
                table: "TRX_Employee_Kpi_Score",
                column: "criteria_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_employee_kpi_score_employee_id",
                table: "TRX_Employee_Kpi_Score",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_employee_kpi_score_filled_by_user_id",
                table: "TRX_Employee_Kpi_Score",
                column: "filled_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_employee_kpi_score_kpi_period_id_employee_id_criteria_id",
                table: "TRX_Employee_Kpi_Score",
                columns: new[] { "kpi_period_id", "employee_id", "criteria_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_trx_kpi_period_name_year",
                table: "TRX_Kpi_Period",
                columns: new[] { "name", "year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_trx_kpi_score_revision_employee_kpi_score_id",
                table: "TRX_Kpi_Score_Revision",
                column: "employee_kpi_score_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_kpi_score_revision_revised_by_user_id",
                table: "TRX_Kpi_Score_Revision",
                column: "revised_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_leave_approval_approver_id",
                table: "TRX_Leave_Approval",
                column: "approver_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_leave_approval_leave_request_id",
                table: "TRX_Leave_Approval",
                column: "leave_request_id");

            migrationBuilder.CreateIndex(
                name: "ix_trx_leave_request_employee_id",
                table: "TRX_Leave_Request",
                column: "employee_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MST_Employee_Address");

            migrationBuilder.DropTable(
                name: "MST_Employee_Contact");

            migrationBuilder.DropTable(
                name: "MST_Employee_Education");

            migrationBuilder.DropTable(
                name: "MST_Employee_Employment");

            migrationBuilder.DropTable(
                name: "MST_Employee_Family");

            migrationBuilder.DropTable(
                name: "MST_Employee_FinancialAccount");

            migrationBuilder.DropTable(
                name: "MST_Employee_Hierarchy");

            migrationBuilder.DropTable(
                name: "MST_Employee_Identity");

            migrationBuilder.DropTable(
                name: "MST_Employee_WorkExperience");

            migrationBuilder.DropTable(
                name: "SYS_User_Role");

            migrationBuilder.DropTable(
                name: "TRX_Attendance");

            migrationBuilder.DropTable(
                name: "TRX_Kpi_Score_Revision");

            migrationBuilder.DropTable(
                name: "TRX_Leave_Approval");

            migrationBuilder.DropTable(
                name: "REF_Address_Type");

            migrationBuilder.DropTable(
                name: "REF_City");

            migrationBuilder.DropTable(
                name: "REF_Contact_Type");

            migrationBuilder.DropTable(
                name: "REF_Education_Degree");

            migrationBuilder.DropTable(
                name: "REF_Education_Title");

            migrationBuilder.DropTable(
                name: "REF_University");

            migrationBuilder.DropTable(
                name: "REF_Employment_Status");

            migrationBuilder.DropTable(
                name: "REF_Employment_Type");

            migrationBuilder.DropTable(
                name: "REF_End_Reason");

            migrationBuilder.DropTable(
                name: "REF_Grade");

            migrationBuilder.DropTable(
                name: "REF_Job_Title");

            migrationBuilder.DropTable(
                name: "REF_Location");

            migrationBuilder.DropTable(
                name: "REF_Organization");

            migrationBuilder.DropTable(
                name: "REF_Vendor");

            migrationBuilder.DropTable(
                name: "REF_Relationship");

            migrationBuilder.DropTable(
                name: "REF_Account_Type");

            migrationBuilder.DropTable(
                name: "REF_Hierarchy_Type");

            migrationBuilder.DropTable(
                name: "REF_Identity_Type");

            migrationBuilder.DropTable(
                name: "REF_Industry");

            migrationBuilder.DropTable(
                name: "REF_Job_Level");

            migrationBuilder.DropTable(
                name: "SYS_Role");

            migrationBuilder.DropTable(
                name: "TRX_Employee_Kpi_Score");

            migrationBuilder.DropTable(
                name: "TRX_Leave_Request");

            migrationBuilder.DropTable(
                name: "REF_Province");

            migrationBuilder.DropTable(
                name: "SYS_User");

            migrationBuilder.DropTable(
                name: "TRX_Kpi_Criteria");

            migrationBuilder.DropTable(
                name: "TRX_Kpi_Period");

            migrationBuilder.DropTable(
                name: "MST_Employee");

            migrationBuilder.DropTable(
                name: "REF_Country");

            migrationBuilder.DropTable(
                name: "REF_Gender");

            migrationBuilder.DropTable(
                name: "REF_Marital_Status");

            migrationBuilder.DropTable(
                name: "REF_Religion");
        }
    }
}
