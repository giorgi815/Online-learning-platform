using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OLP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fourth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificate_Courses_CourseId",
                table: "Certificate");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificate_DomainUsers_StudentId",
                table: "Certificate");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Courses_CourseId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_DomainUsers_StudentId",
                table: "Payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Certificate",
                table: "Certificate");

            migrationBuilder.RenameTable(
                name: "Payment",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "Certificate",
                newName: "Certificates");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_StudentId_CourseId",
                table: "Payments",
                newName: "IX_Payments_StudentId_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_ProviderSessionId",
                table: "Payments",
                newName: "IX_Payments_ProviderSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_CourseId",
                table: "Payments",
                newName: "IX_Payments_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Certificate_StudentId_CourseId",
                table: "Certificates",
                newName: "IX_Certificates_StudentId_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Certificate_CourseId",
                table: "Certificates",
                newName: "IX_Certificates_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Certificates",
                table: "Certificates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Courses_CourseId",
                table: "Certificates",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_DomainUsers_StudentId",
                table: "Certificates",
                column: "StudentId",
                principalTable: "DomainUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Courses_CourseId",
                table: "Payments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_DomainUsers_StudentId",
                table: "Payments",
                column: "StudentId",
                principalTable: "DomainUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Courses_CourseId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_DomainUsers_StudentId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Courses_CourseId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_DomainUsers_StudentId",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Certificates",
                table: "Certificates");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payment");

            migrationBuilder.RenameTable(
                name: "Certificates",
                newName: "Certificate");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_StudentId_CourseId",
                table: "Payment",
                newName: "IX_Payment_StudentId_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_ProviderSessionId",
                table: "Payment",
                newName: "IX_Payment_ProviderSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_CourseId",
                table: "Payment",
                newName: "IX_Payment_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Certificates_StudentId_CourseId",
                table: "Certificate",
                newName: "IX_Certificate_StudentId_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Certificates_CourseId",
                table: "Certificate",
                newName: "IX_Certificate_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Certificate",
                table: "Certificate",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificate_Courses_CourseId",
                table: "Certificate",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificate_DomainUsers_StudentId",
                table: "Certificate",
                column: "StudentId",
                principalTable: "DomainUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Courses_CourseId",
                table: "Payment",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_DomainUsers_StudentId",
                table: "Payment",
                column: "StudentId",
                principalTable: "DomainUsers",
                principalColumn: "Id");
        }
    }
}
