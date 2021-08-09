
CREATE FUNCTION `FNC_IsFollow`(
	`followUpUserId` VARCHAR(255),
	`followedUserId` VARCHAR(255)
) RETURNS tinyint(4)
    READS SQL DATA
BEGIN
DECLARE IsFollowing TINYINT;
SET IsFollowing =  (SELECT (CASE
WHEN EXISTS(
SELECT NULL
FROM Follows fstatus WHERE fstatus.FollowUpUserId = followUpUserId AND fstatus.FollowedUserId = followedUserId)
THEN 1
ELSE 0
END));
RETURN IsFollowing;
END