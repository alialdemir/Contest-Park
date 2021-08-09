
CREATE PROCEDURE `SP_PostUnLike`(
	IN `userId` VARCHAR(255),
	IN `postId` INT
)
BEGIN

DELETE FROM Likes l WHERE l.PostId = postId AND l.UserId = userId;

UPDATE Posts SET
LikeCount = (SELECT COUNT(*) FROM Likes l WHERE l.PostId = postId),
ModifiedDate = CURRENT_TIMESTAMP()
WHERE PostId = postId;

END