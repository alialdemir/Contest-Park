
CREATE PROCEDURE `Admin_SP_NewPost`()
BEGIN
 
 

INSERT INTO Posts (
Bet,
CompetitorTrueAnswerCount,
CompetitorUserId,
Description ,
DuelId,
FounderTrueAnswerCount,
FounderUserId ,
OwnerUserId ,
PicturePath,
PostImageType,
SubCategoryId,
PostType,
BalanceType
)
SELECT p1.Bet,
p1.CompetitorTrueAnswerCount,
           (SELECT a.Id FROM AspNetUsers a  WHERE  RIGHT(a.Id,4) = "-bot" ORDER BY RAND() LIMIT 1),
                p1.Description ,
                p1.DuelId,
                p1.FounderTrueAnswerCount,
           (SELECT a.Id FROM AspNetUsers a  WHERE  RIGHT(a.Id,4) = "-bot" ORDER BY RAND() LIMIT 1),
                   "-1",
                p1.PicturePath,
                p1.PostImageType,
                p1.SubCategoryId,
                p1.PostType,
                p1.BalanceType
FROM Posts p1 
WHERE p1.PostType = 2
ORDER BY RAND()
LIMIT 5;

UPDATE Posts SET OwnerUserId = FounderUserId
WHERE OwnerUserId = "-1";
END