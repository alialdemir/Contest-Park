
CREATE PROCEDURE `SP_TakesMissionReward`(
	IN `userId` VARCHAR(255),
	IN `missionId` TINYINT
)
BEGIN
if exists (select * from CompletedMissions where UserId = userId AND MissionId = missionId AND IsMissionCompleted = 0 ) then

UPDATE CompletedMissions
SET IsMissionCompleted = 1,
ModifiedDate = CURRENT_TIMESTAMP()
WHERE UserId = userId AND MissionId = missionId;

CALL `SP_UpdateBalance`(
(SELECT m.Reward FROM Missions m WHERE m.MissionId = missionId),
(SELECT m.RewardBalanceType FROM Missions m WHERE m.MissionId = missionId),
7, -- 7 değeri BalanceHistoryTypes.Mission demek oluyor
userId);

end if;
END