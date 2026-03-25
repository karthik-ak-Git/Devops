using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DevOpsAIAgent.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Enable vector extension for PostgreSQL
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "CiCdEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                        .Annotation("Sqlite:Autoincrement", true),
                    RepositoryName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RepositoryUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CommitHash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Conclusion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    WorkflowName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WorkflowRunId = table.Column<long>(type: "bigint", nullable: true),
                    RunUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BranchName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TriggerActor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CiCdEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Deployments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                        .Annotation("Sqlite:Autoincrement", true),
                    RepositoryName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Environment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Version = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CommitHash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DeployedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deployments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackedRepositories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Owner = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    HtmlUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DefaultBranch = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedRepositories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebhookConfigurations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                        .Annotation("Sqlite:Autoincrement", true),
                    RepositoryFullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Owner = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RepoName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    WebhookUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SecretHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Events = table.Column<string>(type: "jsonb", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AiAnalyses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                        .Annotation("Sqlite:Autoincrement", true),
                    CiCdEventId = table.Column<long>(type: "bigint", nullable: false),
                    AnalysisText = table.Column<string>(type: "text", nullable: false),
                    GitDiff = table.Column<string>(type: "text", nullable: true),
                    ErrorLog = table.Column<string>(type: "text", nullable: true),
                    ModelUsed = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Embedding = table.Column<object>(type: "vector(1536)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TokensUsed = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AiAnalyses_CiCdEvents_CiCdEventId",
                        column: x => x.CiCdEventId,
                        principalTable: "CiCdEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Incidents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RepositoryName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AssignedTo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RelatedCiCdEventId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incidents_CiCdEvents_RelatedCiCdEventId",
                        column: x => x.RelatedCiCdEventId,
                        principalTable: "CiCdEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            // Create indexes for performance
            migrationBuilder.CreateIndex(
                name: "IX_AiAnalyses_CiCdEventId",
                table: "AiAnalyses",
                column: "CiCdEventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AiAnalyses_CreatedAt",
                table: "AiAnalyses",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CiCdEvents_Conclusion",
                table: "CiCdEvents",
                column: "Conclusion");

            migrationBuilder.CreateIndex(
                name: "IX_CiCdEvents_ReceivedAt",
                table: "CiCdEvents",
                column: "ReceivedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CiCdEvents_RepositoryName",
                table: "CiCdEvents",
                column: "RepositoryName");

            migrationBuilder.CreateIndex(
                name: "IX_CiCdEvents_RepositoryName_ReceivedAt",
                table: "CiCdEvents",
                columns: new[] { "RepositoryName", "ReceivedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Deployments_Environment_Status",
                table: "Deployments",
                columns: new[] { "Environment", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Deployments_RepositoryName",
                table: "Deployments",
                column: "RepositoryName");

            migrationBuilder.CreateIndex(
                name: "IX_Deployments_StartedAt",
                table: "Deployments",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_CreatedAt",
                table: "Incidents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_RelatedCiCdEventId",
                table: "Incidents",
                column: "RelatedCiCdEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_Severity",
                table: "Incidents",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_Status",
                table: "Incidents",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_Status_Severity",
                table: "Incidents",
                columns: new[] { "Status", "Severity" });

            migrationBuilder.CreateIndex(
                name: "IX_TrackedRepositories_FullName",
                table: "TrackedRepositories",
                column: "FullName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrackedRepositories_IsActive",
                table: "TrackedRepositories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedRepositories_Owner",
                table: "TrackedRepositories",
                column: "Owner");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_Key",
                table: "UserSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UpdatedAt",
                table: "UserSettings",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookConfigurations_IsActive",
                table: "WebhookConfigurations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookConfigurations_Owner",
                table: "WebhookConfigurations",
                column: "Owner");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookConfigurations_RepositoryFullName",
                table: "WebhookConfigurations",
                column: "RepositoryFullName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiAnalyses");

            migrationBuilder.DropTable(
                name: "Deployments");

            migrationBuilder.DropTable(
                name: "Incidents");

            migrationBuilder.DropTable(
                name: "TrackedRepositories");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "WebhookConfigurations");

            migrationBuilder.DropTable(
                name: "CiCdEvents");
        }
    }
}