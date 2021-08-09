
CREATE PROCEDURE `SP_DuelResult`(
	IN `duelId` INT,
	IN `userId` VARCHAR(255)

)
BEGIN
SELECT
d.FounderUserId,
d.OpponentUserId,
CASE
    WHEN d.FounderTotalScore = d.OpponentTotalScore THEN d.Bet
    ELSE d.Bet * 2
END AS Gold,
d.SubCategoryId,
d.BalanceType,
d.FounderTotalScore AS FounderScore,
d.OpponentTotalScore AS OpponentScore,
CASE WHEN d.FounderUserId = userId THEN d.FounderVictoryScore WHEN d.OpponentUserId = userId THEN d.OpponentVictoryScore END AS VictoryBonus,
CASE WHEN d.FounderUserId = userId THEN d.FounderFinishScore WHEN d.OpponentUserId = userId THEN d.OpponentFinishScore END AS FinishBonus
FROM Duels d
WHERE d.DuelId = duelId
LIMIT 1;
END