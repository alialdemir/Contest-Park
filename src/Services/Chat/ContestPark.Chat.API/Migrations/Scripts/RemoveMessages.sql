CREATE PROCEDURE SP_RemoveMessages(
    UserId VARCHAR(256),
    ConversationId BIGINT
)
BEGIN
DECLARE IsSender TINYINT;

SET IsSender = (SELECT CASE WHEN c.SenderUserId = UserId THEN TRUE WHEN c.ReceiverUserId = UserId THEN FALSE END
FROM Conversations c WHERE c.ConversationId  = ConversationId);

UPDATE Messages SET
ReceiverIsDeleted = CASE WHEN IsSender = FALSE THEN TRUE ELSE ReceiverIsDeleted END,
SenderIsDeleted = CASE WHEN IsSender = TRUE THEN TRUE ELSE SenderIsDeleted END
WHERE Messages.ConversationId = ConversationId;
 
END;