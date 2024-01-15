using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoringPassword.Models;
using StoringPassword.Repos;
using System.Security.Cryptography;
using System.Text;

namespace StoringPassword.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountsRepository _repository;
        public AccountController(IAccountsRepository repository)
        {
            _repository = repository;
        }

        // GET: AccountController/Login
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if ((await _repository.GetAllUsers()).Count == 0)
                return RedirectToAction("Regist", "Account");

            return View();
        }

        // POST: AccountController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if ((await _repository.GetAllUsers()).Count == 0)
                return RedirectToAction("Regist", "Account");
            if (!ModelState.IsValid)
                return View(model);

            IQueryable<User> users = _repository.GetUsersByLogin(model);

            if (!users.Any())
            {
                ModelState.AddModelError("", "Incorrect login or password!");
                return View(model);
            }

            User user = users.First();

            if (await _repository.IsPasswordCorrect(user, model))
            {
                ModelState.AddModelError("", "Incorrect login or password!");
                return View(model);
            }
            HttpContext.Session.SetString("FirstName", user.FirstName ?? string.Empty);
            HttpContext.Session.SetString("LastName", user.LastName ?? string.Empty);
            HttpContext.Session.SetString("Login", user.Login);
            return RedirectToAction("Index", "Home");
        }

        // GET: AccountController/Registration
        [HttpGet]
        public ActionResult Registration() => View();

        // POST: AccountController/Registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            if (await _repository.IsLoginExists(model.Login))
            {
                ModelState.AddModelError("", "This login is taken!");
                return View(model);
            }

            User user = new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Login = model.Login
            };
            user = await _repository.CreateAndHashPassword(user, model);

            await _repository.AddUserToDb(user);

            return RedirectToAction("Login");
        }
    }
}