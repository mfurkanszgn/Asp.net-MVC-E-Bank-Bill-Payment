using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebProje.Data;
using WebProje.Models;
using System.Data;
namespace WebProje.Models
{
    public class DbInitializer : IDbInitializer 
    {
        private readonly AppDbContext _db;
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(AppDbContext db,
            UserManager<Users> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;

            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {

            }

            if (_db.Roles.Any(r => r.Name == "Admin")) return;

            _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("User")).GetAwaiter().GetResult();

            _userManager.CreateAsync(new Users
            {
                UserName = "Admin@sakarya.edu.tr",
                Email = "admin@gmail.com",
                Name = "Furkan",
                LastName = "Sezgin",
                EmailConfirmed = true,
                Phone = "1112223333",

            }, "102938.Mfs").GetAwaiter().GetResult();


            _userManager.AddToRoleAsync(_db.Users.FirstOrDefaultAsync(u => u.Name == "Furkan").GetAwaiter().GetResult(), "Admin").GetAwaiter().GetResult();



       
        }

    }
}
