IF NOT EXISTS (SELECT * FROM [vaccines_city]
	WHERE [CityName] = N'@CityName'
	AND [_date] = '@date')
INSERT INTO [vaccines_city] ([CityName], [_date], [first_dose_0-19], [first_dose_20-29], [first_dose_30-39], [first_dose_40-49], [first_dose_50-59], [first_dose_60-69], [first_dose_70-79], [first_dose_80-89], [first_dose_90+], [second_dose_0-19], [second_dose_20-29], [second_dose_30-39], [second_dose_40-49], [second_dose_50-59], [second_dose_60-69], [second_dose_70-79], [second_dose_80-89], [second_dose_90+])
Values (N'@CityName' , '@date' , @1_0 , @1_1 , @1_2 , @1_3 , @1_4 , @1_5 , @1_6 , @1_7 , @1_8 , @2_0 , @2_1 , @2_2 , @2_3 , @2_4 , @2_5 , @2_6 , @2_7 , @2_8)
