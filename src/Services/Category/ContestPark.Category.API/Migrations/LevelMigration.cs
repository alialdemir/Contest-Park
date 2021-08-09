using System.Reflection;
using ContestPark.Core.Dapper.Extensions;
using FluentMigrator;

namespace ContestPark.Category.API.Migrations
{
    [Migration(20200618)]
    public class LevelMigration : Migration
    {
        public override void Up()
        {
            this.CreateTableIfNotExists("LevelUps", table =>
          table
                .WithColumn("Level")
                .AsInt16()
                .PrimaryKey()

                .WithColumn("Exp")
                .AsInt32()

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("UserLevels", table =>
          table
                .WithColumn("UserLevelId")
                .AsInt32()
                .PrimaryKey()
                .Identity()

                .WithColumn("UserId")
                .AsString(255)
                .NotNullable()
                .Indexed("UserId_Index")

                .WithColumn("Level")
                .AsInt16()
                .WithDefaultValue(1)

                .WithColumn("Exp")
                .AsInt32()
                .WithDefaultValue(0)

                .WithColumn("SubCategoryId")
                .AsInt16()
                .NotNullable()
                .Indexed("SubCategoryId_Index")

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            Create.ForeignKey()
                        .FromTable("UserLevels")
                        .ForeignColumn("Level")
                        .ToTable("LevelUps")
                        .PrimaryColumn("Level");

            Execute.ExecuteScripts(Assembly.GetExecutingAssembly(),
                                "SP_UpdateLevel.sql"
                              );
        }

        public override void Down()
        {
        }
    }
}
