using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsShoppingCart.WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder builder)
        {
            builder.CreateTable(
                 name: "Orders",
                 columns: table => new
                 {
                     Id = table.Column<Guid>(nullable: false),
                     UserId = table.Column<string>(type: "nvarchar(350)", nullable: false),
                     Data = table.Column<string>(nullable: false),
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Orders", x => x.Id);
                 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
