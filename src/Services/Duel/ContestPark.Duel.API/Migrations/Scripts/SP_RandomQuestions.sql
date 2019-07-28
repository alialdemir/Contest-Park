CREATE PROCEDURE SP_RandomQuestions( 
IN subCategoryId INT(11),
IN founderUserId VARCHAR(255),
IN opponentUserId VARCHAR(255),
IN founderLanguge TINYINT,
IN opponentLanguge TINYINT
) 
BEGIN 
SELECT q.QuestionId, q.Link, q.AnswerType, q.QuestionType, ql.Question, ql.Language, al.CorrectStylish, al.Stylish1, al.Stylish2, al.Stylish3
FROM Questions q
INNER JOIN QuestionOfQuestionLocalizeds qoql ON qoql.QuestionId = q.QuestionId
INNER JOIN QuestionLocalizeds ql ON ql.QuestionLocalizedId = qoql.QuestionLocalizedId AND ql.Language IN (founderLanguge, opponentLanguge)
INNER JOIN AnswerLocalizeds al ON al.QuestionId = q.QuestionId AND al.Language = ql.Language
WHERE q.IsActive = true
AND q.SubCategoryId = subCategoryId
AND q.QuestionId NOT IN (SELECT aq.QuestionId
FROM AskedQuestions aq
WHERE aq.QuestionId = q.QuestionId AND aq.UserId IN (founderUserId, opponentUserId))
ORDER BY RAND()
LIMIT 14;
END;