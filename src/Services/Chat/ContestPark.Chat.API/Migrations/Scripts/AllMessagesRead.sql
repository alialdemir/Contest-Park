CREATE PROCEDURE SP_AllMessagesRead(
    UserId VARCHAR(255),
    ChatId TINYINT
)
BEGIN

UPDATE Conversations SET
SenderUnreadMessageCount = CASE
    WHEN SenderUserId = UserId THEN 0
    ELSE SenderUnreadMessageCount
END,
ReceiverUnreadMessageCount = CASE
    WHEN ReceiverUserId = UserId THEN  0
    ELSE ReceiverUnreadMessageCount
END
WHERE Conversations.ConversationId = ChatId;

UPDATE Messages SET
ReceiverIsReadMessage=TRUE
WHERE Messages.ConversationId=ChatId;

END;