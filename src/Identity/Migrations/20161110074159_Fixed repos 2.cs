using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Identity.Migrations
{
    public partial class Fixedrepos2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Repos",
                table: "Repos");

            migrationBuilder.AddColumn<string>(
                name: "RepoKey",
                table: "Repos",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "RepoName",
                table: "Repos",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Repos",
                table: "Repos",
                column: "RepoKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Repos",
                table: "Repos");

            migrationBuilder.DropColumn(
                name: "RepoKey",
                table: "Repos");

            migrationBuilder.AlterColumn<string>(
                name: "RepoName",
                table: "Repos",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Repos",
                table: "Repos",
                column: "RepoName");
        }
    }
}
