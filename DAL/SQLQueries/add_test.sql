IF NOT EXISTS (SELECT * FROM [tests] WHERE [Id] = @ID)
INSERT INTO [tests] (Id, _date, result)
Values (@ID , '@date' , @result)
