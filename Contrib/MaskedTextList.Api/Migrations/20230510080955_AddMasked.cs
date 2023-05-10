using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecAll.Contrib.MaskedTextList.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMasked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "textitems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MaskedContent",
                table: "textitems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "textitems");

            migrationBuilder.DropColumn(
                name: "MaskedContent",
                table: "textitems");
        }
    }
}
