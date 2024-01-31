using StoringPassword.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System.Linq.Expressions;
using StoringPassword.Repos;
using Newtonsoft.Json;
namespace StoringPassword.Controllers
{
    public class MessagesController : Controller
    {
        private readonly IMessagesRepository _repository;
        public MessagesController(IMessagesRepository repository) => _repository = repository;

        // GET: Messages
        public async Task<IActionResult> Index() => View(await _repository.GetAllMessages());

        public async Task<IActionResult> GetUserMessages()
        {
            User userSpecification = await _repository.GetUserByLogin(HttpContext.Session.GetString("Login")).FirstAsync();
            return View(await _repository.GetUserMessages(userSpecification));
        }

        public async Task<IActionResult> GetJsonUserMessages()
        {
            User userSpecification = await _repository.GetUserByLogin(HttpContext.Session.GetString("Login")).FirstAsync();
            var messages = await _repository.GetUserMessages(userSpecification);

            var jsonSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return Json(JsonConvert.SerializeObject(messages, jsonSettings));
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            Message? msg = await _repository.GetMessageDetails(id);

            if (msg == null)
                return NotFound();

            return Json(msg);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_repository.GetUserByLogin(HttpContext.Session.GetString("Login")), "Id", "Login");
            return View();
        }

        // POST: Messages/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Mesage,UserId")] Message message)
        {
            Console.WriteLine($"Received data: Id={message.Id}, Mesage={message.Mesage}, UserId={message.UserId}");
            await _repository.CreateMessage(message);
            await _repository.SaveChanges();
            return Json(new { success = true, message = "Message created successfully" });
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var message = await _repository.GetMessage(id);

            if (message == null)
                return NotFound();

            return Json(message); // Возвращаем данные в формате JSON
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Mesage,UserId")] Message message)
        {
            Console.WriteLine($"Received data: Id={message.Id}, Mesage={message.Mesage}, UserId={message.UserId}");
            if (id != message.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _repository.UpdateMessage(message);
                    await _repository.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _repository.MessageExists(message.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(GetUserMessages));
            }
            ViewData["UserId"] = new SelectList(_repository.GetUserByLogin(HttpContext.Session.GetString("Login")), "Id", "Login", message.UserId);
            return View(message);
        }

        // GET: Messages/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //        return NotFound();

        //    var message = await _repository.GetMessageDetails(id);

        //    if (message == null)
        //        return NotFound();

        //    return View(message);
        //}

        // POST: Messages/Delete/5
        [HttpDelete]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Console.WriteLine($"Confirmed {id}");
            await _repository.DeleteMessage(id);
            await _repository.SaveChanges();

            // Возвращаем JSON с информацией об успешном удалении
            return Json(new { success = true, message = "Message deleted successfully" });
        }


    }
}
