using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using projectbombo.Data;
using projectbombo.Models;
using System.Linq;
using System.Threading.Tasks;

namespace projectbombo.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("UserId", user.Id.ToString()) // Kullanıcı ID'yi tutalım
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Tarayıcıyı kapatsa bile giriş açık kalsın
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                              new ClaimsPrincipal(claimsIdentity),
                                              authProperties);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public IActionResult Register(string username, string password, string confirmPassword)
        {
            // Kullanıcı adı daha önce alınmış mı?
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == username);
            if (existingUser != null)
            {
                ViewBag.Error = "Bu kullanıcı adı zaten alınmış!";
                return View();
            }

            // Şifreler uyuşuyor mu?
            if (password != confirmPassword)
            {
                ViewBag.Error = "Şifreler uyuşmuyor!";
                return View();
            }

            // Yeni kullanıcıyı ekleyelim
            var newUser = new User
            {
                Username = username,
                Password = password // Şifreyi basitçe kaydediyoruz. Şifreleri gerçek projelerde hash'lemelisiniz!
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();

            // Kayıt işlemi tamamlandıktan sonra giriş sayfasına yönlendir
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

    }
}
