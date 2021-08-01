using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class BL_class
    {
        private DAL.DAL_class dal;

        public BL_class()
        {
            this.dal = new DAL_class();
        }

        public async Task download_data()
        {
            await dal.update_all_tables();
        }

        public Dictionary<int, Dictionary<DateTime, int>> getTotalVaccineByDay(int numOfDays = -1, string cityName = "")
        {
            Dictionary<int, Dictionary<DateTime, int>> ret = new Dictionary<int, Dictionary<DateTime, int>>();

            var first = dal.firstVaccineByDay(cityName);
            var second = dal.secondVaccineByDay(cityName);

            first = reduce_days(first, numOfDays);
            second = reduce_days(second, numOfDays);

            ret[1] = (from f in first
                      join s in second on f.Key equals s.Key
                      select new { key = f.Key, val = f.Value - s.Value })
                        .ToDictionary(t => t.key, t => t.val);

            ret[2] = (from s in second
                      select new { key = s.Key, val = s.Value })
                        .ToDictionary(t => t.key, t => t.val);

            return ret;
        }


        private Dictionary<DateTime, int> reduce_days(Dictionary<DateTime, int> data, int number_of_days)
        {
            if (number_of_days > 0)
                return (from d in data
                        where d.Key >= data.Keys.Last().AddDays(-number_of_days)
                        select new { key = d.Key, val = d.Value })
                        .ToDictionary(t => t.key, t => t.val);
            else
                return data;
        }


        public Dictionary<DateTime, int[]> getNewVaccinesByDay(string city = "")
        {
            var first_by_day = dal.firstVaccineByDay(city);
            var second_by_day = dal.secondVaccineByDay(city);

            var ret = (from f in first_by_day
                       join s in second_by_day on f.Key equals s.Key
                       select new
                       {
                           k = f.Key,
                           array = new int[] { f.Value - getPreviousDay(first_by_day, f.Key)
                                     , s.Value - getPreviousDay(second_by_day, f.Key) }
                       }).ToDictionary(t => t.k, t => t.array);

            return ret;
        }

        private int getPreviousDay(Dictionary<DateTime, int> days, DateTime day)
        {
            return (from d in days
                    where d.Key == day.AddDays(-1)
                    select d.Value).ToList().Sum();
        }


        public List<string> get_city_names()
        {
            return dal.get_city_names();
        }


        public Dictionary<DateTime, int> get_positive_tests(int number_of_days = -1, DateTime date = default(DateTime))
        {
            return reduce_days(dal.get_tests(1), number_of_days);
        }

        public Dictionary<DateTime, int> get_negative_tests(int number_of_days = -1, DateTime date = default(DateTime))
        {
            return reduce_days(dal.get_tests(-1), number_of_days);
        }

        public Dictionary<DateTime, int> get_number_of_tests_per_day(int number_of_days = -1, DateTime date = default(DateTime))
        {
            return (from positive in get_positive_tests(number_of_days)
                    join negative in get_negative_tests(number_of_days) on positive.Key equals negative.Key
                    select new
                    {
                        key = positive.Key,
                        val = positive.Value + negative.Value
                    }
                   ).ToDictionary(t => t.key, t => t.val);
        }

        public Dictionary<DateTime, int> get_last_7_days_avg(Dictionary<DateTime, int> data)
        {
            Dictionary<DateTime, int> ret = new Dictionary<DateTime, int>();

            foreach (var day in data)
            {
                DateTime thisDay = day.Key;
                    int sum = 0;

                    for (var i = thisDay.AddDays(-7); i < thisDay; i = i.AddDays(1))
                        if (data.ContainsKey(i))
                            sum += data[i];                        

                    ret[thisDay] = sum/7;  
            }

            return ret;
        }


        public Dictionary<DateTime, double> get_percentage_of_positive(int number_of_days = -1)
        {
            return (from positive in get_positive_tests(number_of_days)
                    join negative in get_negative_tests(number_of_days) on positive.Key equals negative.Key
                    select new
                    {
                        key = positive.Key,
                        val = ((double)positive.Value / (positive.Value + negative.Value))
                    }
                   ).ToDictionary(t => t.key, t => t.val);
        }


        public int get_last_day_tests()
        {
            return dal.get_tests(1, true).First().Value;
        }
    }
}
