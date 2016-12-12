using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Identity.Migrations
{
    public partial class Private : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GitRepoRepoKey",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_GitRepoRepoKey",
                table: "AspNetUsers",
                column: "GitRepoRepoKey");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Repos_GitRepoRepoKey",
                table: "AspNetUsers",
                column: "GitRepoRepoKey",
                principalTable: "Repos",
                principalColumn: "RepoKey",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Repos_GitRepoRepoKey",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_GitRepoRepoKey",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GitRepoRepoKey",
                table: "AspNetUsers");
        }
    }
}
