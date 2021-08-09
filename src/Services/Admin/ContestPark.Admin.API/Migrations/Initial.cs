using ContestPark.Core.Dapper.Extensions;
using FluentMigrator;
using System.Reflection;

namespace ContestPark.Admin.API.Migrations
{
    [Migration(20210731)]
    public class Initial: Migration
    {
        public override void Up()
        {
            Execute.ExecuteScripts(Assembly.GetExecutingAssembly(),
                                  "Admin_SP_AvgBet.sql",
                                  "Admin_SP_DeleteQuestions.sql",
                                  "Admin_SP_NewPost.sql",
                                  "Admin_SP_NewUsers.sql"
                                  );
        }

        public override void Down()
        {
        }
    }
}
