CREATE DEFINER=`CarDErmATEXe`@`%` PROCEDURE `SP_LastCategoriesPlayed`(
	IN `UserId` VARCHAR(255),
	IN `LangId` TINYINT

)
LANGUAGE SQL
NOT DETERMINISTIC
NO SQL
SQL SECURITY DEFINER
COMMENT ''
BEGIN
SELECT d.SubCategoryId,
sc.PicturePath,
sc.Price,
sc.DisplayPrice,
scl.SubCategoryName
 FROM Duels d
 INNER JOIN SubCategories sc on d.SubCategoryId = sc.SubCategoryId
 INNER JOIN SubCategoryLangs scl on scl.SubCategoryId = sc.SubCategoryId AND scl.`Language` = LangId
WHERE d.FounderUserId = UserId OR d.OpponentUserId = UserId
GROUP BY d.SubCategoryId
ORDER BY d.DuelId DESC
LIMIT 10;
END