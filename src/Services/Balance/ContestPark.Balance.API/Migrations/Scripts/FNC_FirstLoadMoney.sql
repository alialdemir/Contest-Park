
CREATE FUNCTION `FNC_FirstLoadMoney`(
	`userId` VARCHAR(255),
	`balanceType` TINYINT
) RETURNS tinyint(4)
BEGIN
DECLARE IsFollowing TINYINT;
SET IsFollowing = (SELECT (CASE-- Eğer para yüklemiş ama hiç çekmemişse ve parası 10 tl altındaysa yenmesini sağlıyoruz çünkü en az bi kere para çekebilsin
WHEN EXISTS(
SELECT * FROM
(
SELECT
COALESCE(SUM(ph.Amount), 0) AS PurchaseHistory,
COALESCE(SUM(mwr.Amount), 0) AS MoneyWithdrawRequest
FROM Balances b
LEFT JOIN PurchaseHistories ph ON ph.UserId = b.UserId
LEFT JOIN MoneyWithdrawRequests mwr ON mwr.UserId = b.UserId
WHERE b.UserId = userId
AND b.Money <= 21.00
) AS t
WHERE t.PurchaseHistory > 0 AND t.MoneyWithdrawRequest = 0 AND balanceType = 2 -- Eğer para ile oynuyorsa
)
THEN 1
ELSE 0
END));
RETURN IsFollowing;
END