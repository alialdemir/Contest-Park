CREATE PROCEDURE `SP_Notifications`(
	IN `UserId` VARCHAR(255),
	IN `LangId` TINYINT,
	IN `Offset` INT,
	IN `PageSize` INT
)
BEGIN
SELECT
n.NotificationId,
n.WhoId AS UserId,
ntl.Description,
n.Link,
n.IsNotificationSeen,
n.CreatedDate AS DATE,
n.NotificationType,
n.PostId,
FNC_IsFollow(UserId, n.WhoId) AS IsFollowing
FROM Notifications n
INNER JOIN NotificationTypes nt ON n.NotificationType = nt.NotificationTypeId
INNER JOIN NotificationTypeLocalizeds ntl ON nt.NotificationTypeId = ntl.NotificationType
WHERE n.WhonId = UserId AND nt.IsActive = 1 AND ntl.`Language` = LangId
ORDER BY n.CreatedDate DESC
LIMIT Offset, PageSize;
END;