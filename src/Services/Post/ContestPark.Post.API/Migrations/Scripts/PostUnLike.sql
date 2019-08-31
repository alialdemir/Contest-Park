CREATE PROCEDURE SP_PostUnLike(
    userId VARCHAR(255),
    postId INT
)
BEGIN

DELETE FROM Likes l WHERE l.PostId = postId AND l.UserId = userId;

UPDATE Posts p SET
LikeCount = (CASE WHEN LikeCount IS NOT NULL THEN LikeCount - 1 ELSE  null END)
WHERE p.PostId = postId;

END;