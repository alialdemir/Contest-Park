
CREATE PROCEDURE `Admin_SP_NewUsers`()
BEGIN
SELECT 
a.Id,
a.DisplayGameCount AS duel,
a.UserName,
a.FullName,
(SELECT  b.Gold FROM Balances b WHERE b.UserId = a.Id) AS Gold,
(SELECT  b.Money FROM Balances b WHERE b.UserId = a.Id) AS Money,
a.PhoneNumber,
a.CreatedDate,
a.ModifiedDate
FROM AspNetUsers a
WHERE a.IsBot = 0 AND a.FaceBookId IS NULL
ORDER BY a.CreatedDate DESC;


SELECT
CASE
    WHEN d.DuelType = 5 THEN "İPTAL"
    WHEN d.FounderTotalScore IS NULL AND d.OpponentTotalScore  IS NULL  THEN "Oynanıyor..."
    WHEN d.FounderTotalScore = d.OpponentTotalScore THEN "Beraberer"
    WHEN d.FounderTotalScore > d.OpponentTotalScore AND RIGHT(d.FounderUserId,4) = "-bot" THEN "Bot Kazandı"
    WHEN d.FounderTotalScore > d.OpponentTotalScore AND RIGHT(d.FounderUserId,4) <> "-bot" THEN "Oyuncu Kazandı"
    WHEN d.OpponentTotalScore > d.FounderTotalScore AND RIGHT(d.OpponentUserId,4) = "-bot" THEN "Bot Kazandı"
    WHEN d.OpponentTotalScore > d.FounderTotalScore AND RIGHT(d.OpponentUserId,4) <> "-bot" THEN "Oyuncu Kazandı"
END AS Kazanan,

CASE
    WHEN d.BalanceType = 1 THEN "Altın"
    WHEN d.BalanceType = 2 THEN "Para"
END AS BalanceType,
d.FounderTotalScore,
d.OpponentTotalScore,
d.Bet,
d.DuelId,
d.SubCategoryId,
d.FounderUserId,
d.OpponentUserId,
d.CreatedDate,
d.ModifiedDate
FROM Duels d
ORDER BY d.CreatedDate DESC
LIMIT 50;
END