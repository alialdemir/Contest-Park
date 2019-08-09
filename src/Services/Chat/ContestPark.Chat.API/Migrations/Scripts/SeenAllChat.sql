CREATE PROCEDURE SP_SeenAllChat(
    UserId VARCHAR(255)
)
BEGIN
UPDATE Messages SET ReceiverIsReadMessage = 1 WHERE
                           ConversationId IN (SELECT c.ConversationId FROM Conversations c
                           WHERE c.ReceiverUserId = UserId or c.SenderUserId = UserId);

                            UPDATE Conversations SET
                            SenderUnreadMessageCount = CASE
                                WHEN SenderUserId = UserId THEN 0
                                ELSE SenderUnreadMessageCount
                            END,
                            ReceiverUnreadMessageCount = CASE
                                WHEN ReceiverUserId = UserId THEN  0
                                ELSE ReceiverUnreadMessageCount
                            END
                            WHERE ReceiverUserId= UserId or SenderUserId = UserId;
END;