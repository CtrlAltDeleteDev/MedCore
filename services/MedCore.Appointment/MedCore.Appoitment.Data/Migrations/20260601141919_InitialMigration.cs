using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedCore.Appoitment.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Appoitment");

            migrationBuilder.CreateTable(
                name: "Employees",
                schema: "Appoitment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meets",
                schema: "Appoitment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SkillIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meets_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "Appoitment",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                schema: "Appoitment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MeetId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skills_Meets_MeetId",
                        column: x => x.MeetId,
                        principalSchema: "Appoitment",
                        principalTable: "Meets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSkills",
                schema: "Appoitment",
                columns: table => new
                {
                    EmployeesId = table.Column<int>(type: "int", nullable: false),
                    SkillsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSkills", x => new { x.EmployeesId, x.SkillsId });
                    table.ForeignKey(
                        name: "FK_EmployeeSkills_Employees_EmployeesId",
                        column: x => x.EmployeesId,
                        principalSchema: "Appoitment",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeSkills_Skills_SkillsId",
                        column: x => x.SkillsId,
                        principalSchema: "Appoitment",
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkills_SkillsId",
                schema: "Appoitment",
                table: "EmployeeSkills",
                column: "SkillsId");

            migrationBuilder.CreateIndex(
                name: "IX_Meets_EmployeeId",
                schema: "Appoitment",
                table: "Meets",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_MeetId",
                schema: "Appoitment",
                table: "Skills",
                column: "MeetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeSkills",
                schema: "Appoitment");

            migrationBuilder.DropTable(
                name: "Skills",
                schema: "Appoitment");

            migrationBuilder.DropTable(
                name: "Meets",
                schema: "Appoitment");

            migrationBuilder.DropTable(
                name: "Employees",
                schema: "Appoitment");
        }
    }
}
