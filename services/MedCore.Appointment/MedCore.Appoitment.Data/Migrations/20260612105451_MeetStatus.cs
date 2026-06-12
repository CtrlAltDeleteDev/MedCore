using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
#pragma warning disable SA1122

namespace MedCore.Appoitment.Data.Migrations
{
    /// <inheritdoc />
    public partial class MeetStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Appoitment",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SkillIds",
                schema: "Appoitment",
                table: "Meets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "Appoitment",
                table: "Meets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Status",
                schema: "Appoitment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "Appoitment",
                table: "Status",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "Pending" },
                    { 1, "Confirmed" },
                    { 2, "Cancelled" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Meets_Status",
                schema: "Appoitment",
                table: "Meets",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_Meets_Status_Status",
                schema: "Appoitment",
                table: "Meets",
                column: "Status",
                principalSchema: "Appoitment",
                principalTable: "Status",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meets_Status_Status",
                schema: "Appoitment",
                table: "Meets");

            migrationBuilder.DropTable(
                name: "Status",
                schema: "Appoitment");

            migrationBuilder.DropIndex(
                name: "IX_Meets_Status",
                schema: "Appoitment",
                table: "Meets");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Appoitment",
                table: "Meets");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Appoitment",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SkillIds",
                schema: "Appoitment",
                table: "Meets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
