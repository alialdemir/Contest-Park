
CREATE FUNCTION `FNC_Mission1-2-3`(
	`UserId` VARCHAR(255),
	`WinDuelCount` INT
) RETURNS tinyint(4)
BEGIN
	
DECLARE Result TINYINT;
SET Result = (SELECT (CASE
WHEN EXISTS(
SELECT NULL FROM 
(
SELECT COUNT(d.DuelId) AS WinDuelCount FROM Duels d
WHERE d.CreatedDate >= NOW() + INTERVAL -1 DAY
AND d.CreatedDate <  NOW() + INTERVAL  0 DAY
AND ((d.FounderUserId = UserId AND  d.FounderTotalScore > d.OpponentTotalScore)
OR (d.OpponentUserId = UserId AND d.OpponentTotalScore > d.FounderTotalScore))
) AS wdc
WHERE wdc.WinDuelCount > WinDuelCount
)
THEN 1
ELSE 0
END));

RETURN Result;
END