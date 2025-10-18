using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data_Analyse_MVC.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnalyseResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AnalyseResults_UploadFileId",
                table: "AnalyseResults",
                column: "UploadFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalyseResults_UploadFiles_UploadFileId",
                table: "AnalyseResults",
                column: "UploadFileId",
                principalTable: "UploadFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalyseResults_UploadFiles_UploadFileId",
                table: "AnalyseResults");

            migrationBuilder.DropIndex(
                name: "IX_AnalyseResults_UploadFileId",
                table: "AnalyseResults");
        }
    }
}
