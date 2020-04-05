using ContestPark.Core.Dapper.Extensions;
using FluentMigrator;

namespace ContestPark.Notification.API.Migrations
{
    [Migration(20200405)]
    public class PushNotificationMigration : Migration
    {
        public override void Up()
        {
            this.CreateTableIfNotExists("PushNotifications", table =>
          table
                .WithColumn("UserId")
                .AsString(255)
                .Indexed("Indexed_UserId")
                .Unique()
                .NotNullable()

                .WithColumn("Token")
                .AsString()
                .NotNullable()

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));
        }

        public override void Down()
        {
            Delete.Table("PushNotifications");
        }
    }
}
