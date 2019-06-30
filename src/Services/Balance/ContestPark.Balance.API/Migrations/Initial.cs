using ContestPark.Core.Dapper.Extensions;
using FluentMigrator;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Infrastructure;
using System;
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
                .AsDecimal()
                .NotNullable()
                .WithDefaultValue(0)

                .WithColumn("Money")
                .AsDecimal()
                .NotNullable()
                .WithDefaultValue(0)

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()
                .WithDefault(SystemMethods.CurrentDateTime)

                .WithColumn("CreatedDate")
                .AsDateTime()
                .Nullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("BalanceHistories", table =>
            table
                .WithColumn("Id")
                .AsInt64()
                .PrimaryKey()
                .Identity()

                .WithColumn("UserId")
                .AsString(255)
                .NotNullable()

                .WithColumn("OldAmount")
                .AsDecimal()
                .NotNullable()

                .WithColumn("NewAmount")
                .AsDecimal()
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
                .WithDefault(SystemMethods.CurrentDateTime)

                .WithColumn("CreatedDate")
                .AsDateTime()
                .Nullable()
                .WithDefault(SystemMethods.CurrentDateTime)
            );

            this.CreateTableIfNotExists("PurchaseHistories", table =>
            table

                .WithColumn("Id")
                .AsInt64()
                .PrimaryKey()
                .Identity()

                .WithColumn("UserId")
                .AsString(255)
                .NotNullable()

                .WithColumn("Amount")
                .AsDecimal()
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
                .WithDefault(SystemMethods.CurrentDateTime)

                .WithColumn("CreatedDate")
                .AsDateTime()
                .Nullable()
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

    public static class Ex
    {
        public static IFluentSyntax CreateTableIfNotExists(this MigrationBase self, string tableName, Func<ICreateTableWithColumnOrSchemaOrDescriptionSyntax, IFluentSyntax> constructTableFunction, string schemaName = "dbo")
        {
            if (!self.Schema.Schema(schemaName).Table(tableName).Exists())
            {
                return constructTableFunction(self.Create.Table(tableName));
            }
            else
                return null;
        }
    }
}
