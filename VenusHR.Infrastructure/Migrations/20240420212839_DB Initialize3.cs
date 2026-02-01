using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VenusHR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DBInitialize3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EMail",
                table: "Hrs_Employees",
                newName: "E_Mail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "E_Mail",
                table: "Hrs_Employees",
                newName: "EMail");
        }
    }
}
