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
    public class TransactionsController : Controller
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
          
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = _context.Users.FirstOrDefault(u => u.Id == userId);
            ViewBag.username = currentUser.Name;
           
         

          

           
            var appDbContext = _context.Transactions.Include(t => t.BankAccount).Include(t => t.Users);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.BankAccount)
                .Include(t => t.Users)
                .FirstOrDefaultAsync(m => m.TransactionID == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Withdraw
        public IActionResult Transfer()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bankaccountsID = _context.BankAccounts.Where(x => x.UsersId == userId).ToList();
            ViewData["BankAccountID"] = new SelectList(bankaccountsID, "BankAccountID", "BankAccountName");
            ViewData["TrasferAccount"] =new SelectList(_context.BankAccounts, "BankAccountID", "BankAccountName");
            ViewBag.Accounts = _context.BankAccounts.ToList();
           



            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer([Bind("TransactionID,TransactionType,TransactionAmount,TransactionDate,UsersId,BankAccountID,TransferBankAccountID")] Transaction transaction)
        {
            var bankaccountID = transaction.BankAccountID;
            BankAccount bankaccount = _context.BankAccounts.Find(bankaccountID);

            var transferbankaccountID = transaction.TransferBankAccountID;
            BankAccount Transferbankaccount = _context.BankAccounts.Find(transferbankaccountID);


            //BankAccount transferAccount = transaction.TrasferAccount;
            var newBalance = bankaccount.BankAccountBalance - transaction.TransactionAmount;
            var transferAccountBalance = Transferbankaccount.BankAccountBalance + transaction.TransactionAmount;
            if (ModelState.IsValid & newBalance < bankaccount.BankAccountLimit & transferAccountBalance< Transferbankaccount.BankAccountLimit)
            {

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                transaction.UsersId = userId;
                transaction.TransactionDate = DateTime.Now;
                transaction.TransactionType = "Transfer";

                bankaccount.BankAccountBalance = newBalance;
                Transferbankaccount.BankAccountBalance = transferAccountBalance;

                _context.BankAccounts.Attach(Transferbankaccount);
                _context.Entry(Transferbankaccount).State = EntityState.Modified;
                    
                _context.BankAccounts.Attach(bankaccount);
                _context.Entry(bankaccount).State = EntityState.Modified;

                _context.SaveChanges();

                _context.Add(transaction);

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            ViewData["BankAccountID"] = new SelectList(_context.BankAccounts, "BankAccountID", "BankAccountName", transaction.BankAccountID);
            ViewData["TrasferAccount"] = new SelectList(_context.BankAccounts, "BankAccountID", "BankAccountName");
            ViewBag.Accounts = _context.BankAccounts.ToList();

       


            return View(transaction);
        }
        public IActionResult Withdraw()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bankaccountsID = _context.BankAccounts.Where(x => x.UsersId == userId).ToList();
            ViewData["BankAccountID"] = new SelectList(bankaccountsID, "BankAccountID", "BankAccountName");
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw([Bind("TransactionID,TransactionType,TransactionAmount,TransactionDate,UsersId,BankAccountID")] Transaction transaction)
        {
            var bankaccountID = transaction.BankAccountID;
            BankAccount bankaccount = _context.BankAccounts.Find(bankaccountID);
            var newBalance = bankaccount.BankAccountBalance - transaction.TransactionAmount;
            if (ModelState.IsValid & newBalance < bankaccount.BankAccountLimit)
            {

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                transaction.UsersId = userId;
                transaction.TransactionDate = DateTime.Now;
                transaction.TransactionType = "Withdraw";


                bankaccount.BankAccountBalance = newBalance;
                _context.BankAccounts.Attach(bankaccount);
                _context.Entry(bankaccount).State = EntityState.Modified;

                _context.SaveChanges();

                _context.Add(transaction);

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            ViewData["BankAccountID"] = new SelectList(_context.BankAccounts, "BankAccountID", "BankAccountID", transaction.BankAccountID);
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Id", transaction.UsersId);
            return View(transaction);
        }
        // GET: Transactions/Deposit

        public  IActionResult Deposit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
           
            var bankaccountsID = _context.BankAccounts.Where(x => x.UsersId == userId).ToList();
            ViewData["BankAccountID"] = new SelectList(bankaccountsID, "BankAccountID", "BankAccountName");
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }
        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit([Bind("TransactionID,TransactionType,TransactionAmount,TransactionDate,UsersId,BankAccountID")] Transaction transaction)
        {
            var bankaccountID = transaction.BankAccountID;
            BankAccount bankaccount = _context.BankAccounts.Find(bankaccountID);
            var newBalance = bankaccount.BankAccountBalance + transaction.TransactionAmount;
            if (ModelState.IsValid & newBalance < bankaccount.BankAccountLimit)
            {

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                transaction.UsersId = userId;
                transaction.TransactionDate = DateTime.Now;
                transaction.TransactionType = "Deposit";
              
                
                bankaccount.BankAccountBalance = newBalance;
                _context.BankAccounts.Attach(bankaccount);
                _context.Entry(bankaccount).State = EntityState.Modified;

                _context.SaveChanges();

                _context.Add(transaction);
               
                await _context.SaveChangesAsync();
                return RedirectToAction ("Index","Home");
            }
           
            ViewData["BankAccountID"] = new SelectList(_context.BankAccounts, "BankAccountID", "BankAccountID", transaction.BankAccountID);
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Id", transaction.UsersId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["BankAccountID"] = new SelectList(_context.BankAccounts, "BankAccountID", "BankAccountID", transaction.BankAccountID);
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Id", transaction.UsersId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionID,TransactionType,TransactionAmount,TransactionDate,UsersId,BankAccountID")] Transaction transaction)
        {
            if (id != transaction.TransactionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionID))
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
            ViewData["BankAccountID"] = new SelectList(_context.BankAccounts, "BankAccountID", "BankAccountID", transaction.BankAccountID);
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Id", transaction.UsersId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.BankAccount)
                .Include(t => t.Users)
                .FirstOrDefaultAsync(m => m.TransactionID == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionID == id);
        }
    }
}
