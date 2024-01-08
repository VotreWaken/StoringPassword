using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoringPassword.Models;
using System.Security.Cryptography;
using System.Text;

namespace StoringPassword.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserContext _context;

        public AccountController(UserContext context)
        {
            _context = context;
        }

        // Login 
        public ActionResult Login()
        {
            return View();
        }
        // 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel logon)
        {
            if (ModelState.IsValid)
            {
                if(_context.Users.ToList().Count == 0)
                {
                    ModelState.AddModelError("", "Wrong login or password!");
                    return View(logon);
                }
                var users = _context.Users.Where(a => a.Login == logon.Login);
                if (users.ToList().Count == 0)
                {
                    ModelState.AddModelError("", "Wrong login or password!");
                    return View(logon);
                }
                var user = users.First();
                string? salt = user.Salt;

                //переводим пароль в байт-массив  
                byte[] password = Encoding.Unicode.GetBytes(salt + logon.Password);

                //создаем объект для получения средств шифрования  
                var md5 = MD5.Create();

                //вычисляем хеш-представление в байтах  
                byte[] byteHash = md5.ComputeHash(password);

                StringBuilder hash = new StringBuilder(byteHash.Length);
                for (int i = 0; i < byteHash.Length; i++)
                    hash.Append(string.Format("{0:X2}", byteHash[i]));

                if (user.Password != hash.ToString())
                {
                    ModelState.AddModelError("", "Wrong login or password!");
                    return View(logon);
                }
                HttpContext.Session.SetString("FirstName", user.FirstName);
                HttpContext.Session.SetString("LastName", user.LastName);
                HttpContext.Session.SetString("Login", user.Login);
                return RedirectToAction("Index", "Home");
            }
            return View(logon);
        }

        // Register 
        public IActionResult Register()
        {
            return View();
        }

        // Register 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterModel reg)
        {
            if (ModelState.IsValid)
            {
                User user = new User();
                user.FirstName = reg.FirstName;
                user.LastName = reg.LastName;
                user.Login = reg.Login;

                byte[] saltbuf = new byte[16];

                RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                randomNumberGenerator.GetBytes(saltbuf);

                StringBuilder sb = new StringBuilder(16);
                for (int i = 0; i < 16; i++)
                    sb.Append(string.Format("{0:X2}", saltbuf[i]));
                string salt = sb.ToString();

                //переводим пароль в байт-массив  
                byte[] password = Encoding.Unicode.GetBytes(salt + reg.Password);

                //создаем объект для получения средств шифрования  
                var md5 = MD5.Create();

                //вычисляем хеш-представление в байтах  
                byte[] byteHash = md5.ComputeHash(password);

                StringBuilder hash = new StringBuilder(byteHash.Length);
                for (int i = 0; i < byteHash.Length; i++)
                    hash.Append(string.Format("{0:X2}", byteHash[i]));

                user.Password = hash.ToString();
                user.Salt = salt;
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }

            return View(reg);
        }
    }
}