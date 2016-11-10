using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Identity.Migrations
{
    public partial class Fixedrepos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Repos",
                table: "Repos");

            migrationBuilder.DropColumn(
                name: "UserId",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Repos",
                table: "Repos");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
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
                column: "UserId");
        }
    }
}
