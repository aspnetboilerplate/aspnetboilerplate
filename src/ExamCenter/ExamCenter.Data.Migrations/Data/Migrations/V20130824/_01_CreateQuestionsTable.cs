using Abp.Data.Migrations;
using FluentMigrator;

namespace ExamCenter.Data.Migrations.V20130824
{
    [Migration(2013082401)]
    public class _01_CreateQuestionsTable : Migration
    {
        public override void Up()
        {
            Create.Table("Questions")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("QuestionText").AsString(2000).NotNullable()
                .WithColumn("AnsweringType").AsInt32().NotNullable()
                .WithColumn("EstimatedAnsweringTime").AsInt32().Nullable()
                .WithColumn("ExperienceDegree").AsInt32().Nullable()
                .WithColumn("RightAnswerText").AsString(1000).Nullable()
                .WithAuditColumns();
        }

        public override void Down()
        {
            Delete.Table("Questions");
        }
    }
}
