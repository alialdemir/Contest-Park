using System.Reflection;
using ContestPark.Core.Dapper.Extensions;
using FluentMigrator;

namespace ContestPark.Mission.API.Migrations
{
    [Migration(20200509)]
    public class Initial : Migration
    {
        public override void Up()
        {
            this.CreateTableIfNotExists("Missions", table =>
        table
              .WithColumn("MissionId")
              .AsByte()
              .PrimaryKey()
              .Identity()

              .WithColumn("Reward")
              .AsDecimal(13, 2)
              .NotNullable()

              .WithColumn("RewardBalanceType")
              .AsByte()
              .NotNullable()

              .WithColumn("MissionTime")
              .AsByte()
              .NotNullable()

              .WithColumn("PicturePath")
              .AsString(255)
              .NotNullable()

              .WithColumn("Visibility")
              .AsBoolean()
              .WithDefaultValue(true)

              .WithColumn("ModifiedDate")
              .AsDateTime()
              .Nullable()

              .WithColumn("CreatedDate")
              .AsDateTime()
              .NotNullable()
              .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("MissionLocalizeds", table =>
        table
              .WithColumn("MissionLocalizedId")
              .AsInt16()
              .PrimaryKey()
              .Identity()

              .WithColumn("MissionId")
              .AsByte()
              .NotNullable()

              .WithColumn("Title")
              .AsString(255)
              .NotNullable()

              .WithColumn("Description")
              .AsString(500)
              .NotNullable()

              .WithColumn("Language")
              .AsByte()
              .NotNullable()

              .WithColumn("ModifiedDate")
              .AsDateTime()
              .Nullable()

              .WithColumn("CreatedDate")
              .AsDateTime()
              .NotNullable()
              .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("CompletedMissions", table =>
        table
              .WithColumn("CompletedMissionId")
              .AsInt32()
              .PrimaryKey()
              .Identity()

              .WithColumn("MissionId")
              .AsByte()
              .NotNullable()

              .WithColumn("UserId")
              .AsString(255)
              .Indexed("Indexed_UserId")
              .NotNullable()

              .WithColumn("MissionComplate")
              .AsBoolean()
              .NotNullable()

              .WithColumn("ModifiedDate")
              .AsDateTime()
              .Nullable()

              .WithColumn("CreatedDate")
              .AsDateTime()
              .NotNullable()
              .WithDefault(SystemMethods.CurrentDateTime));

            Execute.ExecuteScripts(Assembly.GetExecutingAssembly(),
                                   "FNC_IsMissionCompleted.sql",
                                   "FNC_Mission1-2-3.sql",
                                   "SP_Check_Mission.sql",
                                   "SP_Missions.sql",
                                   "SP_TakesMissionReward.sql"
                                   );
        }

        public override void Down()
        {
        }
    }
}
