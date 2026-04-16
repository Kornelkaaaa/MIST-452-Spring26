using System.Diagnostics;
using System.Security.Claims;
using BooksSpring26.Data;
using BooksSpring26.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksSpring26.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private BooksDbContext _dcContext;

        public HomeController(BooksDbContext DbContext)//dependency / i think is indepent injection he will ask about this on the quiz 
        {
            _dcContext = DbContext;
        }
        public IActionResult Index()
        {
            var listOfBooks = _dcContext.Books.Include(c => c.category); //if you fetch books, include the related category data
            return View(listOfBooks.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Details(int id)
        {
            var book = _dcContext.Books.Include(c => c.category).FirstOrDefault(b => b.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            var cartItem = new CartItem
            {
                BookId = id,
                Book = book,
                Quantity = 1
            };

            return View(cartItem);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(CartItem cartItem)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  //fetches the user id of the currently logged in user

            cartItem.UserId = userId;

            // Check if the cart item already exists for the user and the same book
            CartItem existingCartItem = _dcContext.CartItems.FirstOrDefault(c => c.BookId == cartItem.BookId && c.UserId == userId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += cartItem.Quantity; //if the item already exists, update the quantity
                _dcContext.CartItems.Update(existingCartItem);
            }
            else
            {
                _dcContext.CartItems.Add(cartItem); //if the item does not exist, add it to the cart
            }

            _dcContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
