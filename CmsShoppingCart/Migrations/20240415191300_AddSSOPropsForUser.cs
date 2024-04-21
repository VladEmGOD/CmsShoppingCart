using CmsShoppingCart.WebApp.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsShoppingCart.WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSSOPropsForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthenticationType",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IdentityProviderId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.Sql("UPDATE AspNetUsers SET AuthenticationType = 1 WHERE Id='41487ff9-929d-40f2-af1f-4daa5f8f6399'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthenticationType",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdentityProviderId",
                table: "AspNetUsers");
        }
    }
}
