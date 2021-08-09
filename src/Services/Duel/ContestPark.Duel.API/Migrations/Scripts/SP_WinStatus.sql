
CREATE PROCEDURE `SP_WinStatus`(
	IN `UserId` VARCHAR(255),
	IN `balanceType` TINYINT
)
    READS SQL DATA
BEGIN

SELECT * FROM (
-- Oyuncunun parası 41 tl den büyükse yenilsin
-- veya oyuncunun en son satın aldığı paketin (paket tutarı * 2) - (paket tutarı / 2) değerinden büyükse yenilsin
-- veya oyuncunun toplam para yükleme tutarı < oyuncunun toplam para çekme miktarından büyük ise yenilsin 
SELECT (CASE
WHEN EXISTS(
SELECT NULL
FROM Balances b 
WHERE
(b.UserId = userId AND b.Money >= (SELECT ROUND((ph.Amount  * 2)) FROM PurchaseHistories ph WHERE ph.UserId = userId ORDER BY ph.PurchaseHistoryId DESC LIMIT 1))
OR 
(
COALESCE((SELECT SUM(ph.Amount) FROM PurchaseHistories ph WHERE ph.UserId = userId), 0) 
<
COALESCE((SELECT SUM(ph.Amount) FROM MoneyWithdrawRequests ph WHERE ph.UserId = userId), 0)
)


)
THEN 1
ELSE 0
END) AS Check1

) AS Check1, 
-- Kaybettiği düello sayısı kazandığı düello sayısından büyük ise true döner 
-- Eğer para yüklemiş ama hiç çekmemişse ve parası 10 tl altındaysa yenmesini sağlıyoruz true döner
-- True dönerse oyuncu yenmeli
( 
SELECT 
CASE
    WHEN 
 `FNC_FirstLoadMoney`(userId, balanceType) = 1
	 OR
	  WinCount.c < (LoseCount.c + 5)
	  
	  THEN 1
    ELSE 0	
END AS  Check2
FROM
-- kaybettiği düello sayısı
(SELECT
COUNT(d.DuelId) AS c
FROM Duels d
WHERE (d.FounderUserId = UserId OR d.OpponentUserId = UserId) AND 
(d.FounderTotalScore > d.OpponentTotalScore AND RIGHT(d.FounderUserId,4) = "-bot" OR d.OpponentTotalScore > d.FounderTotalScore AND RIGHT(d.OpponentUserId,4) = "-bot")
ORDER BY d.CreatedDate DESC
) AS LoseCount, 

-- kazandığı düello sayısı
(SELECT
COUNT(d.DuelId) AS c
FROM Duels d
WHERE (d.FounderUserId = UserId OR d.OpponentUserId = UserId) AND 
(d.FounderTotalScore > d.OpponentTotalScore AND RIGHT(d.FounderUserId,4) <> "-bot" OR d.OpponentTotalScore > d.FounderTotalScore AND RIGHT(d.OpponentUserId,4) <> "-bot")
ORDER BY d.CreatedDate DESC 
) AS WinCount

) AS  Check2,


-- NOT: ŞUAN KULLANILMIYOR. AMA true dönerse ve Check2 false ise yenilmesi gerekir
(
SELECT 0 AS Check3 FROM VersionInfo LIMIT 1
) AS Check3,

-- Kazandığı düello sayısı  + 7 kaybettiğiığı düello sayısından büyük ise true döner 
-- True dönerse oyuncu yenilmeli
( SELECT 
CASE
    WHEN 
	 `FNC_FirstLoadMoney`(userId, balanceType) = 0
	 AND
	 WinCount.c > (LoseCount.c + 7) THEN 1
    ELSE 0	
END AS  Check4
FROM
-- kaybettiği düello sayısı
(SELECT
COUNT(d.DuelId) AS c
FROM Duels d
WHERE (d.FounderUserId = UserId OR d.OpponentUserId = UserId) AND 
(d.FounderTotalScore > d.OpponentTotalScore AND RIGHT(d.FounderUserId,4) = "-bot" OR d.OpponentTotalScore > d.FounderTotalScore AND RIGHT(d.OpponentUserId,4) = "-bot")
ORDER BY d.CreatedDate DESC
) AS LoseCount, 

-- kazandığı düello sayısı
(SELECT
COUNT(d.DuelId) AS c
FROM Duels d
WHERE (d.FounderUserId = UserId OR d.OpponentUserId = UserId) AND 
(d.FounderTotalScore > d.OpponentTotalScore AND RIGHT(d.FounderUserId,4) <> "-bot" OR d.OpponentTotalScore > d.FounderTotalScore AND RIGHT(d.OpponentUserId,4) <> "-bot")
ORDER BY d.CreatedDate DESC 
) AS WinCount

) AS  Check4

;
END
