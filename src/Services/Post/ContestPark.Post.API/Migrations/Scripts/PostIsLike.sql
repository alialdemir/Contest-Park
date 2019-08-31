CREATE FUNCTION FNC_PostIsLike(
    userId VARCHAR(255),
    postId INT
)
RETURNS TINYINT
READS SQL DATA
BEGIN
DECLARE IsLike TINYINT;
SET IsLike =  (SELECT (CASE
                             WHEN EXISTS(
                             SELECT NULL
                             FROM Likes l WHERE l.UserId = userId AND l.PostID = postId)
                             THEN 1
                             ELSE 0
                             END));
RETURN IsLike;
END;