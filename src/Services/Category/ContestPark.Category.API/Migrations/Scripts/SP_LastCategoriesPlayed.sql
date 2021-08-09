
CREATE PROCEDURE `SP_LastCategoriesPlayed`(
	IN `UserId` VARCHAR(255),
	IN `LangId` TINYINT,
	IN `Offset` INT,
	IN `PageSize` INT
)
    NO SQL
BEGIN
SELECT * FROM 
(
SELECT
d.SubCategoryId,
sc.PicturePath,
scl.SubCategoryName,
1 as IsSubCategoryOpen
FROM Duels d
INNER JOIN SubCategories sc ON d.SubCategoryId = sc.SubCategoryId AND sc.Visibility = true
INNER JOIN SubCategoryLangs scl on scl.SubCategoryId = sc.SubCategoryId AND scl.`Language` = LangId
WHERE d.FounderUserId = UserId OR d.OpponentUserId = UserId
GROUP BY d.SubCategoryId, d.DuelId
ORDER BY d.DuelId DESC
) t
GROUP BY t.SubCategoryId
LIMIT Offset, PageSize;
END