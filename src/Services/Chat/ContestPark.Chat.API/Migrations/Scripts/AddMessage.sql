CREATE FUNCTION FNC_AddMessage(
    senderUserId VARCHAR(256),
    receiverUserId VARCHAR(256),
    lastWriterUserId VARCHAR(256),
    message VARCHAR(1000)    
)
RETURNS BIGINT
READS SQL DATA
BEGIN
DECLARE ConversationId BIGINT;

SET ConversationId = (SELECT c.ConversationId FROM Conversations c 
WHERE (c.ReceiverUserId = receiverUserId AND c.SenderUserId = senderUserId) 
OR (c.ReceiverUserId = senderUserId AND c.SenderUserId = receiverUserId));

CASE WHEN ConversationId IS NULL THEN 
INSERT INTO Conversations (SenderUserId, ReceiverUserId, LastMessage, LastWriterUserId) VALUES (senderUserId, receiverUserId, Message, lastWriterUserId);
SET ConversationId = LAST_INSERT_ID();
ELSE
BEGIN
END;
END CASE;

INSERT INTO Messages (ConversationId, TEXT, AuthorUserId, ReceiverIsDeleted, SenderIsDeleted, ReceiverIsReadMessage, CreatedDate)
VALUES (ConversationId, message , lastWriterUserId, FALSE, FALSE, FALSE, NOW());

UPDATE Conversations SET
LastMessage = message ,
LastMessageDate = NOW(),
LastWriterUserId = lastWriterUserId,
SenderUnreadMessageCount = CASE WHEN SenderUnreadMessageCount + 1 THEN  SenderUnreadMessageCount END,
ReceiverUnreadMessageCount = CASE WHEN ReceiverUnreadMessageCount + 1 THEN  ReceiverUnreadMessageCount END
WHERE Conversations.ConversationId = ConversationId;

RETURN ConversationId;
END;