using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    static class Jeson
    {
        public static JToken[] toJesonToken(string jsonString, string[] tags)
        {
            JObject jeson = JObject.Parse(jsonString);

            if (tags.Length > 0)
            {
                var res = jeson[tags[0]];
                for (int i = 1; i < tags.Length; i++)
                {
                    res = res[tags[i]];
                }

                return res.ToArray();
            }
            return null;
        }
    }
}
