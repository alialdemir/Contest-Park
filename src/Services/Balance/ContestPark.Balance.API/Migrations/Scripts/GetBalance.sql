CREATE PROCEDURE SP_GetBalance(
    UserId VARCHAR(256)
)
BEGIN
SELECT c.Gold, c.Money
FROM Balances c
WHERE c.UserId=userId;
END;