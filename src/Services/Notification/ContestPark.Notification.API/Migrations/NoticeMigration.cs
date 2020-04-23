using ContestPark.Core.Dapper.Extensions;
using FluentMigrator;

namespace ContestPark.Notification.API.Migrations
{
    [Migration(20200423)]
    public class NoticeMigration : Migration
    {
        public override void Up()
        {
            this.CreateTableIfNotExists("Notices", table =>
          table
                .WithColumn("NoticeId")
                .AsInt32()
                .PrimaryKey()
                .Identity()

                .WithColumn("PicturePath")
                .AsString(255)
                .NotNullable()

                .WithColumn("Link")
                .AsString(255)
                .NotNullable()

                .WithColumn("Language")
                .AsByte()
                .NotNullable()

                .WithColumn("IsActice")
                .AsBoolean()
                .WithDefaultValue(false)

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
            Delete.Table("Notices");
        }
    }
}
