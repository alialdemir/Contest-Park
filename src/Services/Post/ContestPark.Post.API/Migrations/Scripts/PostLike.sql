
CREATE PROCEDURE `SP_PostLike`(
	IN `userId` VARCHAR(255),
	IN `postId` INT
)
BEGIN

INSERT INTO Likes (UserId, PostId) VALUES (userId, postId);

UPDATE Posts SET
LikeCount = (SELECT COUNT(*) FROM Likes l WHERE l.PostId = postId),
ModifiedDate = CURRENT_TIMESTAMP()
WHERE PostId = postId;

END