
CREATE PROCEDURE `SP_GetRankingAllTimes`(
	IN `PageSize` INT,
	IN `Offset` INT
)
BEGIN
SELECT b.UserId, FORMAT(b.TotalScore, 2) AS TotalScore FROM 
(
	SELECT  UserId,
	SUM(DisplayTotalMoneyScore) AS TotalScore
	FROM ScoreRankings
	WHERE TotalMoneyScore > 0
	GROUP BY UserId
) AS b
ORDER BY b.TotalScore DESC
LIMIT Offset, PageSize;
END
