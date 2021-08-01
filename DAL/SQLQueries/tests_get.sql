select count(*) count_res , _date
from tests
where [result] = @result
group by _date
order by _date