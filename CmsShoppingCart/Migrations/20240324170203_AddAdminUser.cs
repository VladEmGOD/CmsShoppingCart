using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json.Linq;
using System.Security;

#nullable disable

namespace CmsShoppingCart.WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder builder)
        {
            builder.InsertData(
                table: "AspNetUsers",
                columns: [
                    "Id",
                    "Ocupation",
                    "UserName",
                    "NormalizedUserName",
                    "Email",
                    "NormalizedEmail",
                    "EmailConfirmed",
                    "PasswordHash",
                    "SecurityStamp",
                    "ConcurrencyStamp",
                    "AccessFailedCount",
                    "PhoneNumberConfirmed",
                    "TwoFactorEnabled",
                    "LockoutEnabled"
                    ],
                values: [
                    "41487ff9-929d-40f2-af1f-4daa5f8f6399",
                    "Admin",
                    "admin",
                    "ADMIN",
                    "admin@gmail.com",
                    "ADMIN@GMAIL.COM",
                    true,
                    "AQAAAAIAAYagAAAAEMVwPyrYa/Sy6V3Bu8rlb8WwtDamdWssT6rkAeM48pfUwhcSwNwEGfuW9FrFTWcOjg==",
                    "TYYDK22O4KVJEZL4WJSCVDT7DHPER6DN",
                    "cb818db3-72f3-47a6-a8f1-2b50b70ca6ef",
                    0,
                    false,
                    false,
                    true
                    ]
                );

            builder.InsertData(
                table: "AspNetRoles",
                columns: [
                    "Id",
                    "Name",
                    "NormalizedName",
                    "ConcurrencyStamp",
                ],
                values: [
                    "754be0a4-28fa-4940-b870-a991323a5153",
                    "Admin",
                    "ADMIN",
                    "b0a2fb76-bde0-4b79-bc36-7ec5afecbbf3"
                    ]
                );

            builder.InsertData(
                table: "AspNetUserRoles",
                columns: [
                    "UserId",
                    "RoleId",
                ],
                values: [
                    "41487ff9-929d-40f2-af1f-4daa5f8f6399",
                    "754be0a4-28fa-4940-b870-a991323a5153"
                    ]
                );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder builder)
        {
            builder.DeleteData(table: "AspNetUserRoles", keyColumn: "UserId", keyValue: "41487ff9-929d-40f2-af1f-4daa5f8f6399");
            builder.DeleteData(table: "AspNetRoles", keyColumn: "Id", keyValue: "754be0a4-28fa-4940-b870-a991323a5153");
            builder.DeleteData(table: "AspNetUsers", keyColumn: "Id", keyValue: "41487ff9-929d-40f2-af1f-4daa5f8f6399");
        }
    }
}
