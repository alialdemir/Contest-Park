CREATE PROCEDURE SP_AddComment(
    userId VARCHAR(255),
    postId INT,
    text VARCHAR(500)
)
BEGIN

INSERT INTO Comments (UserId, PostId, Text) VALUES (userId, postId, text);

UPDATE Posts SET
CommentCount = (CASE WHEN CommentCount IS NOT NULL THEN CommentCount + 1 ELSE  1 END)
WHERE PostId = postId;

END;