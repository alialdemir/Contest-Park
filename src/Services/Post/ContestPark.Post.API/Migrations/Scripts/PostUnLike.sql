CREATE PROCEDURE SP_PostUnLike(
    userId VARCHAR(256),
    postId INT
)
BEGIN

DELETE FROM Likes WHERE PostId=postId AND UserId=userId;

UPDATE Posts SET
LikeCount = (CASE WHEN LikeCount IS NOT NULL THEN LikeCount - 1 ELSE  null END)
WHERE PostId = postId;

END;