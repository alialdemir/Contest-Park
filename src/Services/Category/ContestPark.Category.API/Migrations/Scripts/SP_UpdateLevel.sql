CREATE PROCEDURE `SP_UpdateLevel`(
	IN `UserId` VARCHAR(255),
	IN `SubCategoryId` SMALLINT(6),
	IN `Exp` INT(11)
)
BEGIN
DECLARE UserExp INT(11);
DECLARE UserLevel INT(11);
DECLARE NextLevelExp INT(11);
DECLARE NextLevel INT(11);
SELECT
UserExp = ul.Exp AS CurrentExp,
UserLevel = ul.`Level` AS CurrentLevel,
NextLevelExp = lu.Exp AS NextLevelExp,
NextLevel = lu.`Level` + 1 AS NextLevel
FROM UserLevels ul
INNER JOIN LevelUps lu ON lu.`Level` = ul.`Level`
WHERE ul.UserId = UserId AND ul.SubCategoryId = SubCategoryId;
	 
UPDATE UserLevels
SET 
UserLevels.Exp = CASE WHEN ((UserExp + Exp) - NextLevelExp) < 0 THEN (UserExp + Exp) ELSE ((UserExp + Exp) - NextLevelExp) END,
UserLevels.Level = CASE WHEN (UserExp + Exp) >= NextLevelExp THEN LEVEL + 1 ELSE Level END,
UserLevels.ModifiedDate = CURRENT_TIMESTAMP()
WHERE UserLevels.UserId = UserId AND UserLevels.SubCategoryId = SubCategoryId;
END;