select _date, A.f_0_19 + A.f_20_29 + A.f_30_39 + A.f_40_49 + A.f_50_59 + A.f_60_69 + A.f_70_79 + A.f_80_89 + A.f_90 as sum_f,
		   A.s_0_19 + A.s_20_29 + A.s_30_39 + A.s_40_49 + A.s_50_59 + A.s_60_69 + A.s_70_79 + A.s_80_89 + A.s_90 as sum_s
from (select _date, sum([first_dose_0-19]) as f_0_19,
				sum([first_dose_20-29]) as f_20_29,
				sum([first_dose_30-39]) as f_30_39,
				sum([first_dose_40-49]) as f_40_49,
				sum([first_dose_50-59]) as f_50_59,
				sum([first_dose_60-69]) as f_60_69,
				sum([first_dose_70-79]) as f_70_79,
				sum([first_dose_80-89]) as f_80_89,
				sum([first_dose_90+]) as f_90,
				sum([second_dose_0-19]) as s_0_19,
				sum([second_dose_20-29]) as s_20_29,
				sum([second_dose_30-39]) as s_30_39,
				sum([second_dose_40-49]) as s_40_49,
				sum([second_dose_50-59]) as s_50_59,
				sum([second_dose_60-69]) as s_60_69,
				sum([second_dose_70-79]) as s_70_79,
				sum([second_dose_80-89]) as s_80_89,
				sum([second_dose_90+]) as s_90
from vaccines_city
group by _date) as A
order by _date



