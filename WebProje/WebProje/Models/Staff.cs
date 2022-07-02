using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProje.Models
{
    public class Staff
    {
        public int StaffID { get; set; }
        public String FullName { get; set; }
        public String Email { get; set; }

        
        public String Position { get; set; }
        public DateTime AddTime { get; set; }
    }
}
