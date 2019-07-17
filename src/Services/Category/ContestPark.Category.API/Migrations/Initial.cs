﻿using ContestPark.Core.Dapper.Extensions;
using FluentMigrator;
using System.Reflection;

namespace ContestPark.Category.API.Migrations
{
    [Migration(20190631)]
    public class Initial : Migration
    {
        public override void Up()
        {
            Execute.ExecuteScripts(Assembly.GetExecutingAssembly(),
                                   "ChangeFollowersCount.sql",
                                   "SP_GetSubCategoryDetail",
                                   "SP_GetFollowedSubCategories",
                                   "GetCategories.sql");

            this.CreateTableIfNotExists("Categories", table =>
          table
                .WithColumn("CategoryId")
                .AsInt16()
                .PrimaryKey()
                .Identity()

                .WithColumn("Visibility")
                .AsBoolean()
                .WithDefaultValue(true)

                .WithColumn("DisplayOrder")
                .AsBoolean()
                .WithDefaultValue(0)

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("CategoryLocalizeds", table =>
                  table
                        .WithColumn("CategoryLocalizedId")
                        .AsInt16()
                        .PrimaryKey()
                        .Identity()

                        .WithColumn("CategoryId")
                        .AsInt16()
                        .NotNullable()

                        .WithColumn("Text")
                        .AsString(250)
                        .NotNullable()

                        .WithColumn("Language")
                        .AsByte()
                        .NotNullable()

                        .WithColumn("ModifiedDate")
                        .AsDateTime()
                        .Nullable()

                        .WithColumn("CreatedDate")
                        .AsDateTime()
                        .NotNullable()
                        .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("FollowSubCategories", table =>
                  table
                        .WithColumn("FollowSubCategoryId")
                        .AsInt32()
                        .PrimaryKey()
                        .Identity()

                        .WithColumn("SubCategoryId")
                        .AsInt16()
                        .NotNullable()

                        .WithColumn("UserId")
                        .AsString(255)
                        .NotNullable()

                        .WithColumn("ModifiedDate")
                        .AsDateTime()
                        .Nullable()

                        .WithColumn("CreatedDate")
                        .AsDateTime()
                        .NotNullable()
                        .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("OpenSubCategories", table =>
                  table
                        .WithColumn("OpenSubCategoryId")
                        .AsInt32()
                        .PrimaryKey()
                        .Identity()

                        .WithColumn("SubCategoryId")
                        .AsInt16()
                        .NotNullable()

                        .WithColumn("UserId")
                        .AsString(255)
                        .NotNullable()

                        .WithColumn("ModifiedDate")
                        .AsDateTime()
                        .Nullable()

                        .WithColumn("CreatedDate")
                        .AsDateTime()
                        .NotNullable()
                        .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("SubCategoryRls", table =>
                  table
                        .WithColumn("SubCategoriesOfCategoryId")
                        .AsInt32()
                        .PrimaryKey()
                        .Identity()

                        .WithColumn("SubCategoryId")
                        .AsInt16()
                        .NotNullable()

                        .WithColumn("CategoryId")
                        .AsInt16()
                        .NotNullable()

                        .WithColumn("ModifiedDate")
                        .AsDateTime()
                        .Nullable()

                        .WithColumn("CreatedDate")
                        .AsDateTime()
                        .NotNullable()
                        .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("SubCategories", table =>
                  table
                        .WithColumn("SubCategoryId")
                        .AsInt16()
                        .PrimaryKey()
                        .Identity()

                        .WithColumn("DisplayOrder")
                        .AsByte()
                        .WithDefaultValue(0)

                        .WithColumn("DisplayPrice")
                        .AsString(50)
                        .NotNullable()

                        .WithColumn("Price")
                        .AsDecimal(13, 2)
                        .NotNullable()

                        .WithColumn("FollowerCount")
                        .AsInt64()
                        .WithDefaultValue(0)

                        .WithColumn("PicturePath")
                        .AsString(900)
                        .NotNullable()

                        .WithColumn("Visibility")
                        .AsBoolean()
                        .WithDefaultValue(true)

                        .WithColumn("ModifiedDate")
                        .AsDateTime()
                        .Nullable()

                        .WithColumn("CreatedDate")
                        .AsDateTime()
                        .NotNullable()
                        .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("SubCategoryLangs", table =>
                  table
                        .WithColumn("SubCategoryLocalizedId")
                        .AsInt16()
                        .PrimaryKey()
                        .Identity()

                        .WithColumn("SubCategoryId")
                        .AsInt16()
                        .NotNullable()

                        .WithColumn("SubCategoryName")
                        .AsString(250)
                        .NotNullable()

                        .WithColumn("Description")
                        .AsString(250)
                        .NotNullable()

                        .WithColumn("Language")
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
                        .FromTable("CategoryLocalizeds")
                        .ForeignColumn("CategoryId")
                        .ToTable("Categories")
                        .PrimaryColumn("CategoryId");

            Create.ForeignKey()
                        .FromTable("FollowSubCategories")
                        .ForeignColumn("SubCategoryId")
                        .ToTable("SubCategories")
                        .PrimaryColumn("SubCategoryId");

            //Create.ForeignKey()
            //            .FromTable("OpenSubCategories")
            //            .ForeignColumn("UserId")
            //            .ToTable("AspNetUsers")
            //            .PrimaryColumn("Id");

            Create.ForeignKey()
                        .FromTable("OpenSubCategories")
                        .ForeignColumn("SubCategoryId")
                        .ToTable("SubCategories")
                        .PrimaryColumn("SubCategoryId");

            Create.ForeignKey()
                        .FromTable("SubCategoryRls")
                        .ForeignColumn("SubCategoryId")
                        .ToTable("SubCategories")
                        .PrimaryColumn("SubCategoryId");

            Create.ForeignKey()
                        .FromTable("SubCategoryRls")
                        .ForeignColumn("CategoryId")
                        .ToTable("Categories")
                        .PrimaryColumn("CategoryId");

            Create.ForeignKey()
                        .FromTable("SubCategoryLangs")
                        .ForeignColumn("SubCategoryId")
                        .ToTable("SubCategories")
                        .PrimaryColumn("SubCategoryId");
        }

        public override void Down()
        {
        }
    }
}