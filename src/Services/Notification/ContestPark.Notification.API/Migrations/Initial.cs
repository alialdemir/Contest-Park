using System.Reflection;
using ContestPark.Core.Dapper.Extensions;
using FluentMigrator;

namespace ContestPark.Notification.API.Migrations
{
    [Migration(20200311)]
    public class Initial : Migration
    {
        public override void Up()
        {
            this.CreateTableIfNotExists("NotificationTypes", table =>
          table
                .WithColumn("NotificationTypeId")
                .AsByte()
                .PrimaryKey()
                .Identity()

                .WithColumn("IsActive")
                .AsBoolean()
                .WithDefaultValue(true)

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("Notifications", table =>
          table
                .WithColumn("NotificationId")
                .AsInt32()
                .PrimaryKey()
                .Identity()

                .WithColumn("WhonId")
                .AsString(255)
                .NotNullable()

                .WithColumn("WhoId")
                .AsString(255)
                .NotNullable()

                .WithColumn("IsNotificationSeen")
                .AsBoolean()
                .WithDefaultValue(false)

                .WithColumn("NotificationType")
                .AsByte()
                .ForeignKey("FK_Notifications_NotificationTypes", "NotificationTypes", "NotificationTypeId")
                .NotNullable()

                .WithColumn("PostId")
                .AsInt32()
                .Nullable()

                .WithColumn("Link")
                .AsString()
                .Nullable()

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("NotificationTypeLocalizeds", table =>
          table
                .WithColumn("NotificationLocalizedId")
                .AsInt16()
                .PrimaryKey()
                .Identity()

                .WithColumn("Description")
                .AsString(500)
                .NotNullable()

                .WithColumn("Language")
                .AsByte()
                .NotNullable()

                .WithColumn("NotificationType")
                .AsByte()
                .ForeignKey("FK_NotificationTypeLocalizeds_NotificationTypes", "NotificationTypes", "NotificationTypeId")
                .NotNullable()

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            Execute.ExecuteScripts(Assembly.GetExecutingAssembly(), "SP_Notifications.sql");
        }

        public override void Down()
        {
        }
    }
}
