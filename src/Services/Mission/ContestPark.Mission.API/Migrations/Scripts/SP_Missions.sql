
CREATE PROCEDURE `SP_Missions`(
	IN `UserId` VARCHAR(255),
	IN `LangId` TINYINT
)
    DETERMINISTIC
BEGIN
SELECT
m.MissionId,
ml.Title,
ml.Description,
m.PicturePath,
m.Reward,
m.RewardBalanceType,
m.MissionTime,
`FNC_IsMissionCompleted`(UserId, m.MissionId) AS IsCompleteMission
FROM Missions m
INNER JOIN MissionLocalizeds ml ON m.MissionId =  ml.MissionId
WHERE ml.`Language` = LangId AND m.Visibility = 1;
END