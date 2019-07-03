CREATE PROCEDURE SP_GetSubCategoryDetail(
    SubCategoryId TINYINT,
    LangId TINYINT
)
BEGIN
SELECT
sc.SubCategoryId,
scl.SubCategoryName,
scl.Description,
sc.FollowerCount,
sc.PicturePath,
(CASE
WHEN EXISTS(
SELECT NULL
FROM FollowSubCategories AS fcl  where fcl.SubCategoryId = sc.SubCategoryId)
THEN 1
ELSE 0
END) as IsSubCategoryFollowUpStatus
FROM SubCategories sc		
INNER JOIN SubCategoryLangs scl ON scl.SubCategoryId = sc.SubCategoryId AND scl.`Language` = LangId
WHERE sc.SubCategoryId=SubCategoryId;
END;