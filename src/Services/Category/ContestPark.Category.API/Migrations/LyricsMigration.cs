using ContestPark.Core.Dapper.Extensions;
using ContestPark.Core.Database.Enums;
using FluentMigrator;

namespace ContestPark.Category.API.Migrations
{
    [Migration(20210813)]
    public class LyricsMigration : Migration
    {
        public override void Up()
        {
            this.CreateTableIfNotExists("Artists", table =>
          table
                .WithColumn("ArtistId")
                .AsInt16()
                .PrimaryKey()
                .Identity()

                .WithColumn("CategoryId")
                .AsInt16()
                .Indexed("Artists_Categories")
                .Nullable()

                .WithColumn("ArtistName")
                .AsString(255)
                .NotNullable()

                .WithColumn("PicturePath")
                .AsString(900)
                .NotNullable()

                .WithColumn("CoverPicturePath")
                .AsString(900)
                .Nullable()

                .WithColumn("Slug")
                .AsString(150)
                .NotNullable()

                .WithColumn("Description")
                .AsString(int.MaxValue)
                .Nullable()

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

            this.CreateTableIfNotExists("Songs", table =>
          table
                .WithColumn("SongId")
                .AsInt16()
                .PrimaryKey()
                .Identity()

                .WithColumn("SubCategoryId")
                .AsInt16()
                .Indexed("Songs_SubCategories")
                .Nullable()

                .WithColumn("SongName")
                .AsString(255)
                .NotNullable()

                .WithColumn("ArtistId")
                .AsInt16()
                .NotNullable()

                .WithColumn("ExternalId")
                .AsString(100)
                .NotNullable()

                .WithColumn("Language")
                .AsString(2)
                .NotNullable()

                .WithColumn("Language_cc")
                .AsString(2)
                .NotNullable()

                .WithColumn("FullLanguage")
                .AsString(2)
                .Nullable()

                .WithColumn("Duration")
                .AsInt32()
                .NotNullable()

                .WithColumn("Lyrics")
                .AsString()
                .NotNullable()

                .WithColumn("Slug")
                .AsString(150)
                .NotNullable()

                .WithColumn("YoutubeVideoId")
                .AsString(100)
                .NotNullable()

                .WithColumn("PicturePath")
                .AsString(900)
                .NotNullable()

                .WithColumn("CoverPicturePath")
                .AsString(900)
                .Nullable()

                .WithColumn("Provider")
                .AsString(150)
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


            this.CreateTableIfNotExists("Lyrics", table =>
          table
                .WithColumn("LyricsId")
                .AsInt16()
                .PrimaryKey()
                .Identity()

                .WithColumn("Text")
                .AsString(500)
                .NotNullable()

                .WithColumn("SongId")
                .AsInt16()
                .NotNullable()

                .WithColumn("Duration")
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
                        .FromTable("Songs")
                        .ForeignColumn("ArtistId")
                        .ToTable("Artists")
                        .PrimaryColumn("ArtistId");

            Create.ForeignKey()
                        .FromTable("Lyrics")
                        .ForeignColumn("SongId")
                        .ToTable("Songs")
                        .PrimaryColumn("SongId");


            Create.ForeignKey()
                        .FromTable("Artists")
                        .ForeignColumn("CategoryId")
                        .ToTable("Categories")
                        .PrimaryColumn("CategoryId");


            Create.ForeignKey()
                        .FromTable("Songs")
                        .ForeignColumn("SubCategoryId")
                        .ToTable("SubCategories")
                        .PrimaryColumn("SubCategoryId");
        }

        public override void Down()
        {
        }
    }
}
