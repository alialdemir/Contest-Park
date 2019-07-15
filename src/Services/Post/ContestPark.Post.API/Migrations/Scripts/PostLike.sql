CREATE PROCEDURE SP_PostLike(
    userId VARCHAR(256),
    postId INT
)
BEGIN

INSERT INTO Likes (UserId, PostId) VALUES (userId, postId);

UPDATE Posts SET
LikeCount = (CASE WHEN LikeCount IS NOT NULL THEN LikeCount + 1 ELSE  1 END)
WHERE PostId = postId;

END;