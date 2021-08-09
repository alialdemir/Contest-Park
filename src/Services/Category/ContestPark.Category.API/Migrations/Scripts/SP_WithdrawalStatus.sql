
CREATE PROCEDURE `SP_WithdrawalStatus`(
	IN `UserId` VARCHAR(255)
)
    READS SQL DATA
BEGIN
(SELECT (CASE
WHEN EXISTS(
SELECT NULL FROM (
SELECT COUNT(d.DuelId) AS TotalDuelCount FROM Duels d WHERE d.CreatedDate >= (SELECT ph.CreatedDate FROM PurchaseHistories ph
WHERE ph.UserId = UserId
ORDER BY ph.CreatedDate DESC
LIMIT 1) AND (d.FounderUserId = UserId OR d.OpponentUserId = UserId)
ORDER BY d.CreatedDate DESC
) AS d
WHERE d.TotalDuelCount >= 10
)
THEN 1
ELSE 0
END));
END