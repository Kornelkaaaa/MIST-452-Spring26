using BooksSpring26;
using BooksSpring26.Data;
using BooksSpring26.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksSpring26.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DbInitializer.AdminRole)]
    public class UserController : Controller
    {
        private readonly BooksDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(BooksDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? searchTerm)
        {
            var query = _dbContext.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(u =>
                    (u.Name != null && u.Name.Contains(term)) ||
                    (u.Email != null && u.Email.Contains(term)) ||
                    (u.UserName != null && u.UserName.Contains(term)));
            }

            var users = query.OrderBy(u => u.Email).ToList();

            var adminIds = new HashSet<string>();
            foreach (var u in users)
            {
                var identityUser = await _userManager.FindByIdAsync(u.Id);
                if (identityUser != null && await _userManager.IsInRoleAsync(identityUser, DbInitializer.AdminRole))
                {
                    adminIds.Add(u.Id);
                }
            }

            ViewData["SearchTerm"] = searchTerm;
            ViewData["AdminIds"] = adminIds;
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (await _userManager.IsInRoleAsync(user, DbInitializer.AdminRole))
            {
                await _userManager.RemoveFromRoleAsync(user, DbInitializer.AdminRole);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, DbInitializer.AdminRole);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(string id)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var orderCount = _dbContext.Order.Count(o => o.ApplicationUserId == id);
            ViewData["OrderCount"] = orderCount;
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> LockUnlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(100);
            }

            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}
