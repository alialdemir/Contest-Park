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
FROM OpenSubCategories osc
INNER JOIN SubCategories sc ON sc.SubCategoryId=osc.SubCategoryId		
INNER JOIN SubCategoryLangs scl ON scl.SubCategoryId = sc.SubCategoryId AND scl.`Language` = LangId
WHERE osc.UserId = UserId
ORDER BY sc.DisplayOrder
LIMIT Offset, PageSize;
END;