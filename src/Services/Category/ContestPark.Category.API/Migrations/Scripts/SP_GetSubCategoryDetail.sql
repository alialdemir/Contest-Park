
CREATE PROCEDURE `SP_GetSubCategoryDetail`(
	IN `SubCategoryId` TINYINT,
	IN `LangId` TINYINT,
	IN `UserId` VARCHAR(255)
)
BEGIN

SELECT
sc.SubCategoryId,
scl.SubCategoryName,
scl.Description,
sc.FollowerCount,
sc.PicturePath,

(SELECT ul.Level FROM UserLevels ul WHERE ul.UserId = UserId AND ul.SubCategoryId = sc.SubCategoryId) AS Level,

(CASE
WHEN EXISTS(
SELECT NULL
FROM OpenSubCategories AS osc  WHERE (sc.Price = 0) OR (osc.UserId = UserId and osc.SubCategoryId = sc.SubCategoryId))
THEN 1
ELSE 0
END) as IsSubCategoryOpen,

(CASE
WHEN EXISTS(
SELECT NULL
FROM FollowSubCategories AS fcl  where fcl.UserId = UserId and fcl.SubCategoryId = sc.SubCategoryId)
THEN 1
ELSE 0
END) as IsSubCategoryFollowUpStatus
FROM SubCategories sc		
INNER JOIN SubCategoryLangs scl ON scl.SubCategoryId = sc.SubCategoryId AND scl.`Language` = LangId
WHERE sc.SubCategoryId= SubCategoryId;
END