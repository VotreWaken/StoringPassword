using GuestsBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using StoringPassword.Models;

namespace StoringPassword.Controllers
{
    public class MessagesController : Controller
    {
        private readonly UserContext _context;

        public MessagesController(UserContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index() => View(await GetAllMessages());

        public async Task<List<Message>> GetAllMessages()
        {
            var myDbContext = _context.Messages.Include(x => x.User);
            return await myDbContext.ToListAsync();
        }

        public async Task<IActionResult> MyMessages()
        {
            User userSpecification = await GetUserByLogin(HttpContext.Session.GetString("Login")).FirstAsync();
            return View(await GetSelfMessages(userSpecification));
        }
        public IQueryable<User> GetUserByLogin(string? login) => _context.Users.Where(x => x.Login == login);
        public async Task<List<Message>> GetSelfMessages(User concreteUser)
        {
            var myDbContext = _context.Messages.Where(m => m.User.Id == concreteUser.Id);
            return await myDbContext.ToListAsync();
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            Message? msg = await GetMessageDetails(id);

            if (msg == null)
                return NotFound();

            return View(msg);
        }
        public async Task<Message?> GetMessageDetails(int? id) =>
          await _context.Messages
          .Include(m => m.User)
          .FirstOrDefaultAsync(m => m.Id == id);
        // GET: Messages/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(GetUserByLogin(HttpContext.Session.GetString("Login")), "Id", "Login");
            return View();
        }

        // POST: Messages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Mesage,UserId")] Message message)
        {
            if (ModelState.IsValid)
            {
                await CreateMessage(message);
                await SaveChanges();
                return RedirectToAction(nameof(MyMessages));
            }
            ViewData["UserId"] = new SelectList(GetUserByLogin(HttpContext.Session.GetString("Login")), "Id", "Login", message.UserId);
            return View(message);
        }
        public async Task CreateMessage(Message msg) => await _context.AddAsync(msg);
        public async Task SaveChanges() => await _context.SaveChangesAsync();
        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var message = await GetMessage(id);

            if (message == null)
                return NotFound();

            ViewData["UserId"] = new SelectList(GetUserByLogin(HttpContext.Session.GetString("Login")), "Id", "Login", message.UserId);

            return View(message);
        }
        public async Task<Message?> GetMessage(int? id) => await _context.Messages.FindAsync(id);
        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Mesage,UserId")] Message message)
        {
            if (id != message.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    UpdateMessage(message);
                    await SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await MessageExists(message.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(MyMessages));
            }
            ViewData["UserId"] = new SelectList(GetUserByLogin(HttpContext.Session.GetString("Login")), "Id", "Login", message.UserId);
            return View(message);
        }
        public void UpdateMessage(Message msg) => _context.Update(msg);

        public async Task<bool> MessageExists(int id) => await _context.Messages.AnyAsync(e => e.Id == id);
        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var message = await GetMessageDetails(id);

            if (message == null)
                return NotFound();

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await DeleteMessage(id);
            await SaveChanges();
            return RedirectToAction(nameof(MyMessages));
        }

        public async Task DeleteMessage(int id)
        {
            var message = await GetMessage(id);

            if (message != null)
                _context.Messages.Remove(message);
        }

    }
}
