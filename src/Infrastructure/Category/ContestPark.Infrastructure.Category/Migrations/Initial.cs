using FluentMigrator;

namespace ContestPark.Infrastructure.Category.Migrations
{
    [Migration(2)]
    public class Initial : Migration
    {
        public override void Up()
        {
            #region Categories

            Create.Table("Categories")
                 .WithColumn("CategoryId")
                 .AsInt16()
                 .PrimaryKey()
                 .Identity()

                 .WithColumn("Visibility")
                 .AsBoolean()
                 .NotNullable()
                 .WithDefaultValue(true)

                 .WithColumn("Order")
                 .AsByte()
                 .NotNullable()
                 .WithDefaultValue(0)

                 .WithColumn("Color")
                 .AsString(7)
                 .NotNullable()

                 .WithColumn("ModifiedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable()

                 .WithColumn("CreatedDate")
                 .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                 .NotNullable();

            #endregion Categories

            #region CategoryLangs

            Create.Table("CategoryLangs")
                .WithColumn("CategoryLangId")
                .AsInt16()
                .PrimaryKey()
                .Identity()

                .WithColumn("CategoryName")
                .AsString(256)
                .NotNullable()

                .WithColumn("CategoryId")
                .AsInt16()
                .ForeignKey("Categories", "CategoryId")
                .NotNullable()

                .WithColumn("LanguageId")
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

            #endregion CategoryLangs

            #region SubCategories

            Create.Table("SubCategories")
                .WithColumn("SubCategoryId")
                 .AsInt16()
                 .PrimaryKey()
                 .Identity()

                .WithColumn("CategoryId")
                .AsInt16()
                .NotNullable()
                .ForeignKey("Categories", "CategoryId")

                .WithColumn("PictuePath")
                .AsString(250)
                .NotNullable()

                .WithColumn("Visibility")
                .AsBoolean()
                .WithDefaultValue(true)

                .WithColumn("Order")
                .AsByte()
                .NotNullable()
                .WithDefaultValue(0)

                .WithColumn("Price")
                .AsInt64()
                .NotNullable()
                .WithDefaultValue(0)

                .WithColumn("DisplayPrice")
                .AsString(5)
                .NotNullable()

                .WithColumn("ModifiedDate")
                .AsDateTime2()
                .WithDefault(SystemMethods.CurrentDateTimeOffset)
                .NotNullable()

                .WithColumn("CreatedDate")
                .AsDateTime2()
                .WithDefault(SystemMethods.CurrentDateTimeOffset)
                .NotNullable();

            #endregion SubCategories

            #region SubCategoryLangs

            Create.Table("SubCategoryLangs")
                .WithColumn("SubCategoryLangId")
                 .AsInt16()
                 .PrimaryKey()
                 .Identity()

                 .WithColumn("SubCategoryName")
                 .AsString(250)
                 .NotNullable()

                .WithColumn("LanguageId")
                .AsByte()
                .NotNullable()

                .WithColumn("SubCategoryId")
                .AsInt16()
                .NotNullable()
                .ForeignKey("SubCategories", "SubCategoryId")

                .WithColumn("ModifiedDate")
                .AsDateTime2()
                .WithDefault(SystemMethods.CurrentDateTimeOffset)
                .NotNullable()

                .WithColumn("CreatedDate")
                .AsDateTime2()
                .WithDefault(SystemMethods.CurrentDateTimeOffset)
                .NotNullable();

            #endregion SubCategoryLangs

            #region OpenSubCategories

            Create.Table("OpenSubCategories")
                .WithColumn("OpenSubCategoryId")
                .AsInt32()
                .PrimaryKey()
                .Identity()

                .WithColumn("UserId")
                .AsString(450)
                .NotNullable()
                //  .ForeignKey("AspNetUsers", "Id")

                .WithColumn("SubCategoryId")
                .AsInt16()
                .NotNullable()
                .ForeignKey("SubCategories", "SubCategoryId")

                .WithColumn("ModifiedDate")
                .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                .NotNullable()

                .WithColumn("CreatedDate")
                .AsDateTime2()
                 .WithDefault(SystemMethods.CurrentDateTimeOffset)
                .NotNullable();

            #endregion OpenSubCategories
        }

        public override void Down()
        {
            Delete.Table("Categories");
            Delete.Table("CategoryLangs");
            Delete.Table("SubCategories");
            Delete.Table("OpenSubCategories");
            Delete.Table("SubCategoryLangs");
        }
    }
}