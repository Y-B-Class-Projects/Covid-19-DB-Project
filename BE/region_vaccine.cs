using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class region_vaccine
    {
        public DateTime date { get; set; }
        public int cityCode { get; set; }
        Dictionary<string, string> first_dose = new Dictionary<string, string>();
        Dictionary<string, string> second_dose = new Dictionary<string, string>();

        
    }
}
