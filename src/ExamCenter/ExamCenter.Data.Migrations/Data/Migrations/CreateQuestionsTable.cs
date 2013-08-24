using FluentMigrator;

namespace ExamCenter.Data.Migrations
{
    [Migration(2013082401)]
    public class CreateQuestionsTable : Migration
    {
        public override void Up()
        {
            Create.Table("Questions")

                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()

                .WithColumn("QuestionText").AsString(2000).NotNullable()
                .WithColumn("AnsweringType").AsInt32().NotNullable()
                .WithColumn("EstimatedAnsweringTime").AsInt32().Nullable()
                .WithColumn("ExperienceDegree").AsInt32().Nullable()
                .WithColumn("TotalAskedCountInAllExams").AsInt32().NotNullable()
                .WithColumn("LastAskedTime").AsDateTime().Nullable()
                .WithColumn("RightAnswerText").AsString(1000).Nullable()

                .WithColumn("CreationDate").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("CreatorUserId").AsInt32().Nullable().ForeignKey("Users", "Id")

                .WithColumn("LastModificationDate").AsDateTime().Nullable()
                .WithColumn("LastModifierUserId").AsInt32().Nullable().ForeignKey("Users", "Id");
        }

        public override void Down()
        {
            Delete.Table("Questions");
        }
    }
}
