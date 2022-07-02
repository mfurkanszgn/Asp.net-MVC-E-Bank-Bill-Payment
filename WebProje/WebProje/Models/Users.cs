using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProje.Models
{
    public class Users:IdentityUser
    {
        private RoleManager<IdentityRole> roleManager;
        public string Name { get; set; }
        public string LastName { get; set; }

        public string    Phone { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
        public  virtual ICollection<BankAccount> BankAccounts { get; set; }
   
       // public  virtual ICollection<UserBill> UserBills { get; set; }
    }
}
