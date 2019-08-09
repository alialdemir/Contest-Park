CREATE PROCEDURE SP_GetFollowedSubCategories(
    UserId VARCHAR(255),
    LangId TINYINT,
	Offset INT,
	PageSize INT
)
BEGIN
SELECT
sc.SubCategoryId,
scl.SubCategoryName,
sc.PicturePath
FROM FollowSubCategories fsc
INNER JOIN SubCategories sc ON sc.SubCategoryId=fsc.SubCategoryId		
INNER JOIN SubCategoryLangs scl ON scl.SubCategoryId = sc.SubCategoryId AND scl.`Language` = LangId
WHERE fsc.UserId = UserId
ORDER BY sc.DisplayOrder
LIMIT Offset, PageSize;
END;