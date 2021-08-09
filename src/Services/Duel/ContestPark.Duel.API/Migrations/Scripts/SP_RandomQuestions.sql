
CREATE PROCEDURE `SP_RandomQuestions`(
	IN `subCategoryId` INT(11),
	IN `founderUserId` VARCHAR(255),
	IN `opponentUserId` VARCHAR(255),
	IN `founderLanguge` TINYINT,
	IN `opponentLanguge` TINYINT

)
BEGIN 
SELECT 
ddd.QuestionId, ddd.Link, ddd.AnswerType, ddd.QuestionType, ql.Question, ql.Language, al.CorrectStylish, al.Stylish1, al.Stylish2, al.Stylish3

 FROM QuestionOfQuestionLocalizeds qoql, (
SELECT q.QuestionId, q.Link, q.AnswerType, q.QuestionType  FROM Questions q
WHERE q.IsActive = true
AND q.SubCategoryId = subCategoryId
AND q.QuestionId NOT IN (SELECT aq.QuestionId
FROM AskedQuestions aq
WHERE aq.QuestionId = q.QuestionId AND aq.UserId IN (founderUserId, opponentUserId))
ORDER BY RAND()
LIMIT 7
) AS ddd, QuestionLocalizeds ql, AnswerLocalizeds al
      WHERE qoql.QuestionId = ddd.QuestionId  AND
		  ql.QuestionLocalizedId = qoql.QuestionLocalizedId AND
		   ql.`Language` IN (founderLanguge, opponentLanguge) AND 
		   al.QuestionId = qoql.QuestionId AND
			 al.`Language` = ql.`Language`;
END