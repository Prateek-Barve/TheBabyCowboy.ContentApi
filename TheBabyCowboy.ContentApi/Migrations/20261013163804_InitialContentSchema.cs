using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBabyCowboy.ContentApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialContentSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentCertifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Issuer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentCertifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentProjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ShortDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    GitHubUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentProjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentSkillCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentSkillCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IconUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    SkillCategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentSkills_ContentSkillCategories_SkillCategoryId",
                        column: x => x.SkillCategoryId,
                        principalTable: "ContentSkillCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentProjectSkills",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentProjectSkills", x => new { x.ProjectId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_ContentProjectSkills_ContentProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "ContentProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentProjectSkills_ContentSkills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "ContentSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentProjectSkills_SkillId",
                table: "ContentProjectSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentSkillCategories_Name",
                table: "ContentSkillCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentSkills_SkillCategoryId_Name",
                table: "ContentSkills",
                columns: new[] { "SkillCategoryId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentCertifications");

            migrationBuilder.DropTable(
                name: "ContentProjectSkills");

            migrationBuilder.DropTable(
                name: "ContentProjects");

            migrationBuilder.DropTable(
                name: "ContentSkills");

            migrationBuilder.DropTable(
                name: "ContentSkillCategories");
        }
    }
}
