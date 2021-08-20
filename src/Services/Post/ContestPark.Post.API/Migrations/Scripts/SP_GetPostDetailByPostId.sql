
CREATE PROCEDURE `SP_GetPostDetailByPostId`(
	IN `PageSize` INT,
	IN `Offset` INT,
	IN `userId` NVARCHAR(255),
	IN `postId` INT,
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
                             p.IsCommentOpen,
                     
                             p.Bet,
                             scl.SubCategoryName,
                             sc.PicturePath AS SubCategoryPicturePath,
                             p.BalanceType,

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
                           LEFT JOIN SubCategoryLocalizeds scl ON scl.SubCategoryId = p.SubCategoryId AND scl.`Language`= language
                        WHERE p.PostId = postId AND p.PostType <> 5
                           ORDER BY p.CreatedDate desc
LIMIT Offset, PageSize;
END