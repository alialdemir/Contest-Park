using ContestPark.Core.Dapper.Extensions;
using FluentMigrator;
using System.Reflection;

namespace ContestPark.Balance.API.Migrations
{
    [Migration(20190630)]
    public class Initial : Migration
    {
        public override void Up()
        {
            this.CreateTableIfNotExists("Balances", table =>
            table
                .WithColumn("UserId")
                .AsString(255)
                .Indexed("Indexed_UserId")
                .Unique()
                .NotNullable()

                .WithColumn("Gold")
                .AsDecimal(13, 2)
                .NotNullable()
                .WithDefaultValue(0)

                .WithColumn("Money")
                .AsDecimal(13, 2)
                .NotNullable()
                .WithDefaultValue(0)

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("BalanceHistories", table =>
            table
                .WithColumn("BalanceHistoryId")
                .AsInt64()
                .PrimaryKey()
                .Identity()

                .WithColumn("UserId")
                .AsString(255)
                .Indexed("Indexed_UserId")
                .NotNullable()

                .WithColumn("OldAmount")
                .AsDecimal(13, 2)
                .NotNullable()

                .WithColumn("NewAmount")
                .AsDecimal(13, 2)
                .NotNullable()

                .WithColumn("BalanceHistoryType")
                .AsByte()
                .NotNullable()

                .WithColumn("BalanceType")
                .AsByte()
                .NotNullable()

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime)
            );

            this.CreateTableIfNotExists("PurchaseHistories", table =>
            table

                .WithColumn("PurchaseHistoryId")
                .AsInt64()
                .PrimaryKey()
                .Identity()

                .WithColumn("UserId")
                .AsString(255)
                .Indexed("Indexed_UserId")
                .NotNullable()

                .WithColumn("Amount")
                .AsDecimal(13, 2)
                .NotNullable()

                 .WithColumn("PackageName")
                 .AsString(250)
                 .NotNullable()

                 .WithColumn("ProductId")
                 .AsString(350)
                 .NotNullable()

                 .WithColumn("Token")
                 .AsString()
                 .NotNullable()

                .WithColumn("BalanceType")
                .AsByte()
                .NotNullable()

                .WithColumn("Platform")
                .AsByte()
                .NotNullable()

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            Execute.ExecuteScripts(Assembly.GetExecutingAssembly(), "UpdateBalance.sql", "GetBalance.sql");
        }

        public override void Down()
        {
            Delete.Table("Balances");
            Delete.Table("BalanceHistories");
            Delete.Table("PurchaseHistories");
        }
    }
}
