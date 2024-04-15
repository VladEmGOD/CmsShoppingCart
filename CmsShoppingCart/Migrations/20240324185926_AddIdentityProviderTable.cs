using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace CmsShoppingCart.WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityProviderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder builder)
        {
            builder.CreateTable(
                name: "SSOIdentityProviders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(type: "nvarchar(350)", nullable: false),
                    ApplicationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SSOIdentityProviders", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder builder)
        {
            builder.DropTable("SSOIdentityProviders");
        }
    }
}
