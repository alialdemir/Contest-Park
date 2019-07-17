using ContestPark.Core.Dapper.Extensions;
using FluentMigrator;

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
        }

        public override void Down()
        {
            Delete.Table("ScoreRankings");
            Delete.Table("BalanceRankings");
            Delete.Table("ContestDates");
        }
    }
}
