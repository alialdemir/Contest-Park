
CREATE PROCEDURE `Admin_SP_DeleteQuestions`(
	IN `SubCategoryId` SMALLINT
)
BEGIN
DELETE FROM AnswerLocalizeds WHERE QuestionId IN  (SELECT QuestionId FROM Questions q WHERE q.SubCategoryId = SubCategoryId);

DELETE FROM QuestionLocalizeds WHERE QuestionLocalizedId IN (SELECT QuestionLocalizedId FROM QuestionOfQuestionLocalizeds WHERE QuestionId IN  (SELECT QuestionId FROM Questions q WHERE q.SubCategoryId = SubCategoryId));

DELETE FROM QuestionOfQuestionLocalizeds WHERE QuestionId IN  (SELECT QuestionId FROM Questions q WHERE q.SubCategoryId=SubCategoryId);

DELETE FROM DuelDetails WHERE QuestionId IN  (SELECT QuestionId FROM Questions q WHERE q.SubCategoryId=SubCategoryId);

DELETE FROM Duels d WHERE d.SubCategoryId=SubCategoryId;

DELETE FROM Questions q WHERE q.SubCategoryId=SubCategoryId;

END