
CREATE PROCEDURE `SP_IsExistsToken`(
	IN `Token` LONGTEXT
)
BEGIN
 (SELECT (CASE
                             WHEN EXISTS(
                             SELECT NULL
                             FROM PurchaseHistories l WHERE l.Token = Token)
                             THEN 1
                             ELSE 0
                             END));
END