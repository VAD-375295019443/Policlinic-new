using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyclinicWeb.Migrations
{
    public partial class KillMainClinicalDiagnosis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Main",
                table: "ClinicalDiagnoses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Main",
                table: "ClinicalDiagnoses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
