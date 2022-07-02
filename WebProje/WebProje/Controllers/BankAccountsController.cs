using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebProje.Data;
using WebProje.Models;

namespace WebProje.Controllers
{
    public class BankAccountsController : Controller
    {
        private readonly AppDbContext _context;

        public BankAccountsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: BankAccounts
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
          
            if (this.User.IsInRole("Admin"))
            {
                var appDbContext = _context.BankAccounts.Include(b => b.Users);
                return View(await appDbContext.ToListAsync());
            }
            else
            {
               var  appDbContext = _context.BankAccounts.Include(b => b.Users).Where(x => x.UsersId == userId);
                return View(await appDbContext.ToListAsync());

            }
           // return View(await appDbContext.ToListAsync());
        }

        // GET: BankAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankAccount = await _context.BankAccounts
                .Include(b => b.Users)
                .FirstOrDefaultAsync(m => m.BankAccountID == id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            return View(bankAccount);
        }

        // GET: BankAccounts/Create
        public IActionResult Create()
        {
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "UserName");
            List<String> tyeps = new List<String>();
            tyeps.Add("Euro");
            tyeps.Add("Tl");
            tyeps.Add("Dolar");
            ViewBag.tyeps = tyeps;
       
            return View();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BankAccountID,BankAccountName,BankAccountType,BankAccountBalance,BankAccountLimit,UsersId")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {


                if (this.User.IsInRole("Admin"))
                {
                    _context.Add(bankAccount);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    bankAccount.UsersId = userId;
                    _context.Add(bankAccount);
                    await _context.SaveChangesAsync();
                    return Redirect("~/Home/Index");
                }
            }


            return View(bankAccount);
        }

        // GET: BankAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankAccount = await _context.BankAccounts.FindAsync(id);
            if (bankAccount == null)
            {
                return NotFound();
            }
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Name", bankAccount.UsersId);
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BankAccountID,BankAccountName,BankAccountType,BankAccountBalance,BankAccountLimit,UsersId")] BankAccount bankAccount)
        {
            if (id != bankAccount.BankAccountID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (this.User.IsInRole("Admin"))
                    {
                        _context.Update(bankAccount);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        bankAccount.UsersId = userId;
                        _context.Update(bankAccount);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankAccountExists(bankAccount.BankAccountID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (this.User.IsInRole("Admin"))
                {
                    return Redirect("~/BankAccounts/Index");
                }
                else
                {
                    return Redirect("~/Home/Index");
                }
            }
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Id", bankAccount.UsersId);
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankAccount = await _context.BankAccounts
                .Include(b => b.Users)
                .FirstOrDefaultAsync(m => m.BankAccountID == id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bankAccount = await _context.BankAccounts.FindAsync(id);
            _context.BankAccounts.Remove(bankAccount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BankAccountExists(int id)
        {
            return _context.BankAccounts.Any(e => e.BankAccountID == id);
        }
    }
}
