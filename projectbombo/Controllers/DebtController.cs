using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            // Borçlar, kullanıcının borçlarını ve alacaklarını alacak şekilde listelenecek
            var debts = _context.Debts
                .Where(d => d.LenderId == userId || d.BorrowerId == userId)
                .OrderByDescending(d => d.Date)
                .Include(d => d.Lender)  // Lender bilgisini yükle
                .Include(d => d.Borrower) // Borrower bilgisini yükle
                .ToList();

            // Verinin boş olup olmadığını kontrol et
            if (debts.IsNullOrEmpty())
            {
                ModelState.AddModelError("", "Borçlar bulunamadı.");
            }

            return View(debts);
        }

        public IActionResult Create()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ViewBag.Users = _context.Users.ToList(); // Kullanıcı listesini çekiyoruz
            return View();
        }

        [HttpPost]
        public IActionResult Create(int borrowerId, decimal amount, string explanation)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Users = _context.Users.ToList();
                return View();
            }
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int lenderId))
            {
                return Unauthorized();
            }

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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
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
