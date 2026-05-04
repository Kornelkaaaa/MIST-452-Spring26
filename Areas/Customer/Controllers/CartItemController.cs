using System.Security.Claims;
using BooksSpring26.Data;
using BooksSpring26.Models;
using BooksSpring26.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

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


            shoppingCartVM.Order.ApplicationUser = _dcContext.Users.Find(userID);

            shoppingCartVM.Order.CustomerName = shoppingCartVM.Order.ApplicationUser.Name;

            shoppingCartVM.Order.StreetAddress = shoppingCartVM.Order.ApplicationUser.StreetAddress;

            shoppingCartVM.Order.City = shoppingCartVM.Order.ApplicationUser.City;

            shoppingCartVM.Order.State = shoppingCartVM.Order.ApplicationUser.State;

            shoppingCartVM.Order.PostalCode = shoppingCartVM.Order.ApplicationUser.PostalCode;

            shoppingCartVM.Order.Phone = shoppingCartVM.Order.ApplicationUser.PhoneNumber;

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ActionName("ReviewOrder")]
        public IActionResult ReviewOrderPOST(ShoppingCartVM shoppingCartVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItemsList = _dcContext.CartItems.Where(c => c.UserId == userId).Include(c => c.Book);

            shoppingCartVM.CartItems = cartItemsList;

            if (!ModelState.IsValid)
            {
                return View("ReviewOrder", shoppingCartVM);
            }

            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                cartItem.Subtotal = cartItem.Book.Price * cartItem.Quantity;
                shoppingCartVM.Order.OrderTotal += cartItem.Subtotal;
            }

            //set only the server-controlled fields
            shoppingCartVM.Order.ApplicationUserId = userId;
            shoppingCartVM.Order.OrderDate = DateOnly.FromDateTime(DateTime.Now);
            shoppingCartVM.Order.OrderStatus = "Pending";
            shoppingCartVM.Order.PaymentStatus = "Pending";

            //save order - creates a new order so we can use OrderId for order details
            _dcContext.Order.Add(shoppingCartVM.Order);
            _dcContext.SaveChanges();

            //save order details
            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    OrderId = shoppingCartVM.Order.OrderId,
                    BookId = cartItem.BookId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Book.Price
                };
                _dcContext.OrderDetail.Add(orderDetail);
            }
            _dcContext.SaveChanges();

            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"customer/cartitem/OrderConfirmation?id={shoppingCartVM.Order.OrderId}",
                CancelUrl = domain + "customer/cartitem/index",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in shoppingCartVM.CartItems)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Book.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Book.Title
                        }
                    },
                    Quantity = item.Quantity
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);
            shoppingCartVM.Order.SessionID = session.Id;
            _dcContext.SaveChanges();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult OrderConfirmation(int id)
        {
            Order order = _dcContext.Order.Find(id);
            var sessID = order.SessionID;
            var service = new SessionService();
            Session session = service.Get(sessID);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                order.PaymentIntentID = session.PaymentIntentId;
                order.PaymentStatus = "Approved";
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<CartItem> listOfCartItems = _dcContext.CartItems.ToList().Where(c => c.UserId == userId).ToList();

            _dcContext.CartItems.RemoveRange(listOfCartItems);
            _dcContext.SaveChanges();

            return View(id);
        }

    }
}
