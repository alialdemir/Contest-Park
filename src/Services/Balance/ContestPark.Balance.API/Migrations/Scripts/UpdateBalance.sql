CREATE PROCEDURE SP_UpdateBalance(
    Amount DECIMAL(13,2),
    BalanceType TINYINT,
    BalanceHistoryType TINYINT,
    UserId VARCHAR(256)
)
BEGIN
DECLARE OldAmount DECIMAL;

SET OldAmount = (SELECT CASE WHEN BalanceType = 1 THEN Gold WHEN BalanceType = 2 THEN Money END FROM Balances WHERE Balances.UserId = UserId);
CASE WHEN OldAmount IS NULL THEN INSERT INTO Balances (UserId, Gold, Money) VALUES (UserId, 0, 0); SET OldAmount=0;
ELSE
BEGIN
END;
END CASE;

UPDATE Balances
SET Gold = CASE WHEN BalanceType = 1 THEN Gold + Amount ELSE Gold END,
Money = CASE WHEN BalanceType = 2 THEN Money + Amount ELSE Money END
WHERE Balances.UserId=UserId;

INSERT INTO BalanceHistories (UserId, OldAmount, NewAmount, BalanceHistoryType, BalanceType)
VALUES (UserId, OldAmount, Amount +  OldAmount, BalanceHistoryType, BalanceType);
END;