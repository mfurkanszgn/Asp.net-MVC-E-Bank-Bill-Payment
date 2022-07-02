using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebProje.Models
{
    public class Transaction
    {

        public int TransactionID { get; set; }
        public string  TransactionType { get; set; }
        public float TransactionAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string UsersId { get; set; }
        public Users Users { get; set; }
        
        public int BankAccountID { get; set; }
        public virtual BankAccount  BankAccount { get; set; }
       
        [NotMapped]
        public virtual int TransferBankAccountID { get; set; }
    }
}
