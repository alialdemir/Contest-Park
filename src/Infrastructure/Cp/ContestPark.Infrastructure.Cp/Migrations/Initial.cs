using FluentMigrator;

namespace ContestPark.Infrastructure.Cp.Migrations
{
    [Migration(3)]
    public class Initial : Migration
    {
        public override void Up()
        {
            #region Cps

            Create.Table("Cps")

                .WithColumn("UserId")
                .AsString(450)
                .PrimaryKey()
                .NotNullable()
                //      .ForeignKey("AspNetUsers", "Id")

                .WithColumn("CpAmount")
                .AsInt32()
                .NotNullable()
                .WithDefaultValue(0)

                 .WithColumn("ModifiedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable()

                 .WithColumn("CreatedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable();

            #endregion Cps

            #region CpInfos

            Create.Table("CpInfos")
                .WithColumn("CpInfoId")
                .AsInt64()
                .PrimaryKey()
                .Identity()

                .WithColumn("UserId")
                .AsString(450)
                .NotNullable()
                //   .ForeignKey("AspNetUsers", "Id")

                .WithColumn("CpSpent")
                .AsInt64()
                .NotNullable()

                .WithColumn("ChipProcessName")
                .AsByte()
                .NotNullable()

                 .WithColumn("ModifiedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable()

                 .WithColumn("CreatedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable();

            #endregion CpInfos
        }

        public override void Down()
        {
            Delete.Table("CpInfos");
            Delete.Table("Cps");
        }
    }
}