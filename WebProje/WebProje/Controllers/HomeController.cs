using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebProje.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebProje.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebProje.Data;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebProje.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly AppDbContext _Appcontext;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Users> _userManager;
        public HomeController(ILogger<HomeController> logger, DatabaseContext context, UserManager<Users> userManager, AppDbContext Appcontext)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _Appcontext = Appcontext;
        }
        [Authorize]
        public IActionResult Index()
        {
            //Current User Or Admin
            var allusers = _Appcontext.Users.ToList();
            ViewBag.allusers = allusers;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = _Appcontext.Users.FirstOrDefault(u => u.Id == userId);
            ViewBag.User = currentUser;
            ViewBag.allusers = allusers;


            //Viewbag user bills
           ViewBag.userbills= _Appcontext.Bills.Where(u => u.UsersId == userId).ToList();


            // Sum Money for Customer
            List<BankAccount> bankacccounts = _Appcontext.BankAccounts.Where(x => x.UsersId == userId).ToList();
            var sum_amounts = 0.0;
            foreach (var item in bankacccounts)
            {
                sum_amounts = sum_amounts + item.BankAccountBalance;
            }
            ViewBag.sumAmounts = sum_amounts;


            List<Bill> bills;
            // Total Bİll 
            if (this.User.IsInRole("Admin"))
            {

                bills = _Appcontext.Bills.Include(b => b.Users).ToList();
            }
            else
            {
                bills = _Appcontext.Bills.Where(x => x.UsersId == userId).ToList();

            }

            ViewBag.bills = bills;
            var unpayed_sum_bills = 0.0;
            var payed_sum_bills = 0.0;
            foreach (var item in bills)
            {
                if (item.BillStatus == false)
                {
                    unpayed_sum_bills += item.BillAmount;
                }
                else
                {
                    payed_sum_bills += item.BillAmount;

                }

            }
            ViewBag.unpayed_sum_bills = unpayed_sum_bills;
            ViewBag.payed_sum_bills = payed_sum_bills;

            //Add position
            var staffs = _Appcontext.Staffs.ToList();

            ViewBag.staffs = staffs;

            //Bank money
            var bankMoney = _Appcontext.Banks.FirstOrDefault().BankMoney;
            ViewBag.bankMoney = bankMoney;
            return View(bankacccounts);
        }
        /*  [HttpPost]
          public IActionResult Cookie(string culture)
          {
              Response.Cookies.Append
                  (
                  CookieRequestCultureProvider.DefaultCookieName,
                  CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                      new CookieOptions
                      {
                          Expires = DateTimeOffset.Now.AddDays(10)
                      }
                      );



              return RedirectToAction("Index");
          }*/



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Accounts()
        {

            return View();
        }
    }
}
