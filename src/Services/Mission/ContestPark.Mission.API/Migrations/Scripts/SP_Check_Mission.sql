
CREATE FUNCTION `SP_Check_Mission`(
	`MissionId` TINYINT,
	`UserId` VARCHAR(255),
	`ResultStatus` INT
) RETURNS int(11)
BEGIN
 if MissionId = 1 then -- 10 Kez Rakibini Yen
        set ResultStatus =  `FNC_Mission1-2-3`(UserId, 10);
    ELSEIF MissionId = 2 then -- 25 Kez Rakibini Yen
        set ResultStatus = `FNC_Mission1-2-3`(UserId, 25);
    ELSEIF MissionId = 3 then -- 50 Kez Rakibini Yen
        set ResultStatus = `FNC_Mission1-2-3`(UserId, 50);
    else 
        set ResultStatus = -2;
    end if;
    
    RETURN ResultStatus;
END