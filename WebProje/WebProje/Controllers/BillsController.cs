using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebProje.Data;
using WebProje.Models;

namespace WebProje.Controllers
{
    [Authorize]
    public class BillsController : Controller
    {
        private readonly AppDbContext _context;

        public BillsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Bills
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Bills.Include(b => b.Company).Include(b => b.Users);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Bills/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills
                .Include(b => b.Company)
                .Include(b => b.Users)
                .FirstOrDefaultAsync(m => m.BillID == id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // GET: Bills/Create
        public IActionResult Create()
        {
            ViewData["CompanyID"] = new SelectList(_context.Companies, "CompanyID", "CompanyName");
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Name");

            return View();
        }
        // GET: Bills/Pay

        // POST: Bills/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BillID,BillDesc,BillAmount,BillStatus,BillDate,CompanyID,UsersId")] Bill bill)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = _context.Users.FirstOrDefault(u => u.Id == userId);

            var company = _context.Companies.FirstOrDefault(x => x.CompanyID == bill.CompanyID);
            if (ModelState.IsValid)
            {
                bill.BillDate = DateTime.Now;
                bill.BillStatus = false;
               

                _context.Add(bill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["CompanyID"] = new SelectList(_context.Companies, "CompanyID", "CompanyID", bill.CompanyID);
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "UserName", bill.UsersId);

            return View(bill);
        }
        // GET: Bills/Pay/5
        public async Task<IActionResult> Pay(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bankaccounts = _context.BankAccounts.Where(x => x.UsersId == userId).ToList();
            ViewData["BankAccount"] = new SelectList(bankaccounts, "BankAccountID", "BankAccountName");
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Name", bill.UsersId);

            Bank bank = _context.Banks.FirstOrDefault();
            ViewData["Fee"] = bill.BillAmount * bank.BankFeeRatio;


            return View(bill);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int id, [Bind("BillID,BillDesc,BillAmount,BillStatus,BillDate,CompanyID,UsersId,BankAccountID")] Bill bill)
        {
            if (id != bill.BillID)
            {
                return NotFound();
            }
            var bankaccountID = bill.BankAccountID;
            BankAccount bankaccount = _context.BankAccounts.Find(bankaccountID);

            Bill payedBill = _context.Bills.Find(id);
            bill = payedBill;
            bill.BillDesc = payedBill.BillDesc;
            bill.BillAmount = payedBill.BillAmount;
            bill.BillDate = payedBill.BillDate;
            bill.CompanyID = payedBill.CompanyID;
            bill.Company = payedBill.Company;
            bill.UsersId = payedBill.UsersId;
            bill.BankAccountID = payedBill.BankAccountID;
            if (bankaccount.BankAccountBalance >= bill.BillAmount)
            {
                try
                {
                    bill.BillStatus = true;

                    bankaccount.BankAccountBalance -= bill.BillAmount;
                    _context.BankAccounts.Attach(bankaccount);
                    _context.Entry(bankaccount).State = EntityState.Modified;


                    Bank bank = _context.Banks.FirstOrDefault();
                    bank.BankMoney += bill.BillAmount * bank.BankFeeRatio;

                    _context.Banks.Attach(bank);
                    _context.Entry(bank).State = EntityState.Modified;
                   
                    _context.SaveChanges();

                    _context.Update(bill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillExists(bill.BillID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
               
            }
            else
            {
                return RedirectToAction("Pay", "Bills");
            }
            ViewData["CompanyID"] = new SelectList(_context.Companies, "CompanyID", "CompanyID", bill.CompanyID);
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Id", bill.UsersId);
            return RedirectToAction("Index", "Home");
        }



        // GET: Bills/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }
            ViewData["CompanyID"] = new SelectList(_context.Companies, "CompanyID", "CompanyName", bill.CompanyID);
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Name", bill.UsersId);
            return View(bill);
        }

        // POST: Bills/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BillID,BillDesc,BillAmount,BillStatus,BillDate,CompanyID,UsersId")] Bill bill)
        {
            if (id != bill.BillID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillExists(bill.BillID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyID"] = new SelectList(_context.Companies, "CompanyID", "CompanyID", bill.CompanyID);
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Id", bill.UsersId);
            return View(bill);
        }

        // GET: Bills/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills
                .Include(b => b.Company)
                .Include(b => b.Users)
                .FirstOrDefaultAsync(m => m.BillID == id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillExists(int id)
        {
            return _context.Bills.Any(e => e.BillID == id);
        }
    }
}
