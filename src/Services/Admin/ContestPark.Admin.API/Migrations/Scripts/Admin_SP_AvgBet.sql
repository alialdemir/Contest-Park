
CREATE PROCEDURE `Admin_SP_AvgBet`()
BEGIN
SELECT AVG(df.KullanicilarinOrtalamaBahisTutari) FROM (
SELECT SUM(d.Bet) AS KullanicilarinOrtalamaBahisTutari FROM AspNetUsers a
INNER JOIN Duels d ON a.Id = d.FounderUserId OR a.Id = d.OpponentUserId
where d.BalanceType = 2 AND d.Bet <= 10
GROUP BY a.Id
) AS df;
END