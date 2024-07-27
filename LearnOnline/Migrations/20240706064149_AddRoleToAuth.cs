using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnOnline.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleToAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "auths",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "auths");
        }
    }
}
