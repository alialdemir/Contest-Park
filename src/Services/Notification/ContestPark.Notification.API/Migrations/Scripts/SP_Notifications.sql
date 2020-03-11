CREATE PROCEDURE SP_Notifications(
    UserId VARCHAR(255),
    LangId TINYINT,
	Offset INT,
	PageSize INT
)
BEGIN
SELECT
n.NotificationId,
n.WhoId AS WhoUserId,
ntl.Description,
n.Link,
n.IsNotificationSeen,
n.CreatedDate AS DATE,
n.NotificationType
FROM Notifications n
INNER JOIN NotificationTypes nt ON n.NotificationType = nt.NotificationTypeId
INNER JOIN NotificationTypeLocalizeds ntl ON nt.NotificationTypeId = ntl.NotificationType
WHERE n.WhonId = UserId AND nt.IsActive = 1 AND ntl.`Language` = LangId
ORDER BY n.CreatedDate DESC
LIMIT Offset, PageSize;
END