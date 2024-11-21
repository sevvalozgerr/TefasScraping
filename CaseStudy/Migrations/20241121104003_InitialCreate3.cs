using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaseStudy.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_newfund_returns_newfund_FundId",
                table: "newfund_returns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_newfund_returns",
                table: "newfund_returns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_newfund",
                table: "newfund");

            migrationBuilder.RenameTable(
                name: "newfund_returns",
                newName: "fund_returns");

            migrationBuilder.RenameTable(
                name: "newfund",
                newName: "fund");

            migrationBuilder.RenameIndex(
                name: "IX_newfund_returns_FundId",
                table: "fund_returns",
                newName: "IX_fund_returns_FundId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_fund_returns",
                table: "fund_returns",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_fund",
                table: "fund",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_fund_returns_fund_FundId",
                table: "fund_returns",
                column: "FundId",
                principalTable: "fund",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fund_returns_fund_FundId",
                table: "fund_returns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_fund_returns",
                table: "fund_returns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_fund",
                table: "fund");

            migrationBuilder.RenameTable(
                name: "fund_returns",
                newName: "newfund_returns");

            migrationBuilder.RenameTable(
                name: "fund",
                newName: "newfund");

            migrationBuilder.RenameIndex(
                name: "IX_fund_returns_FundId",
                table: "newfund_returns",
                newName: "IX_newfund_returns_FundId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_newfund_returns",
                table: "newfund_returns",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_newfund",
                table: "newfund",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_newfund_returns_newfund_FundId",
                table: "newfund_returns",
                column: "FundId",
                principalTable: "newfund",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
