
CREATE PROCEDURE `SP_AllMessagesRead`(
	IN `UserId` VARCHAR(255),
	IN `ChatId` TINYINT
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
END,
ModifiedDate = CURRENT_TIMESTAMP()
WHERE Conversations.ConversationId = ChatId;

UPDATE Messages SET
ReceiverIsReadMessage=TRUE,
ModifiedDate = CURRENT_TIMESTAMP()
WHERE Messages.ConversationId=ChatId;

END