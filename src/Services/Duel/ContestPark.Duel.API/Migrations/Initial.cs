using ContestPark.Core.Dapper.Extensions;
using ContestPark.Core.Database.Enums;
using FluentMigrator;
using System.Reflection;

namespace ContestPark.Duel.API.Migrations
{
    [Migration(20190715)]
    public class Initial : Migration
    {
        public override void Up()
        {
            this.CreateTableIfNotExists("ScoreRankings", table =>
           table

               .WithColumn("ScoreRankingId")
               .AsInt32()
               .PrimaryKey()
               .Identity()

               .WithColumn("ContestDateId")
               .AsInt16()
               .NotNullable()

               .WithColumn("SubCategoryId")
               .AsInt16()
               .NotNullable()

               .WithColumn("UserId")
               .AsString(255)
               .NotNullable()

               .WithColumn("DisplayTotalMoneyScore")
               .AsString(50)
               .NotNullable()

               .WithColumn("TotalMoneyScore")
               .AsInt32()
               .NotNullable()

               .WithColumn("DisplayTotalGoldScore")
               .AsString(50)
               .NotNullable()

               .WithColumn("TotalGoldScore")
               .AsInt32()
               .NotNullable()

               .WithColumn("EntityStatus")
               .AsByte()
               .WithDefaultValue((byte)EntityStatus.Active)

               .WithColumn("ModifiedDate")
               .AsDateTime()
               .Nullable()

               .WithColumn("CreatedDate")
               .AsDateTime()
               .NotNullable()
               .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("BalanceRankings", table =>
                     table

                         .WithColumn("BalanceRankingId")
                         .AsInt32()
                         .PrimaryKey()
                         .Identity()

                         .WithColumn("ContestDateId")
                         .AsInt16()
                         .NotNullable()

                         .WithColumn("UserId")
                         .AsString(255)
                         .NotNullable()

                         .WithColumn("DisplayTotalMoney")
                         .AsString(50)
                         .NotNullable()

                         .WithColumn("TotalMoney")
                         .AsDecimal(13, 2)
                         .NotNullable()

                         .WithColumn("DisplayTotalGold")
                         .AsString(50)
                         .NotNullable()

                         .WithColumn("TotalGold")
                         .AsInt32()
                         .NotNullable()

                         .WithColumn("EntityStatus")
                         .AsByte()
                         .WithDefaultValue((byte)EntityStatus.Active)

                         .WithColumn("ModifiedDate")
                         .AsDateTime()
                         .Nullable()

                         .WithColumn("CreatedDate")
                         .AsDateTime()
                         .NotNullable()
                         .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("ContestDates", table =>
           table

               .WithColumn("ContestDateId")
               .AsInt16()
               .PrimaryKey()
               .Identity()

               .WithColumn("StartDate")
               .AsDateTime()
               .NotNullable()

               .WithColumn("FinishDate")
               .AsDateTime()
               .NotNullable()

               .WithColumn("EntityStatus")
               .AsByte()
               .WithDefaultValue((byte)EntityStatus.Active)

               .WithColumn("ModifiedDate")
               .AsDateTime()
               .Nullable()

               .WithColumn("CreatedDate")
               .AsDateTime()
               .NotNullable()
               .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("Duels", table =>
           table

               .WithColumn("DuelId")
               .AsInt32()
               .PrimaryKey()
               .Identity()

               .WithColumn("Bet")
               .AsDecimal(13, 2)
               .NotNullable()

               .WithColumn("FounderUserId")
               .AsString(255)
               .NotNullable()

               .WithColumn("OpponentUserId")
               .AsString(255)
               .NotNullable()

               .WithColumn("SubCategoryId")
               .AsInt16()
               .NotNullable()

               .WithColumn("ContestDateId")
               .AsInt16()
               .NotNullable()

               .WithColumn("BalanceType")
               .AsByte()
               .NotNullable()

               .WithColumn("DuelType")
               .AsByte()
               .NotNullable()

               .WithColumn("FounderTotalScore")
               .AsByte()
               .Nullable()

               .WithColumn("OpponentTotalScore")
               .AsByte()
               .Nullable()

               .WithColumn("FounderFinishScore")
               .AsByte()
               .Nullable()

               .WithColumn("OpponentFinishScore")
               .AsByte()
               .Nullable()

               .WithColumn("OpponentVictoryScore")
               .AsByte()
               .Nullable()

               .WithColumn("FounderVictoryScore")
               .AsByte()
               .Nullable()

               .WithColumn("BetCommission")
               .AsByte()
               .NotNullable()

               .WithColumn("EntityStatus")
               .AsByte()
               .WithDefaultValue((byte)EntityStatus.Active)

               .WithColumn("ModifiedDate")
               .AsDateTime()
               .Nullable()

               .WithColumn("CreatedDate")
               .AsDateTime()
               .NotNullable()
               .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("DuelDetails", table =>
           table

               .WithColumn("DuelDetailId")
               .AsInt32()
               .PrimaryKey()
               .Identity()

               .WithColumn("DuelId")
               .AsInt32()
               .NotNullable()

               .WithColumn("QuestionId")
               .AsInt32()
               .NotNullable()

               .WithColumn("CorrectAnswer")
               .AsByte()
               .NotNullable()

               .WithColumn("FounderAnswer")
               .AsByte()
               .Nullable()

               .WithColumn("OpponentAnswer")
               .AsByte()
               .Nullable()

               .WithColumn("FounderTime")
               .AsByte()
               .Nullable()

               .WithColumn("OpponentTime")
               .AsByte()
               .Nullable()

               .WithColumn("FounderScore")
               .AsByte()
               .Nullable()

               .WithColumn("OpponentScore")
               .AsByte()
               .Nullable()

               .WithColumn("EntityStatus")
               .AsByte()
               .WithDefaultValue((byte)EntityStatus.Active)

               .WithColumn("ModifiedDate")
               .AsDateTime()
               .Nullable()

               .WithColumn("CreatedDate")
               .AsDateTime()
               .NotNullable()
               .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("Questions", table =>
           table

               .WithColumn("QuestionId")
               .AsInt32()
               .PrimaryKey()
               .Identity()

               .WithColumn("Link")
               .AsString(500)
               .NotNullable()

               .WithColumn("AnswerType")
               .AsByte()
               .NotNullable()

               .WithColumn("QuestionType")
               .AsByte()
               .NotNullable()

               .WithColumn("IsActive")
               .AsBoolean()
               .NotNullable()

               .WithColumn("SubCategoryId")
               .AsInt16()
               .NotNullable()

               .WithColumn("EntityStatus")
               .AsByte()
               .WithDefaultValue((byte)EntityStatus.Active)

               .WithColumn("ModifiedDate")
               .AsDateTime()
               .Nullable()

               .WithColumn("CreatedDate")
               .AsDateTime()
               .NotNullable()
               .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("QuestionLocalizeds", table =>
           table

               .WithColumn("QuestionLocalizedId")
               .AsInt32()
               .PrimaryKey()
               .Identity()

               .WithColumn("Question")
               .AsString(200)
               .NotNullable()

               .WithColumn("Language")
               .AsByte()
               .NotNullable()

               .WithColumn("EntityStatus")
               .AsByte()
               .WithDefaultValue((byte)EntityStatus.Active)

               .WithColumn("ModifiedDate")
               .AsDateTime()
               .Nullable()

               .WithColumn("CreatedDate")
               .AsDateTime()
               .NotNullable()
               .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("QuestionOfQuestionLocalizeds", table =>
           table

               .WithColumn("QuestionOfQuestionLocalizedId")
               .AsInt32()
               .PrimaryKey()
               .Identity()

               .WithColumn("QuestionId")
               .AsInt32()
               .NotNullable()

               .WithColumn("QuestionLocalizedId")
               .AsInt32()
               .NotNullable()

               .WithColumn("ModifiedDate")
               .AsDateTime()
               .Nullable()

               .WithColumn("CreatedDate")
               .AsDateTime()
               .NotNullable()
               .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("AnswerLocalizeds", table =>
           table

               .WithColumn("AnswerLocalizedId")
               .AsInt32()
               .PrimaryKey()
               .Identity()

               .WithColumn("CorrectStylish")
               .AsString(100)
               .NotNullable()

               .WithColumn("Stylish1")
               .AsString(100)
               .NotNullable()

               .WithColumn("Stylish2")
               .AsString(100)
               .NotNullable()

               .WithColumn("Stylish3")
               .AsString(100)
               .NotNullable()

               .WithColumn("Language")
               .AsBoolean()
               .NotNullable()

               .WithColumn("QuestionId")
               .AsInt32()
               .NotNullable()

               .WithColumn("EntityStatus")
               .AsByte()
               .WithDefaultValue((byte)EntityStatus.Active)

               .WithColumn("ModifiedDate")
               .AsDateTime()
               .Nullable()

               .WithColumn("CreatedDate")
               .AsDateTime()
               .NotNullable()
               .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("AskedQuestions", table =>
           table

               .WithColumn("AskedQuestionId")
               .AsInt32()
               .PrimaryKey()
               .Identity()

               .WithColumn("QuestionId")
               .AsInt32()
               .NotNullable()

               .WithColumn("UserId")
               .AsString(255)
               .NotNullable()

               .WithColumn("SubCategoryId")
               .AsInt32()
               .NotNullable()

               .WithColumn("ModifiedDate")
               .AsDateTime()
               .Nullable()

               .WithColumn("CreatedDate")
               .AsDateTime()
               .NotNullable()
               .WithDefault(SystemMethods.CurrentDateTime));

            Create.ForeignKey()
                        .FromTable("ScoreRankings")
                        .ForeignColumn("ContestDateId")
                        .ToTable("ContestDates")
                        .PrimaryColumn("ContestDateId");

            Create.ForeignKey()
                        .FromTable("BalanceRankings")
                        .ForeignColumn("ContestDateId")
                        .ToTable("ContestDates")
                        .PrimaryColumn("ContestDateId");

            Create.ForeignKey()
                        .FromTable("Duels")
                        .ForeignColumn("ContestDateId")
                        .ToTable("ContestDates")
                        .PrimaryColumn("ContestDateId");

            Create.ForeignKey()
                        .FromTable("DuelDetails")
                        .ForeignColumn("DuelId")
                        .ToTable("Duels")
                        .PrimaryColumn("DuelId");

            Create.ForeignKey()
                        .FromTable("DuelDetails")
                        .ForeignColumn("QuestionId")
                        .ToTable("Questions")
                        .PrimaryColumn("QuestionId");

            Create.ForeignKey()
                        .FromTable("AnswerLocalizeds")
                        .ForeignColumn("QuestionId")
                        .ToTable("Questions")
                        .PrimaryColumn("QuestionId");

            Create.ForeignKey()
                        .FromTable("QuestionOfQuestionLocalizeds")
                        .ForeignColumn("QuestionId")
                        .ToTable("Questions")
                        .PrimaryColumn("QuestionId");

            Create.ForeignKey()
                        .FromTable("AskedQuestions")
                        .ForeignColumn("QuestionId")
                        .ToTable("Questions")
                        .PrimaryColumn("QuestionId");

            //Create.ForeignKey()
            //            .FromTable("QuestionOfQuestionLocalizeds")
            //            .ForeignColumn("QuestionLocalizedId")
            //            .ToTable("QuestionLocalizeds")
            //            .PrimaryColumn("QuestionLocalizedId");


            Execute.ExecuteScripts(Assembly.GetExecutingAssembly(),
                                   "SP_DuelResult.sql",
                                   "SP_GetRankingAllTimes.sql",
                                   "SP_GetRankingBySubCategoryId.sql",
                                   "SP_RandomQuestions.sql",
                                   "SP_WinStatus.sql");
        }

        public override void Down()
        {
            Delete.Table("ScoreRankings");
            Delete.Table("BalanceRankings");
            Delete.Table("ContestDates");
        }
    }
}
