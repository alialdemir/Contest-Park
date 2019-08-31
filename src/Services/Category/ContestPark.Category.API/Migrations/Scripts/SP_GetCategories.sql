CREATE PROCEDURE SP_GetCategories(
    UserId VARCHAR(255),
    LangId TINYINT,
    PicturePath VARCHAR(900),
	Offset INT,
	PageSize INT
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
end) as PicturePath    
FROM Categories c
INNER JOIN SubCategoryRls sof ON sof.CategoryId=c.CategoryId
INNER JOIN SubCategories sc ON sc.SubCategoryId= sof.SubCategoryId
INNER JOIN CategoryLocalizeds cl ON cl.CategoryId = c.CategoryId
INNER JOIN SubCategoryLangs scl ON scl.SubCategoryId = sc.SubCategoryId
WHERE c.Visibility=true AND sc.Visibility=TRUE  AND cl.`Language`= LangId  AND scl.`Language`= LangId
ORDER BY sc.Price, c.DisplayOrder, sc.DisplayOrder
LIMIT Offset, PageSize;
END;