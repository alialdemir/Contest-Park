using System.Reflection;
using ContestPark.Core.Dapper.Extensions;
using FluentMigrator;

namespace ContestPark.Follow.API.Migrations
{
    [Migration(20190704)]
    public class Initial : Migration
    {
        public override void Up()
        {
            this.CreateTableIfNotExists("Follows", table =>
            table
                .WithColumn("FollowId")
                .AsInt64()
                .PrimaryKey()
                .Identity()

                .WithColumn("FollowUpUserId")
                .AsString(255)
                .Indexed("Indexed_FollowUpUserId")
                .NotNullable()

                .WithColumn("FollowedUserId")
                .AsString(255)
                .Indexed("Indexed_FollowedUserId")
                .NotNullable()

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            Execute.ExecuteScripts(Assembly.GetExecutingAssembly(),
                                   "FNC_IsFollow.sql");
        }

        public override void Down()
        {
            Delete.Table("Follows");
        }
    }
}
