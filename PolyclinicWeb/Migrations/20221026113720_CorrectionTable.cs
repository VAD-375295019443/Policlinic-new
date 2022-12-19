using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyclinicWeb.Migrations
{
    public partial class CorrectionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdditionalRecommendations_Diarys_DiaryId",
                table: "AdditionalRecommendations");

            migrationBuilder.DropForeignKey(
                name: "FK_ClinicalDiagnoses_Diarys_DiaryId",
                table: "ClinicalDiagnoses");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicationRecommendations_Diarys_DiaryId",
                table: "MedicationRecommendations");

            migrationBuilder.DropTable(
                name: "Diarys");

            migrationBuilder.DropTable(
                name: "IndicatorsOfLaboratoryTests");

            migrationBuilder.DropTable(
                name: "Plots");

            migrationBuilder.RenameColumn(
                name: "DiaryId",
                table: "MedicationRecommendations",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_MedicationRecommendations_DiaryId",
                table: "MedicationRecommendations",
                newName: "IX_MedicationRecommendations_PatientId");

            migrationBuilder.RenameColumn(
                name: "DiaryId",
                table: "ClinicalDiagnoses",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_ClinicalDiagnoses_DiaryId",
                table: "ClinicalDiagnoses",
                newName: "IX_ClinicalDiagnoses_PatientId");

            migrationBuilder.RenameColumn(
                name: "DiaryId",
                table: "AdditionalRecommendations",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_AdditionalRecommendations_DiaryId",
                table: "AdditionalRecommendations",
                newName: "IX_AdditionalRecommendations_PatientId");

            migrationBuilder.AddColumn<DateTime>(
                name: "AppointmentDate",
                table: "MedicationRecommendations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeregistrationDate",
                table: "ClinicalDiagnoses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "ClinicalDiagnoses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "AppointmentDate",
                table: "AdditionalRecommendations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_AdditionalRecommendations_Patients_PatientId",
                table: "AdditionalRecommendations",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicalDiagnoses_Patients_PatientId",
                table: "ClinicalDiagnoses",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationRecommendations_Patients_PatientId",
                table: "MedicationRecommendations",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdditionalRecommendations_Patients_PatientId",
                table: "AdditionalRecommendations");

            migrationBuilder.DropForeignKey(
                name: "FK_ClinicalDiagnoses_Patients_PatientId",
                table: "ClinicalDiagnoses");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicationRecommendations_Patients_PatientId",
                table: "MedicationRecommendations");

            migrationBuilder.DropColumn(
                name: "AppointmentDate",
                table: "MedicationRecommendations");

            migrationBuilder.DropColumn(
                name: "DeregistrationDate",
                table: "ClinicalDiagnoses");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "ClinicalDiagnoses");

            migrationBuilder.DropColumn(
                name: "AppointmentDate",
                table: "AdditionalRecommendations");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "MedicationRecommendations",
                newName: "DiaryId");

            migrationBuilder.RenameIndex(
                name: "IX_MedicationRecommendations_PatientId",
                table: "MedicationRecommendations",
                newName: "IX_MedicationRecommendations_DiaryId");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "ClinicalDiagnoses",
                newName: "DiaryId");

            migrationBuilder.RenameIndex(
                name: "IX_ClinicalDiagnoses_PatientId",
                table: "ClinicalDiagnoses",
                newName: "IX_ClinicalDiagnoses_DiaryId");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "AdditionalRecommendations",
                newName: "DiaryId");

            migrationBuilder.RenameIndex(
                name: "IX_AdditionalRecommendations_PatientId",
                table: "AdditionalRecommendations",
                newName: "IX_AdditionalRecommendations_DiaryId");

            migrationBuilder.CreateTable(
                name: "Diarys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<long>(type: "bigint", nullable: true),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Complaints = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MedicalWorker = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObjectiveData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Profession = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diarys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diarys_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IndicatorsOfLaboratoryTests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryTestId = table.Column<long>(type: "bigint", nullable: true),
                    Indicator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicatorsOfLaboratoryTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndicatorsOfLaboratoryTests_LaboratoryTests_LaboratoryTestId",
                        column: x => x.LaboratoryTestId,
                        principalTable: "LaboratoryTests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Plots",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plots_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diarys_PatientId",
                table: "Diarys",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorsOfLaboratoryTests_LaboratoryTestId",
                table: "IndicatorsOfLaboratoryTests",
                column: "LaboratoryTestId");

            migrationBuilder.CreateIndex(
                name: "IX_Plots_PatientId",
                table: "Plots",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdditionalRecommendations_Diarys_DiaryId",
                table: "AdditionalRecommendations",
                column: "DiaryId",
                principalTable: "Diarys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicalDiagnoses_Diarys_DiaryId",
                table: "ClinicalDiagnoses",
                column: "DiaryId",
                principalTable: "Diarys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationRecommendations_Diarys_DiaryId",
                table: "MedicationRecommendations",
                column: "DiaryId",
                principalTable: "Diarys",
                principalColumn: "Id");
        }
    }
}
