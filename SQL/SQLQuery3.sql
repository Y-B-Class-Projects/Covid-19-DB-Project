select *, 
[sum_first_dose_0-19] + [sum_first_dose_20-29] + [sum_first_dose_30-39] + [sum_first_dose_40-49] + [sum_first_dose_50-59] + [sum_first_dose_60-69] + [sum_first_dose_70-79] + [sum_first_dose_80-89] + [sum_first_dose_90+] as sum_first,
[sum_second_dose_0-19] + [sum_second_dose_20-29] + [sum_second_dose_30-39] + [sum_second_dose_40-49] + [sum_second_dose_50-59] + [sum_second_dose_60-69] + [sum_second_dose_70-79] + [sum_second_dose_80-89] + [sum_second_dose_90+] as sum_second
from
(SELECT [cityName],
		SUM([first_dose_0-19]) as [sum_first_dose_0-19],
		SUM([first_dose_20-29]) as [sum_first_dose_20-29],
		SUM([first_dose_30-39]) as [sum_first_dose_30-39],
		SUM([first_dose_40-49]) as [sum_first_dose_40-49],
		SUM([first_dose_50-59]) as [sum_first_dose_50-59],
		SUM([first_dose_60-69]) as [sum_first_dose_60-69],
		SUM([first_dose_70-79]) as [sum_first_dose_70-79],
		SUM([first_dose_80-89]) as [sum_first_dose_80-89],
		SUM([first_dose_90+]) as [sum_first_dose_90+],
		SUM([second_dose_0-19]) as [sum_second_dose_0-19],
		SUM([second_dose_20-29]) as [sum_second_dose_20-29],
		SUM([second_dose_30-39]) as [sum_second_dose_30-39],
		SUM([second_dose_40-49]) as [sum_second_dose_40-49],
		SUM([second_dose_50-59]) as [sum_second_dose_50-59],
		SUM([second_dose_60-69]) as [sum_second_dose_60-69],
		SUM([second_dose_70-79]) as [sum_second_dose_70-79],
		SUM([second_dose_80-89]) as [sum_second_dose_80-89],
		SUM([second_dose_90+]) as [sum_second_dose_90+]
FROM vaccines_city, city
group by [cityName]) sum_table


select *, 
[sum_first_dose_0-19] + [sum_first_dose_20-29] + [sum_first_dose_30-39] + [sum_first_dose_40-49] + [sum_first_dose_50-59] + [sum_first_dose_60-69] + [sum_first_dose_70-79] + [sum_first_dose_80-89] + [sum_first_dose_90+] as sum_first,
[sum_second_dose_0-19] + [sum_second_dose_20-29] + [sum_second_dose_30-39] + [sum_second_dose_40-49] + [sum_second_dose_50-59] + [sum_second_dose_60-69] + [sum_second_dose_70-79] + [sum_second_dose_80-89] + [sum_second_dose_90+] as sum_second
from
(SELECT [_date],
		SUM([first_dose_0-19]) as [sum_first_dose_0-19],
		SUM([first_dose_20-29]) as [sum_first_dose_20-29],
		SUM([first_dose_30-39]) as [sum_first_dose_30-39],
		SUM([first_dose_40-49]) as [sum_first_dose_40-49],
		SUM([first_dose_50-59]) as [sum_first_dose_50-59],
		SUM([first_dose_60-69]) as [sum_first_dose_60-69],
		SUM([first_dose_70-79]) as [sum_first_dose_70-79],
		SUM([first_dose_80-89]) as [sum_first_dose_80-89],
		SUM([first_dose_90+]) as [sum_first_dose_90+],
		SUM([second_dose_0-19]) as [sum_second_dose_0-19],
		SUM([second_dose_20-29]) as [sum_second_dose_20-29],
		SUM([second_dose_30-39]) as [sum_second_dose_30-39],
		SUM([second_dose_40-49]) as [sum_second_dose_40-49],
		SUM([second_dose_50-59]) as [sum_second_dose_50-59],
		SUM([second_dose_60-69]) as [sum_second_dose_60-69],
		SUM([second_dose_70-79]) as [sum_second_dose_70-79],
		SUM([second_dose_80-89]) as [sum_second_dose_80-89],
		SUM([second_dose_90+]) as [sum_second_dose_90+]
FROM vaccines_city, city
group by [_date]) sum_table



