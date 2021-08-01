using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DAL
{
    public class DAL_class
    {
        public enum Table { vaccines_city, tests };
        private DirectoryInfo workingDirectory;

        public DAL_class()
        {
            workingDirectory = directory.getWorkingDirectory();
        }

        public async Task update_all_tables()
        {
            await update_vaccines_tableAsync();
            await update_test_tableAsync();
        }

        private async Task update_vaccines_tableAsync()
        {
            DateTime lastDate = get_last_date();
            int lenght;

            do
            {
                lastDate = lastDate.AddDays(1);

                lenght = await download_table("https://data.gov.il/api/3/action/datastore_search?resource_id=12c9045c-1bf4-478a-a9e1-1e876cc2e182" +
                "&limit=1000&filters={\"Date\":\"" + lastDate.ToString("yyyy-MM-dd") + "\"}", Table.vaccines_city);

            } while (lastDate < DateTime.Now);
        }

        private async Task update_test_tableAsync()
        {
            int lenght;
            long last_id = get_last_ID(Table.tests);

            do
            {
                lenght = await download_table("https://data.gov.il/api/3/action/datastore_search?resource_id=dcf999c1-d394-4b57-a5e0-9d014a62e046" +
                "&limit=100000&offset=" + last_id, Table.tests);

                last_id += lenght;

            } while (lenght > 0);

        }

        private long get_last_ID(Table table)
        {
            var query = File.ReadAllText(workingDirectory + @"\DAL\SQLQueries\max_ID.sql");
            query = query.Replace("@table_name", table.ToString());

            var res = SQL.getInstance().readScalar(query);

            if (res == "")
                return 0;
            else
                return long.Parse(res);
        }


        private async Task<int> download_table(string tabel_url, Table table)
        {
            var jsonString = await HTTP.get(tabel_url);
            var records = Jeson.toJesonToken(jsonString, new string[] { "result", "records" });

            for (int i = 0; i < records.Length; i++)
                update_row(records[i], table);

            return records.Length;
        }

        private void update_row(JToken jToken, Table table)
        {
            if (table == Table.vaccines_city)
            {
                var CityName = readCityName((string)jToken["CityName"]);
                var date = DateTime.Parse((string)jToken["Date"]);
                var first_dose = new int[9];
                var second_dose = new int[9];

                string[] strings = new string[9] { "_dose_0-19", "_dose_20-29", "_dose_30-39", "_dose_40-49", "_dose_50-59", "_dose_60-69", "_dose_70-79", "_dose_80-89", "_dose_90+" };
                for (int i = 0; i < 9; i++)
                {
                    first_dose[i] = to_int(jToken["first" + strings[i]].Value<string>());
                    second_dose[i] = to_int(jToken["second" + strings[i]].Value<string>());
                }
                add_vaccine_city(CityName, date, first_dose, second_dose);
            }
            else if (table == Table.tests)
            {
                var date = DateTime.Parse((string)jToken["result_date"]);

                string result_string = jToken["corona_result"].Value<string>();
                int result;

                if (result_string == "חיובי")
                    result = 1;
                else if (result_string == "שלילי")
                    result = -1;
                else
                    result = 0;

                long id = to_int(jToken["_id"].Value<string>());

                add_test(id, date, result);
            }
        }


        private void add_test(long ID, DateTime date, int result)
        {
            string query = File.ReadAllText(workingDirectory + @"\DAL\SQLQueries\add_test.sql");

            query = query.Replace("@ID", ID.ToString());
            query = query.Replace("@date", date.ToString("yyyy-MM-dd"));
            query = query.Replace("@result", result.ToString());

            writeToTable(query);
        }

        private int to_int(string s)
        {
            string temp = string.Empty;
            int ret = 0;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '.')
                    break;
                if (Char.IsDigit(s[i]))
                    temp += s[i];
            }

            if (temp.Length > 0)
                ret = int.Parse(temp);

            return ret;
        }

        private string readCityName(string cityName)
        {
            return Regex.Replace(cityName, "[^a-zA-Zא-ת0-9_. ]+", "", RegexOptions.Compiled);
        }


        private void add_vaccine_city(string CityName, DateTime date, int[] first_dose, int[] second_dose)
        {
            string query = File.ReadAllText(workingDirectory + @"\DAL\SQLQueries\vaccines_city_add.sql");

            query = query.Replace("@CityName", CityName);
            query = query.Replace("@date", date.ToString("yyyy-MM-dd"));

            for (int i = 0; i < 9; i++)
            {
                query = query.Replace("@1_" + i, first_dose[i].ToString());
                query = query.Replace("@2_" + i, second_dose[i].ToString());
            }


            writeToTable(query);
        }

        private void writeToTable(string query)
        {
            try
            {
                SQL.getInstance().write(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error!");
            }
        }

        public DateTime get_last_date()
        {
            string query = "select max(_date) from vaccines_city";
            var ret = DateTime.Parse(SQL.getInstance().readScalar(query));
            return ret;
        }

        public Dictionary<DateTime, int> firstVaccineByDay(string cityName = "")
        {
            var ret = new Dictionary<DateTime, int>();

            string query;

            if (cityName == "")
                query = File.ReadAllText(workingDirectory + @"\DAL\SQLQueries\vaccines_by_day.sql");
            else
            {
                query = File.ReadAllText(workingDirectory + @"\DAL\SQLQueries\vaccines_by_day_by_city.sql");
                query = query.Replace("@city_name", cityName);
            }


            var first_by_day = SQL.getInstance().getRows(query, new string[] { "_date", "sum_f" });

            foreach (var day in first_by_day)
                ret[DateTime.Parse(day[0])] = int.Parse(day[1]);
            return ret;
        }

        public Dictionary<DateTime, int> secondVaccineByDay(string cityName = "")
        {
            var ret = new Dictionary<DateTime, int>();

            string query;

            if (cityName == "")
                query = File.ReadAllText(workingDirectory + @"\DAL\SQLQueries\vaccines_by_day.sql");
            else
            {
                query = File.ReadAllText(workingDirectory + @"\DAL\SQLQueries\vaccines_by_day_by_city.sql");
                query = query.Replace("@city_name", cityName);
            }
            var first_by_day = SQL.getInstance().getRows(query, new string[] { "_date", "sum_s" });

            foreach (var day in first_by_day)
                ret[DateTime.Parse(day[0])] = int.Parse(day[1]);
            return ret;
        }


        public List<string> get_city_names()
        {
            string query = File.ReadAllText(workingDirectory + @"\DAL\SQLQueries\get_city_names.sql");

            var list = SQL.getInstance().getRows(query, new string[] { "CityName" });

            return (from row in list
                    select row[0]).ToList();
        }


        public Dictionary<DateTime, int> get_tests(int result, bool isOnlyLastDay = false)
        {
            var ret = new Dictionary<DateTime, int>();

            string query;

            if (!isOnlyLastDay)
                query = File.ReadAllText(workingDirectory + @"\DAL\SQLQueries\tests_get.sql");
            else
            {
                query = File.ReadAllText(workingDirectory + @"\DAL\SQLQueries\tests_get_last_day.sql");
            }

            query = query.Replace("@result", result.ToString());


            var tests = SQL.getInstance().getRows(query, new string[] { "_date", "count_res" });

            foreach (var day in tests)
                ret[DateTime.Parse(day[0])] = int.Parse(day[1]);
            return ret;
        }
    }
}