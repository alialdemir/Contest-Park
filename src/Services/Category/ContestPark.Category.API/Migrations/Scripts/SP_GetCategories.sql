
CREATE PROCEDURE `SP_GetCategories`(
	IN `UserId` VARCHAR(255),
	IN `LangId` TINYINT,
	IN `PicturePath` VARCHAR(900),
	IN `IsAllOpen` TINYINT,
	IN `Offset` INT,
	IN `PageSize` INT
)
BEGIN
SELECT
c.CategoryId,
cl.TEXT AS CategoryName,
sc.SubCategoryId,
scl.SubCategoryName,
 
(case (SELECT
(CASE
WHEN EXISTS(
SELECT NULL
FROM OpenSubCategories AS osc  where IsAllOpen = true OR (osc.UserId =UserId and osc.SubCategoryId = sc.SubCategoryId))
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
FROM OpenSubCategories AS osc  where IsAllOpen = true OR (osc.UserId =UserId and osc.SubCategoryId = sc.SubCategoryId))
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
FROM OpenSubCategories AS osc  where IsAllOpen = true OR (osc.UserId =UserId and osc.SubCategoryId = sc.SubCategoryId)
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
FROM OpenSubCategories AS osc  where IsAllOpen = true OR (osc.UserId =UserId and osc.SubCategoryId = sc.SubCategoryId)
) THEN 1
ELSE 0
END) ) = 1 then 1
ELSE 0
end) as IsSubCategoryOpen 

FROM Categories c
INNER JOIN SubCategoryRls sof ON sof.CategoryId=c.CategoryId
INNER JOIN SubCategories sc ON sc.SubCategoryId= sof.SubCategoryId
INNER JOIN CategoryLocalizeds cl ON cl.CategoryId = c.CategoryId
INNER JOIN SubCategoryLangs scl ON scl.SubCategoryId = sc.SubCategoryId
WHERE c.Visibility=true AND sc.Visibility=true  AND cl.`Language`= LangId  AND scl.`Language`= LangId
ORDER BY sc.Price, c.DisplayOrder, sc.DisplayOrder
LIMIT Offset, PageSize;
END