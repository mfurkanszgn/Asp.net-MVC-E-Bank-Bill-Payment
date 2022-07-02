using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebProje.Models
{
    public class Bank
    {
        [Key]
        public int BankID { get; set; }
        public String BankName { get; set; }
        public float BankMoney { get; set; }
        public float BankFeeRatio { get; set; }
    }
}
