
CREATE PROCEDURE `SP_RecommendedSubcategories`(
	IN `UserId` VARCHAR(255),
	IN `LangId` TINYINT


,
	IN `Offset` INT,
	IN `PageSize` INT


,
	IN `PicturePath` VARCHAR(900)




)
    NO SQL
BEGIN
SELECT 
sc.SubCategoryId,
scl.SubCategoryName,
(case (SELECT
(CASE
WHEN EXISTS(
SELECT NULL
FROM OpenSubCategories AS osc  where osc.UserId =UserId and osc.SubCategoryId = sc.SubCategoryId)
THEN 1
ELSE 0
END) )
when 1 then 0
else sc.DisplayPrice
end) as DisplayPrice,
 
(case (SELECT
(CASE
WHEN EXISTS(
SELECT NULL
FROM OpenSubCategories AS osc  where osc.UserId =UserId and osc.SubCategoryId = sc.SubCategoryId)
THEN 1
ELSE 0
END) )
when 1 then 0
else sc.Price
end) as Price,

(case
when sc.Price = 0 then sc.PicturePath
when (SELECT
(CASE
WHEN EXISTS(
SELECT NULL AS emp
FROM OpenSubCategories AS osc  where osc.UserId = UserId and osc.SubCategoryId = sc.SubCategoryId
) THEN 1
ELSE 0
END) ) = 1 then sc.PicturePath
ELSE PicturePath
end) as PicturePath,

(case
when sc.Price = 0 then 1
when (SELECT
(CASE
WHEN EXISTS(
SELECT NULL AS emp
FROM OpenSubCategories AS osc  where osc.UserId =UserId and osc.SubCategoryId = sc.SubCategoryId
) THEN 1
ELSE 0
END) ) = 1 then 1
ELSE 0
end) as IsSubCategoryOpen

 FROM SubCategoryRls scr1, SubCategories sc, SubCategoryLocalizeds scl,
(SELECT scr.CategoryId FROM 
Duels d
INNER JOIN SubCategoryRls scr ON scr.SubCategoryId = d.SubCategoryId
WHERE (d.FounderUserId = UserId OR d.OpponentUserId = UserId)
GROUP BY d.SubCategoryId) lastCategoriesPlayed

WHERE scr1.CategoryId = lastCategoriesPlayed.CategoryId
AND sc.SubCategoryId = scr1.SubCategoryId
AND scl.SubCategoryId = sc.SubCategoryId AND scl.`Language` = LangId
AND sc.Visibility = true
GROUP BY scr1.SubCategoryId
ORDER BY sc.Price, sc.DisplayOrder
LIMIT Offset, PageSize;
END