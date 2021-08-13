using ContestPark.Core.Dapper.Extensions;
using ContestPark.Core.Database.Enums;
using FluentMigrator;
using System.Reflection;

namespace ContestPark.Post.API.Migrations
{
    [Migration(2201906)]
    public class Initial : Migration
    {
        public override void Up()
        {
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

                .WithColumn("IsCommentOpen")
                .AsBoolean()
                .WithDefaultValue(true)

                .WithColumn("IsArchive")
                .AsBoolean()

                .WithColumn("PostType")
                .AsByte()
                .NotNullable()

            #endregion Post

            #region PostConstest

                .WithColumn("Bet")
                .AsDecimal(13, 2)
                .Nullable()

                .WithColumn("BalanceType")
                .AsByte()
                .NotNullable()

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

                .WithColumn("EntityStatus")
                .AsByte()
                .WithDefaultValue((byte)EntityStatus.Active)

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

            this.CreateTableIfNotExists("Comments", table =>
            table

                .WithColumn("CommentId")
                .AsInt32()
                .PrimaryKey()
                .Identity()

                .WithColumn("Text")
                .AsString(500)
                .NotNullable()

                .WithColumn("UserId")
                .AsString(255)
                .NotNullable()

                .WithColumn("PostId")
                .AsInt32()
                .NotNullable()

                .WithColumn("EntityStatus")
                .AsByte()
                .WithDefaultValue((byte)EntityStatus.Active)

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

            Create.ForeignKey()
                        .FromTable("Comments")
                        .ForeignColumn("PostId")
                        .ToTable("Posts")
                        .PrimaryColumn("PostId");

            Execute.ExecuteScripts(Assembly.GetExecutingAssembly(),
                                   "AddComment.sql",
                                   "FNC_PostIsLike.sql",
                                   "PostIsLike.sql",
                                   "PostLike.sql",
                                   "PostUnLike.sql",
                                   "SP_GetPostByUserId.sql",
                                   "SP_GetPostDetailByPostId.sql",
                                   "SP_GetPostsBySubcategoryId.sql"
                                   );
        }

        public override void Down()
        {
            Delete.Table("Likes");
            Delete.Table("Posts");
        }
    }
}
