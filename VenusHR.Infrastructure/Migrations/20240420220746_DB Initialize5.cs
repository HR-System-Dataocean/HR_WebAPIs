using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VenusHR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DBInitialize5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SS_RequestActions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionSerial = table.Column<int>(type: "int", nullable: false),
                    RequestSerial = table.Column<int>(type: "int", nullable: false),
                    SS_EmployeeID = table.Column<int>(type: "int", nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfigID = table.Column<int>(type: "int", nullable: false),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    Seen = table.Column<bool>(type: "bit", nullable: false),
                    ActionID = table.Column<int>(type: "int", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmedNoOfdays = table.Column<int>(type: "int", nullable: false),
                    ActionRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsHidden = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SS_RequestActions", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SS_RequestActions");
        }
    }
}
