using Microsoft.AspNetCore.Mvc;

namespace BorcTakip.Controllers
{
    public class HomeController : Controller
    {
        // Ana sayfa (Hoş geldiniz sayfası)
        public IActionResult Index()
        {
            return View();
        }

        // Çıkış işlemi
        public IActionResult Logout()
        {
            // Kullanıcıyı çıkış yaptırmak için oturumu sonlandırıyoruz
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
