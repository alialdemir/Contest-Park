
CREATE PROCEDURE `SP_SeenAllChat`(
	IN `UserId` VARCHAR(255)
)
BEGIN
UPDATE
Messages
SET
ReceiverIsReadMessage = 1,

ModifiedDate = CURRENT_TIMESTAMP()

WHERE
ConversationId IN (SELECT c.ConversationId FROM Conversations c WHERE c.ReceiverUserId = UserId or c.SenderUserId = UserId);


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

WHERE ReceiverUserId= UserId or SenderUserId = UserId;
END