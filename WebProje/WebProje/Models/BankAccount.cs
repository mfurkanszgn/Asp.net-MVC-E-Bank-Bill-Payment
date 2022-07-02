using WebProje.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebProje.Models
{
    public class BankAccount
    {    [Key]
        public int BankAccountID { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountType { get; set; }
        public float BankAccountBalance { get; set; }
        public float BankAccountLimit { get; set; }

        public string UsersId { get; set; }
        public  Users Users { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }


    }
}
