
CREATE PROCEDURE `SP_IsCodeActive`(
	IN `code` VARCHAR(256)
,
	IN `userId` VARCHAR(256)
)
    READS SQL DATA
BEGIN
(SELECT r.Amount, r.BalanceType FROM `References` r
                           WHERE r.CODE = code
									AND 
									  (SELECT (CASE
                             WHEN EXISTS(
                             SELECT NULL
                             FROM ReferenceCodes l WHERE l.NewUserId = userId AND l.CODE = code)
                             THEN 0
                             ELSE 1
                             END))
									AND CURRENT_DATE() <= r.FinishDate AND (SELECT COUNT(rc.ReferenceCodeId) FROM  `ReferenceCodes` rc
                           WHERE rc.CODE = code) <= r.Menstruation);
END