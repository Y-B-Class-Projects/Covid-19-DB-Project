using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    static class HTTP
    {
        public static async Task<string> get(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            if (response != null)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return jsonString;
            }

            return null;
        }

    }
}