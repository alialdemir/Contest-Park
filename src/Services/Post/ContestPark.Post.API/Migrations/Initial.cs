using ContestPark.Core.Dapper.Extensions;
using FluentMigrator;
using System.Reflection;

namespace ContestPark.Post.API.Migrations
{
    [Migration(2201906)]
    public class Initial : Migration
    {
        public override void Up()
        {
            Execute.ExecuteScripts(Assembly.GetExecutingAssembly(), "PostIsLike.sql", "PostLike.sql", "PostUnLike.sql");

            this.CreateTableIfNotExists("Posts", table =>
            table

            #region Post

                .WithColumn("PostId")
                .AsInt32()
                .PrimaryKey()
                .Identity()

                .WithColumn("CommentCount")
                .AsInt32()
                .Nullable()

                .WithColumn("LikeCount")
                .AsInt32()
                .Nullable()

                .WithColumn("OwnerUserId")
                .AsString(255)
                .NotNullable()

                .WithColumn("PostType")
                .AsByte()
                .NotNullable()

            #endregion Post

            #region PostConstest

                .WithColumn("Bet")
                .AsDecimal(13, 2)
                .Nullable()

                .WithColumn("DuelId")
                .AsInt32()
                .Nullable()

                .WithColumn("SubCategoryId")
                .AsInt32()
                .Nullable()

                .WithColumn("CompetitorUserId")
                .AsString(256)
                .Nullable()

                .WithColumn("CompetitorTrueAnswerCount")
                .AsByte()
                .Nullable()

                .WithColumn("FounderUserId")
                .AsString(256)
                .Nullable()

                .WithColumn("FounderTrueAnswerCount")
                .AsByte()
                .Nullable()

            #endregion PostConstest

            #region Post image

                .WithColumn("PicturePath")
                .AsString(500)
                .Nullable()

                .WithColumn("PostImageType")
                .AsByte()
                .Nullable()

            #endregion Post image

            #region Post text

                .WithColumn("Description")
                .AsString(500)
                .Nullable()

            #endregion Post text

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("Likes", table =>
            table

                .WithColumn("LikeId")
                .AsInt32()
                .PrimaryKey()
                .Identity()

                .WithColumn("UserId")
                .AsString(255)
                .NotNullable()

                .WithColumn("PostId")
                .AsInt32()
                .NotNullable()

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            Create.ForeignKey()
                        .FromTable("Likes")
                        .ForeignColumn("PostId")
                        .ToTable("Posts")
                        .PrimaryColumn("PostId");
        }

        public override void Down()
        {
            Delete.Table("Likes");
            Delete.Table("Posts");
        }
    }
}
