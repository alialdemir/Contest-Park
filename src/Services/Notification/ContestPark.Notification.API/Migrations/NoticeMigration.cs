using ContestPark.Core.Dapper.Extensions;
using FluentMigrator;

namespace ContestPark.Notification.API.Migrations
{
    [Migration(20200423)]
    public class NoticeMigration : Migration
    {
        public override void Up()
        {
        }

        public override void Down()
        {
            Delete.Table("Notices");
        }
    }
}
