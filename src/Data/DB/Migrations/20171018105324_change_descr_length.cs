﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace BioEngine.Data.DB.Migrations
{
    public partial class change_descr_length : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "descr",
                table: "be_articles_cats",
                nullable: true,
                maxLength: 1000);
        }
        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "descr",
                table: "be_articles_cats",
                nullable: true,
                maxLength: 100);
        }
    }
}
