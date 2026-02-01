using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VenusHR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DBInitialize2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hrs_Employees",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EngName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArbName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArbName4S = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FamilyEngName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FamilyArbName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FamilyArbName4S = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FatherEngName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FatherArbName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FatherArbName4S = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrandEngName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrandArbName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrandArbName4S = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BirthCityId = table.Column<int>(type: "int", nullable: true),
                    ReligionId = table.Column<int>(type: "int", nullable: true),
                    MaritalStatusId = table.Column<int>(type: "int", nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BloodGroupId = table.Column<int>(type: "int", nullable: true),
                    BankId = table.Column<int>(type: "int", nullable: true),
                    NationalityId = table.Column<int>(type: "int", nullable: true),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankAccNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    Gosinumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GosijoinDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GosiexcludeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExcludeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegUserId = table.Column<int>(type: "int", nullable: false),
                    RegComputerId = table.Column<int>(type: "int", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CancelDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    SponsorId = table.Column<int>(type: "int", nullable: true),
                    EMail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    MachineCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectorId = table.Column<int>(type: "int", nullable: true),
                    SsnNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassPortNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntryNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cost1 = table.Column<int>(type: "int", nullable: true),
                    Cost2 = table.Column<int>(type: "int", nullable: true),
                    Cost3 = table.Column<int>(type: "int", nullable: true),
                    Cost4 = table.Column<int>(type: "int", nullable: true),
                    LaborOfficeNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    Whours = table.Column<float>(type: "real", nullable: true),
                    IsProjectRelated = table.Column<bool>(type: "bit", nullable: true),
                    IsSpecialForce = table.Column<bool>(type: "bit", nullable: true),
                    MaxLoanDedution = table.Column<double>(type: "float", nullable: true),
                    LedgerCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasTaqat = table.Column<bool>(type: "bit", nullable: true),
                    BankAccountType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hrs_Employees", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hrs_Employees");
        }
    }
}
