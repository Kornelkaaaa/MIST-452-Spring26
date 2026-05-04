using BooksSpring26.Data;
using BooksSpring26.Models;
using BooksSpring26.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksSpring26.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DbInitializer.AdminRole)]
    public class OrderController : Controller
    {
        [BindProperty]
        public OrderVM orderVM { get; set; }//idk about this 

        private readonly BooksDbContext _dbContext;
        public OrderController(BooksDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            IEnumerable<Order> orders = _dbContext.Order.Include(o => o.ApplicationUser);
            return View(orders);
        }
        public IActionResult Details(int id)
        {
            var order = _dbContext.Order.Include(o => o.ApplicationUser).FirstOrDefault(o => o.OrderId == id);
            var orderDetails = _dbContext.OrderDetail.Where(od => od.OrderId == id).Include(od => od.Book).ToList();
            OrderVM orderVM = new OrderVM
            {
                Order = order,
                OrderDetail = orderDetails
            };
            return View(orderVM);
        }

        [HttpPost]
        public IActionResult UpdateOrderDetails()
        {
            Order orderFromDb = _dbContext.Order.Find(orderVM.Order.OrderId);

            orderFromDb.CustomerName = orderVM.Order.CustomerName;
            orderFromDb.StreetAddress = orderVM.Order.StreetAddress;
            orderFromDb.City = orderVM.Order.City;
            orderFromDb.State = orderVM.Order.State;
            orderFromDb.PostalCode = orderVM.Order.PostalCode;
            orderFromDb.Phone = orderVM.Order.Phone;
            orderFromDb.OrderStatus = orderVM.Order.OrderStatus;
            orderFromDb.PaymentStatus = orderVM.Order.PaymentStatus;
            orderFromDb.Carrier = orderVM.Order.Carrier;
            orderFromDb.TrackingNumber = orderVM.Order.TrackingNumber;
            orderFromDb.ShippingDate = orderVM.Order.ShippingDate;

            _dbContext.Order.Update(orderFromDb);
            _dbContext.SaveChanges();

            return RedirectToAction("Details", new {id = orderFromDb.OrderId}); //idk if this is right, but it should work. Redirects to the details page of the order that was just updated.
        }
        [HttpPost]
        public IActionResult ProcessOrder()
        {
            Order orderFromDb = _dbContext.Order.Find(orderVM.Order.OrderId);

            orderFromDb.OrderStatus = "Processing";

            orderFromDb.ShippingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)); //set the shipping date to 7 days from now, just for testing purposes. In a real application, this would be determined by the shipping method and other factors.

            _dbContext.Order.Update(orderFromDb);
            _dbContext.SaveChanges();

            return RedirectToAction("Details", new { id = orderFromDb.OrderId });
        }

        [HttpPost]
        public IActionResult CompleteOrder()
        {
            Order orderFromDb = _dbContext.Order.Find(orderVM.Order.OrderId);

            orderFromDb.OrderStatus = "Shipped and Completed";

            orderFromDb.ShippingDate = DateOnly.FromDateTime(DateTime.Now);

            _dbContext.Order.Update(orderFromDb);
            _dbContext.SaveChanges();

            return RedirectToAction("Details", new { id = orderFromDb.OrderId });
        }
    }
}
