CREATE PROCEDURE SP_ChangeFollowersCount(
    SubcategoryId SMALLINT,
    FollowerCount TINYINT
)
BEGIN
UPDATE SubCategories SET FollowerCount = SubCategories.FollowerCount + FollowerCount WHERE SubCategories.SubCategoryId=SubcategoryId;
END;