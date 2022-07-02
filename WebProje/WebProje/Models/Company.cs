using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProje.Models
{
    public class Company
    {
        public int CompanyID { get; set; }
        public String CompanyType { get; set; }
        public String CompanyName { get; set; }
        public List<Bill> Bills { get; set; }
    }
}
