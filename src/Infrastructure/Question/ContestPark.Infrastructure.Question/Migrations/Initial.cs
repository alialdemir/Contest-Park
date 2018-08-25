using ContestPark.Domain.Question.Enums;
using FluentMigrator;

namespace ContestPark.Infrastructure.Question.Migrations
{
    [Migration(5)]
    public class Initial : Migration
    {
        public override void Up()
        {
            #region Question

            Create.Table("Questions")
                 .WithColumn("QuestionId")
                 .AsInt32()
                 .PrimaryKey()
                 .Identity()

                 .WithColumn("ModifiedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable()

                 .WithColumn("CreatedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable();

            #endregion Question

            #region QuestionInfo

            Create.Table("QuestionInfos")
                 .WithColumn("QuestionInfoId")
                 .AsInt32()
                 .PrimaryKey()
                 .Identity()

                 .WithColumn("Link")
                 .AsString(350)
                 .NotNullable()

                 .WithColumn("AnswerType")
                 .AsByte()
                 .WithDefaultValue((byte)AnswerTypes.Text)

                 .WithColumn("QuestionType")
                 .AsByte()
                 .WithDefaultValue((byte)QuestionTypes.Text)

                 .WithColumn("IsActive")
                 .AsBoolean()
                 .WithDefaultValue(true)

                 .WithColumn("SubCategoryId")
                 .AsInt16()
                 .NotNullable()
                 //       .ForeignKey("SubCategories", "SubCategoryId")

                 .WithColumn("QuestionId")
                 .AsInt32()
                 .NotNullable()
                 .ForeignKey("Questions", "QuestionId")

                 .WithColumn("ModifiedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable()

                 .WithColumn("CreatedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable();

            #endregion QuestionInfo

            #region AskedQuestion

            Create.Table("AskedQuestions")
                 .WithColumn("AskedQuestionId")
                 .AsInt32()
                 .PrimaryKey()
                 .Identity()

                 .WithColumn("QuestionInfoId")
                 .AsInt32()
                 .NotNullable()
                 .ForeignKey("QuestionInfos", "QuestionInfoId")

                 .WithColumn("UserId")
                 .AsString(450)
                 .NotNullable()
                 //   .ForeignKey("AspNetUsers", "Id")

                 .WithColumn("SubCategoryId")
                 .AsInt16()
                 .NotNullable()
                 //    .ForeignKey("SubCategories", "SubCategoryId")

                 .WithColumn("ModifiedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable()

                 .WithColumn("CreatedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable();

            #endregion AskedQuestion

            #region QuestionAnswer

            Create.Table("QuestionAnswers")
                 .WithColumn("QuestionAnswerId")
                 .AsInt32()
                 .PrimaryKey()
                 .Identity()

                 .WithColumn("Answer")
                 .AsString(200)
                 .NotNullable()

                 .WithColumn("IsCorrect")
                 .AsBoolean()
                 .WithDefaultValue(false)

                 .WithColumn("LanguageId")
                 .AsByte()
                 .NotNullable()

                 .WithColumn("QuestionInfoId")
                 .AsInt32()
                 .NotNullable()
                 .ForeignKey("QuestionInfos", "QuestionInfoId")

                 .WithColumn("QuestionId")
                 .AsInt32()
                 .NotNullable()
                 .ForeignKey("Questions", "QuestionId")

                 .WithColumn("ModifiedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable()

                 .WithColumn("CreatedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable();

            #endregion QuestionAnswer

            #region QuestionLang

            Create.Table("QuestionLangs")
                 .WithColumn("QuestionLangId")
                 .AsInt32()
                 .PrimaryKey()
                 .Identity()

                 .WithColumn("Question")
                 .AsString(200)
                 .NotNullable()

                 .WithColumn("LanguageId")
                 .AsByte()
                 .NotNullable()

                 .WithColumn("QuestionId")
                 .AsInt32()
                 .NotNullable()
                 .ForeignKey("Questions", "QuestionId")

                 .WithColumn("ModifiedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable()

                 .WithColumn("CreatedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable();

            #endregion QuestionLang
        }

        public override void Down()
        {
            Delete.Table("Questions");
            Delete.Table("QuestionInfos");
            Delete.Table("AskedQuestions");
            Delete.Table("QuestionAnswers");
            Delete.Table("QuestionLangs");
        }
    }
}