
CREATE PROCEDURE `SP_GetPostsBySubcategoryId`(
	IN `PageSize` INT,
	IN `Offset` INT

,
	IN `userId` VARCHAR(255),
	IN `subCategoryId` TINYINT,
	IN `language` TINYINT




)
BEGIN
SELECT
                             p.CreatedDate AS Date,
                             p.LikeCount,
                             p.CommentCount,
                             p.OwnerUserId,
                             p.PostId,
                             p.PostType,
                             p.Bet,
                             scl.SubCategoryName,
                             sc.PicturePath AS SubCategoryPicturePath,
                             p.BalanceType,
                             p.IsCommentOpen,
                        

                             p.DuelId,
                             p.SubCategoryId,
                             p.CompetitorUserId,
                             p.CompetitorTrueAnswerCount,
                             p.FounderUserId,
                             p.FounderTrueAnswerCount,
                             FNC_PostIsLike(userId, p.PostId) AS IsLike,

                            p.PostImageType,
                            p.PicturePath,

                            p.Description

                           FROM Posts p
                           LEFT JOIN SubCategories sc ON sc.SubCategoryId = p.SubCategoryId
                           LEFT JOIN SubCategoryLangs scl ON scl.SubCategoryId = p.SubCategoryId AND scl.`Language`= language
                          WHERE p.SubCategoryId = subCategoryId AND p.PostType <> 5
                           ORDER BY p.CreatedDate desc
LIMIT Offset, PageSize;
END