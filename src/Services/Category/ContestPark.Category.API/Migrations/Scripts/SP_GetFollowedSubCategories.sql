
CREATE PROCEDURE `SP_GetFollowedSubCategories`(
	IN `UserId` VARCHAR(255),
	IN `LangId` TINYINT,
	IN `Offset` INT,
	IN `PageSize` INT
)
BEGIN
SELECT
sc.SubCategoryId,
scl.SubCategoryName,
sc.PicturePath,
1 AS IsSubCategoryOpen
FROM FollowSubCategories fsc
INNER JOIN SubCategories sc ON sc.SubCategoryId=fsc.SubCategoryId		
INNER JOIN SubCategoryLocalizeds scl ON scl.SubCategoryId = sc.SubCategoryId AND scl.`Language` = LangId
WHERE fsc.UserId = UserId
ORDER BY fsc.FollowSubCategoryId, sc.DisplayOrder DESC
LIMIT Offset, PageSize;
END