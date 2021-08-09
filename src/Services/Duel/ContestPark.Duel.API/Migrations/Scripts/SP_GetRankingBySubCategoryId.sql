
CREATE PROCEDURE `SP_GetRankingBySubCategoryId`(
	IN `subCategoryId` INT(11),
	IN `balanceType` TINYINT,
	IN `contestDateId` TINYINT,
	IN `Offset` INT,
	IN `PageSize` INT
)
BEGIN
 SELECT CASE
                                  WHEN balanceType=1 THEN sr.DisplayTotalGoldScore
                                  WHEN balanceType=2 THEN sr.DisplayTotalMoneyScore
                                  END AS TotalScore,
                                  sr.UserId
                        FROM ScoreRankings sr
                        WHERE sr.ContestDateId = contestDateId
                           AND sr.SubCategoryId = subCategoryId
                        ORDER BY
                        CASE
                            WHEN balanceType=1 THEN sr.TotalGoldScore
                            WHEN balanceType=2 THEN sr.TotalMoneyScore
                        END DESC
LIMIT Offset, PageSize;
END