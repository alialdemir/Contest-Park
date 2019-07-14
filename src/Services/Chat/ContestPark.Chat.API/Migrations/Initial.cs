using ContestPark.Core.Dapper.Extensions;
using FluentMigrator;
using System.Reflection;

namespace ContestPark.Chat.API.Migrations
{
    [Migration(2201906)]
    public class Initial : Migration
    {
        public override void Up()
        {
            Execute.ExecuteScripts(Assembly.GetExecutingAssembly(), "AllMessagesRead.sql", "RemoveMessages.sql", "AddMessage.sql");

            this.CreateTableIfNotExists("Blocks", table =>
            table

                .WithColumn("BlockId")
                .AsInt64()
                .PrimaryKey()
                .Identity()

                .WithColumn("SkirterUserId")
                .AsString(255)
                .NotNullable()

                .WithColumn("DeterredUserId")
                .AsString(255)
                .NotNullable()

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("Conversations", table =>
            table

                .WithColumn("ConversationId")
                .AsInt64()
                .PrimaryKey()
                .Identity()

                .WithColumn("SenderUserId")
                .AsString(255)
                .NotNullable()

                .WithColumn("ReceiverUserId")
                .AsString(255)
                .NotNullable()

                .WithColumn("SenderUnreadMessageCount")
                .AsInt16()
                .WithDefaultValue(0)

                .WithColumn("ReceiverUnreadMessageCount")
                .AsInt16()
                .WithDefaultValue(0)

                .WithColumn("LastMessage")
                .AsString(1000)
                .NotNullable()

                .WithColumn("LastMessageDate")
                .AsDateTime()
                .Nullable()
                .WithDefault(SystemMethods.CurrentDateTime)

                .WithColumn("LastWriterUserId")
                .AsString(255)
                .NotNullable()

                .WithColumn("ModifiedDate")
                .AsDateTime()
                .Nullable()

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime));

            this.CreateTableIfNotExists("Messages", table =>
            table

                .WithColumn("MessageId")
                .AsInt64()
                .PrimaryKey()
                .Identity()

                .WithColumn("ConversationId")
                .AsInt64()
                .ForeignKey("Conversations", "ConversationId")
                .NotNullable()

                .WithColumn("Text")
                .AsString(1000)
                .NotNullable()

                .WithColumn("AuthorUserId")
                .AsString(255)
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

                .WithColumn("CreatedDate")
                .AsDateTime()
                .NotNullable()
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
