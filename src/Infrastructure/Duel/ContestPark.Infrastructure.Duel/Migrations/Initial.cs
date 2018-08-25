using FluentMigrator;

namespace ContestPark.Infrastructure.Duel.Migrations
{
    [Migration(4)]
    public class Initial : Migration
    {
        public override void Up()
        {
            #region Duels

            Create.Table("Duels")
                 .WithColumn("DuelId")
                 .AsInt32()
                 .PrimaryKey()
                 .Identity()

                .WithColumn("FounderUserId")
                .AsString(450)
                .NotNullable()
                // .ForeignKey("AspNetUsers", "Id")

                .WithColumn("OpponentUserId")
                .AsString(450)
                .NotNullable()
                 //     .ForeignKey("AspNetUsers", "Id")

                 .WithColumn("SubCategoryId")
                 .AsString(20)
                 .NotNullable()
                 //    .ForeignKey("SubCategories", "SubCategoryId")

                 .WithColumn("Bet")
                 .AsInt64()
                 .NotNullable()
                 .WithDefaultValue(0)

                 .WithColumn("FounderTotalScore")
                 .AsByte()
                 .WithDefaultValue(0)

                 .WithColumn("OpponentTotalScore")
                 .AsByte()
                 .WithDefaultValue(0)

                 .WithColumn("ModifiedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable()

                 .WithColumn("CreatedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable();

            #endregion Duels

            #region DuelInfos

            Create.Table("DuelInfos")
                .WithColumn("DuelInfoId")
                .AsInt32()
                .PrimaryKey()
                .Identity()

                .WithColumn("DuelId")
                .AsInt32()
                .NotNullable()
                .ForeignKey("Duels", "DuelId")

                .WithColumn("QuestionInfoId")
                .AsInt32()
                .NotNullable()
            //    .ForeignKey("QuestionInfos", "QuestionInfoId")

                .WithColumn("FounderAnswer")
                .AsByte()
                .NotNullable()

                .WithColumn("OpponentAnswer")
                .AsByte()
                .NotNullable()

                .WithColumn("CorrectAnswer")
                .AsByte()
                .NotNullable()

                .WithColumn("FounderTime")
                .AsByte()
                .NotNullable()

                 .WithColumn("OpponentTime")
                 .AsByte()
                 .NotNullable()

                 .WithColumn("FounderScore")
                 .AsByte()
                 .WithDefaultValue(0)

                 .WithColumn("OpponentScore")
                 .AsByte()
                 .WithDefaultValue(0)

                 .WithColumn("ModifiedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable()

                 .WithColumn("CreatedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable();

            #endregion DuelInfos
        }

        public override void Down()
        {
            Delete.Table("DuelInfos");
            Delete.Table("Duels");
        }
    }
}