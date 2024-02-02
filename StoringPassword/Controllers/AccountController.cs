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
        public async Task<IActionResult> Login(LoginModel model)
        {
            Console.WriteLine("Login: " + model.Login);
            Console.WriteLine("Password: " + model.Password);

            if ((await _repository.GetAllUsers()).Count == 0)
                return RedirectToAction("Regist", "Account");

            if (!ModelState.IsValid)
                return BadRequest("Incorrect login or password!");

            IQueryable<User> users = _repository.GetUsersByLogin(model);

            if (!users.Any())
                return BadRequest("Incorrect login or password!");

            User user = users.First();

            if (await _repository.IsPasswordCorrect(user, model))
                return BadRequest("Incorrect login or password!");

            HttpContext.Session.SetString("FirstName", user.FirstName ?? string.Empty);
            HttpContext.Session.SetString("LastName", user.LastName ?? string.Empty);
            HttpContext.Session.SetString("Login", user.Login);

            return Ok();
        }
        [HttpGet]
        public IActionResult GetUserInfo()
        {
            var userInfo = new
            {
                FirstName = HttpContext.Session.GetString("FirstName"),
                LastName = HttpContext.Session.GetString("LastName"),
                Login = HttpContext.Session.GetString("Login")
            };

            return Json(userInfo);
        }
        // GET: AccountController/Registration
        // [HttpGet]
        // public ActionResult Registration() => View();

        // POST: AccountController/Registration
        [HttpPost]
        public async Task<IActionResult> Registration(RegisterModel model)
        {
            Console.WriteLine("Login: " + model.Login);
            Console.WriteLine("Password: " + model.Password);
            if (!ModelState.IsValid)
            {
                string response1 = "Регистрация Провалилась Некорректные данные!";
                return Json(response1);
            }
            if (await _repository.IsLoginExists(model.Login))
            {
                string response2 = "Регистрация Провалилась Логин Уже Занят!";
                return Json(response2);
            }

            User user = new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Login = model.Login
            };
            user = await _repository.CreateAndHashPassword(user, model);

            await _repository.AddUserToDb(user);

            string response = "Регистрация Успешна!";
            return Json(response);
        }
    }
}