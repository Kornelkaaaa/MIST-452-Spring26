using System.Security.Claims;
using BooksSpring26.Data;
using BooksSpring26.Models;
using BooksSpring26.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksSpring26.Areas.Customer.Controllers
{
    [Authorize]
    [Area("Customer")]
    public class CartItemController : Controller
    {

        private BooksDbContext _dcContext;

        public CartItemController(BooksDbContext DbContext)//dependency / i think is indepent injection he will ask about this on the quiz 
        {
            _dcContext = DbContext;
        }

        public IActionResult Index()
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = _dcContext.CartItems.Where(c => c.UserId == userID).Include(c => c.Book).Include(c => c.Book);

            ShoppingCartVM shoppingCartVM = new ShoppingCartVM
            {
                CartItems = cartItems,
                Order = new Order() ////check this later 
            };


            //subtotall for invidaul cart
            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                var subtotal = cartItem.Quantity * cartItem.Book.Price;

                shoppingCartVM.Order.OrderTotal += subtotal;///add something 
            }

            return View(shoppingCartVM);
        }

        public IActionResult IncrementByOne(int id)
        {
            CartItem cartItem = _dcContext.CartItems.Find(id);

            cartItem.Quantity++;

            _dcContext.Update(cartItem);
            _dcContext.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult DecrementByOne(int id)
        {
            CartItem cartItem = _dcContext.CartItems.Find(id);

            if (cartItem.Quantity <= 1)
            {
                _dcContext.CartItems.Remove(cartItem);
                _dcContext.SaveChanges();
            }
            else
            {
                cartItem.Quantity--;

                _dcContext.Update(cartItem);
                _dcContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public IActionResult RemoveFromCart(int id)
        {
            CartItem cartItem = _dcContext.CartItems.Find(id);

            _dcContext.CartItems.Remove(cartItem);
            _dcContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ReviewOrder()
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItemsList = _dcContext.CartItems.Where(c => c.UserId == userID).Include(c => c.Book).Include(c => c.Book);

            ShoppingCartVM shoppingCartVM = new ShoppingCartVM
            {
                CartItems = cartItemsList,
                Order = new Order()
            };

            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                cartItem.Subtotal = cartItem.Quantity * cartItem.Book.Price;
                shoppingCartVM.Order.OrderTotal += cartItem.Subtotal;
            }


            shoppingCartVM.Order.ApplicationUser = _dcContext.ApplicationUsers.Find(userID);

            shoppingCartVM.Order.CustomerName = shoppingCartVM.Order.ApplicationUser.Name;

            shoppingCartVM.Order.StreetAddress = shoppingCartVM.Order.ApplicationUser.StreetAddress;

            shoppingCartVM.Order.City = shoppingCartVM.Order.ApplicationUser.City;

            shoppingCartVM.Order.State = shoppingCartVM.Order.ApplicationUser.State;

            shoppingCartVM.Order.PostalCode = shoppingCartVM.Order.ApplicationUser.PostalCode;

            shoppingCartVM.Order.Phone = shoppingCartVM.Order.ApplicationUser.PhoneNumber;

            return View();
        }

        [HttpPost]
        [ActionName("ReviewOrder")]
        public IActionResult ReviewOrderPOST(ShoppingCartVM shoppingCartVM)
        {
            //_dcContext.Order.Add(shoppingCartVM.Order);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItemsList = _dcContext.CartItems.Where(c => c.UserId == userId).Include(c => c.Book).Include(c => c.Book);

            shoppingCartVM.CartItems = cartItemsList;

            if (!ModelState.IsValid)
            {
                return View("ReviewOrder", shoppingCartVM);
            }

            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                cartItem.Subtotal = cartItem.Quantity * cartItem.Book.Price;
                shoppingCartVM.Order.OrderTotal += cartItem.Subtotal;
            }

            //set only the server -contrlled firels
            shoppingCartVM.Order.ApplicationUserId = userId;

            shoppingCartVM.Order.OrderDate = DateOnly.FromDateTime(DateTime.Now);

            shoppingCartVM.Order.OrderStatus = "Pending";

            shoppingCartVM.Order.PaymentStatus = "Pending";

            //save order
            _dcContext.Order.Add(shoppingCartVM.Order);
            //create a new order in the orders tabke we can use the order id to link the order details table
            _dcContext.SaveChanges();

            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                OrderDetail orderDetails = new OrderDetail
                {
                    BookId = cartItem.BookId,
                    OrderId = shoppingCartVM.Order.OrderId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Book.Price
                };
                _dcContext.OrderDetail.Add(orderDetails);
            }

            _dcContext.SaveChanges();


            return RedirectToAction("OrderConfiramtion", new { id = shoppingCartVM.Order.OrderId });
        }

        public IActionResult OrderConfiramtion(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<CartItem> listOfCartItems = _dcContext.CartItems.ToList().Where(c => c.UserId == userId).ToList();

            _dcContext.CartItems.RemoveRange(listOfCartItems);
            _dcContext.SaveChanges();

            return View(id);
        }

    }
}