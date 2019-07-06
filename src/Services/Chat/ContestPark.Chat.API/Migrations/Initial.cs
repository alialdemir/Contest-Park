using ContestPark.Core.Dapper.Extensions;
using FluentMigrator;

namespace ContestPark.Chat.API.Migrations
{
    [Migration(2201906)]
    public class Initial : Migration
    {
        public override void Up()
        {
            this.CreateTableIfNotExists("Blocks", table =>
            table

                .WithColumn("BlockId")
                .AsInt64()
                .PrimaryKey()
                .Identity()

                .WithColumn("SkirterUserId")
                .AsString(255)
                .Unique()
                .NotNullable()

                .WithColumn("DeterredUserId")
                .AsString(255)
                .Unique()
                .NotNullable()

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()
                .WithDefault(SystemMethods.CurrentDateTime)

                .WithColumn("CreatedDate")
                .AsDateTime()
                .Nullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("Conversations", table =>
            table

                .WithColumn("ConversationId")
                .AsInt64()
                .PrimaryKey()
                .Identity()

                .WithColumn("SenderUserId")
                .AsString(255)
                .Unique()
                .NotNullable()

                .WithColumn("ReceiverUserId")
                .AsString(255)
                .Unique()
                .NotNullable()

                .WithColumn("SenderUnreadMessageCount")
                .AsInt16()
                .Unique()

                .WithColumn("ReceiverUnreadMessageCount")
                .AsInt16()
                .Unique()

                .WithColumn("LastMessage")
                .AsString(1000)
                .Unique()
                .NotNullable()

                .WithColumn("LastMessageDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("LastWriterUserId")
                .AsString(255)
                .Unique()
                .NotNullable()

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()
                .WithDefault(SystemMethods.CurrentDateTime)

                .WithColumn("CreatedDate")
                .AsDateTime()
                .Nullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("Messages", table =>
            table

                .WithColumn("MessageId")
                .AsInt64()
                .ForeignKey()

                .WithColumn("ConversationId")
                .AsString(255)
                .ForeignKey("Conversations", "ConversationId")
                .NotNullable()

                .WithColumn("Text")
                .AsString(1000)
                .Unique()
                .NotNullable()

                .WithColumn("AuthorUserId")
                .AsString(255)
                .Unique()
                .NotNullable()

                .WithColumn("ReceiverIsDeleted")
                .AsBoolean()

                .WithColumn("SenderIsDeleted")
                .AsBoolean()

                .WithColumn("ReceiverIsReadMessage")
                .AsBoolean()

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()
                .WithDefault(SystemMethods.CurrentDateTime)

                .WithColumn("CreatedDate")
                .AsDateTime()
                .Nullable()
                .WithDefault(SystemMethods.CurrentDateTime));
        }

        public override void Down()
        {
            Delete.Table("Blocks");
            Delete.Table("Conversations");
            Delete.Table("Messages");
        }
    }
}
