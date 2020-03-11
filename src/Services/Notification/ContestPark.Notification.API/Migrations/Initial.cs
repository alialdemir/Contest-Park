using ContestPark.Core.Dapper.Extensions;
using FluentMigrator;

namespace ContestPark.Notification.API.Migrations
{
    [Migration(20200311)]
    public class Initial : Migration
    {
        public override void Up()
        {
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
                .NotNullable()

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            Create.ForeignKey()
                        .FromTable("Notifications")// TODO: burada fk adı çok uzun olduğu için hata veriyor
                        .ForeignColumn("NotificationType")
                        .ToTable("NotificationTypes")
                        .PrimaryColumn("NotificationTypeId");

            Create.ForeignKey()
                        .FromTable("NotificationTypeLocalizeds")
                        .ForeignColumn("NotificationType")
                        .ToTable("NotificationTypes")
                        .PrimaryColumn("NotificationTypeId");
        }

        public override void Down()
        {
        }
    }
}
