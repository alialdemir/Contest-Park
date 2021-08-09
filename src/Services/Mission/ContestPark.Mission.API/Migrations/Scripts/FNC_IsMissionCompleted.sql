
CREATE FUNCTION `FNC_IsMissionCompleted`(
	`UserId` VARCHAR(255),
	`MissionId` TINYINT
) RETURNS tinyint(4)
BEGIN
DECLARE IsMissionCompleted TINYINT;
SET IsMissionCompleted =  (SELECT (CASE
WHEN EXISTS(
SELECT NULL
FROM CompletedMissions cm WHERE cm.UserId = UserId AND cm.MissionId = MissionId)
THEN 1
ELSE 0
END));
RETURN IsMissionCompleted;
END