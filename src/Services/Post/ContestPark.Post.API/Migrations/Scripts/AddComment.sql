
CREATE PROCEDURE `SP_AddComment`(
	IN `userId` VARCHAR(255),
	IN `postId` INT,
	IN `text` VARCHAR(500)
)
BEGIN

INSERT INTO Comments (UserId, PostId, Text) VALUES (userId, postId, text);

UPDATE Posts SET
Posts.CommentCount = ( CASE WHEN Posts.CommentCount IS NOT NULL THEN Posts.CommentCount + 1 ELSE 1 END),
Posts.ModifiedDate = CURRENT_TIMESTAMP()
WHERE PostId = postId;

END