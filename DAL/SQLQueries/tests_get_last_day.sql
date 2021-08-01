select count(*) count_res , _date
from tests
where [result] = 1 AND _date = (select max(_date) from tests)
group by _date
order by _date
