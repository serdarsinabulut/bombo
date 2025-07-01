using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectbombo.Data;
using projectbombo.Models;
using System.Linq;

namespace projectbombo.Controllers
{
    public class DebtController : Controller
    {
        private readonly AppDbContext _context;

        public DebtController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int userId = int.Parse(User.FindFirst("UserId").Value);

            // Borçlar, kullanıcının borçlarını ve alacaklarını alacak şekilde listelenecek
            var debts = _context.Debts
                .Where(d => d.LenderId == userId || d.BorrowerId == userId)
                .OrderByDescending(d => d.Date)
                .Include(d => d.Lender)  // Lender bilgisini yükle
                .Include(d => d.Borrower) // Borrower bilgisini yükle
                .ToList();

            // Verinin boş olup olmadığını kontrol et
            if (debts == null || !debts.Any())
            {
                ModelState.AddModelError("", "Borçlar bulunamadı.");
            }

            return View(debts);
        }

        public IActionResult Create()
        {
            ViewBag.Users = _context.Users.ToList(); // Kullanıcı listesini çekiyoruz
            return View();
        }

        [HttpPost]
        public IActionResult Create(int borrowerId, decimal amount,string explanation)
        {
            int lenderId = int.Parse(User.FindFirst("UserId").Value);

            if (borrowerId == lenderId)
            {
                ModelState.AddModelError("", "Kendine borç veremezsin!");
                ViewBag.Users = _context.Users.ToList();
                return View();
            }

            var debt = new Debt
            {
                LenderId = lenderId,
                BorrowerId = borrowerId,
                Amount = amount,
                Explanation = explanation,
                IsPaid = false,
                Date = DateTime.Now
            };

            _context.Debts.Add(debt);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Pay(int id)
        {
            var debt = _context.Debts.FirstOrDefault(d => d.Id == id);
            if (debt != null)
            {
                debt.IsPaid = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var debt = _context.Debts.FirstOrDefault(d => d.Id == id);
            if (debt != null)
            {
                _context.Debts.Remove(debt);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}
