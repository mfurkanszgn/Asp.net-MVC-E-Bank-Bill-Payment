using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebProje.Models
{
    public class Bill
    {

        public int BillID { get; set; }
        [Required]
        public string BillDesc { get; set; }
        [Required]
        public float BillAmount { get; set; }
        [Required]
        public bool BillStatus { get; set; }
        [Required]
        public DateTime BillDate { get; set; }

        public int CompanyID { get; set; }
        public virtual Company Company { get; set; }
    
        public string UsersId { get; set; }
        public virtual Users Users { get; set; }
        [NotMapped]
        public virtual int BankAccountID { get; set; }

    
    }
}
