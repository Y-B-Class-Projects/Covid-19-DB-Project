using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class city
    {
        public string cityName { get; set; }
        public int cityCode { get; set; }

        public city(string cityName, int cityCode)
        {
            this.cityName = cityName;
            this.cityCode = cityCode;
        }

        public override string ToString()
        {
            return cityName;
        }



        
    }
}
