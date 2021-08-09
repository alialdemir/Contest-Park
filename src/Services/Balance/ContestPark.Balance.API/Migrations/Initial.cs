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

                .WithColumn("VerifyPurchase")
                .AsString(36)
                .Nullable()

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

                .WithColumn("State")
                .AsByte()
                .WithDefaultValue(0)
                .NotNullable()

                 .WithColumn("TransactionId")
                 .AsString(255)
                 .NotNullable()

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("MoneyWithdrawRequests", table =>
            table
                .WithColumn("MoneyWithdrawRequestId")
                .AsInt32()
                .PrimaryKey()
                .Identity()

                .WithColumn("Amount")
                .AsDecimal(13, 2)
                .NotNullable()
                .WithDefaultValue(0)

                .WithColumn("UserId")
                .AsString(255)
                .Indexed("Indexed_UserId")
                .NotNullable()

                .WithColumn("IbanNo")
                .AsString(26)
                .NotNullable()

                .WithColumn("Status")
                .AsByte()
                .NotNullable()
                .WithDefaultValue(1)

                .WithColumn("FullName")
                .AsString(255)
                .NotNullable()

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("References", table =>
            table

                .WithColumn("ReferenceId")
                .AsInt32()
                .PrimaryKey()
                .Identity()

                .WithColumn("Code")
                .AsString(255)
                .NotNullable()

                .WithColumn("BalanceType")
                .AsByte()
                .NotNullable()

                 .WithColumn("Amount")
                .AsDecimal(13, 2)
                .NotNullable()

                 .WithColumn("Menstruation")
                 .AsInt32()
                 .NotNullable()

                 .WithColumn("FinishDate")
                 .AsDateTime()
                 .Nullable());

            this.CreateTableIfNotExists("ReferenceCodes", table =>
            table

                .WithColumn("ReferenceCodeId")
                .AsInt32()
                .PrimaryKey()
                .Identity()

                .WithColumn("ReferenceUserId")
                .AsString(450)

                .WithColumn("NewUserId")
                .AsString(450)
                .NotNullable()

                .WithColumn("Code")
                .AsString(255)
                .NotNullable());

            Execute.ExecuteScripts(Assembly.GetExecutingAssembly(),
                                   "FNC_FirstLoadMoney.sql",
                                   "GetBalance.sql",
                                   "IsCodeActive.sql",
                                   "SP_IsExistsToken.sql",
                                   "UpdateBalance.sql");
        }

        public override void Down()
        {
            Delete.Table("Balances");
            Delete.Table("BalanceHistories");
            Delete.Table("PurchaseHistories");
        }
    }
}
