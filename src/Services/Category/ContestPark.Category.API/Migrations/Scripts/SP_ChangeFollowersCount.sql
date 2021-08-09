
CREATE PROCEDURE `SP_ChangeFollowersCount`(
	IN `SubcategoryId` SMALLINT,
	IN `FollowerCount` TINYINT
)
BEGIN
UPDATE
SubCategories
SET
FollowerCount = SubCategories.FollowerCount + FollowerCount,
ModifiedDate = CURRENT_TIMESTAMP()
WHERE SubCategories.SubCategoryId=SubcategoryId;
END