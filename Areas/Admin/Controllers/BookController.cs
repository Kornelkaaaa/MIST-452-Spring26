using BooksSpring26.Data;
using BooksSpring26.Models;
using BooksSpring26.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

//// "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BooksDbSpr2026;trusted_connection=true"
///
namespace BooksSpring26.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DbInitializer.AdminRole)]
    public class BookController : Controller
    {
        private BooksDbContext _dbContext;

        private IWebHostEnvironment _environment;
        public BookController(BooksDbContext dbContext, IWebHostEnvironment environment)//DOUBLE dependency injection
        {
            _dbContext = dbContext;
            _environment = environment;
        }
        public IActionResult Index() //stundent have to create 
        {

            var books = _dbContext.Books.Include(b => b.category).ToList();//Include(b => b.category).
            return View(books); //view fetches the blank form 


        }

        [HttpGet]
        public IActionResult Create()
        {
            //fetch a list of categories to be passed to the view
            IEnumerable<SelectListItem> listOfCategories = _dbContext.Categories.ToList().Select
                (
                                                                o => new SelectListItem
                                                                {
                                                                    Text = o.Name,
                                                                    Value = o.CategoryId.ToString()
                                                                }//linq query to fetch categories from database and convert them to SelectListItem objects

                );
            //PROJECTION: An object of a data type is transformed into another data type.

            //ViewBag.ListOfCategories = listOfCategories; //passing the list of categories to the view using ViewBag

            //ViewData["ListOfCategoriesVD"] = listOfCategories;

            //View Model is used to support a view when the data that you'd like to display is more complex than a existing model
            BookWithCategoriesVM bookWithCategoriesVMobj = new BookWithCategoriesVM();

            bookWithCategoriesVMobj.Book = new Book(); //initialize the Book property with a new Book object

            bookWithCategoriesVMobj.ListOfCategories = listOfCategories; //assign the list of categories to the ViewModel property

            return View(bookWithCategoriesVMobj); //view fetches the blank form I MISSED

        }

        [HttpPost]
        public IActionResult Create(BookWithCategoriesVM bookWithCategoriesVMobj, IFormFile imgFile)
        {

            if (ModelState.IsValid) //if my model is valid , we are saving new category to database
            {
                string wwwrootPath = _environment.WebRootPath; //get the root path of the web application
                if (imgFile != null)
                {
                    //save the file to the wwwroot directly 
                    using (var filestream = new FileStream(Path.Combine(wwwrootPath, @"Images\" + imgFile.FileName), FileMode.Create))
                    {
                        imgFile.CopyTo(filestream); //copy the uploaded image file to the specified path in the wwwroot folder
                    }
                    //saving the url of the uploaded image to the database
                    bookWithCategoriesVMobj.Book.ImgUrl = @"\Images\" + imgFile.FileName; //set the ImgUrl property of the Book object to the path of the uploaded image
                }

                _dbContext.Books.Add(bookWithCategoriesVMobj.Book);
                _dbContext.SaveChanges();

                return RedirectToAction("Index"); //redirecting to index action of category controller
            }

            return View(bookWithCategoriesVMobj); //if model is not valid, return the same view to show validation errors
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Book book = _dbContext.Books.Find(id);

            IEnumerable<SelectListItem> listOfCategories = _dbContext.Categories.ToList().Select
                 (
                                                                 o => new SelectListItem
                                                                 {
                                                                     Text = o.Name,
                                                                     Value = o.CategoryId.ToString()
                                                                 }

                 );

            BookWithCategoriesVM bookWithCategoriesVMobj = new BookWithCategoriesVM();

            bookWithCategoriesVMobj.Book = book; //initialize the Book property with a new Book object

            bookWithCategoriesVMobj.ListOfCategories = listOfCategories; //assign the list of categories to the ViewModel property

            return View(bookWithCategoriesVMobj);
        }

        [HttpPost]
        public IActionResult Edit(BookWithCategoriesVM bookWithCategoriesVMobj, IFormFile? imgFile)
        {
            string wwwrootPath = _environment.WebRootPath; //get the root path of the web application

            if (ModelState.IsValid)
            {
                if(imgFile != null)
                {
                    if(!string.IsNullOrEmpty(bookWithCategoriesVMobj.Book.ImgUrl)) //check if there is an existing image url for the book
                    {
                        var OldImgPath = Path.Combine(wwwrootPath, bookWithCategoriesVMobj.Book.ImgUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(OldImgPath))//check if the old image file exists in the wwwroot folder
                        {
                            System.IO.File.Delete(OldImgPath); //delete the old image file from the wwwroot folder
                        }
                    }

                    using (var filestream = new FileStream(Path.Combine(wwwrootPath, @"Images\" + imgFile.FileName), FileMode.Create))
                    {
                        imgFile.CopyTo(filestream); //copy the uploaded image file to the specified path in the wwwroot folder
                    }

                    bookWithCategoriesVMobj.Book.ImgUrl = @"\Images\" + imgFile.FileName; //set the ImgUrl property of the Book object to the path of the uploaded image
                }

                _dbContext.Books.Update(bookWithCategoriesVMobj.Book);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");

            }

            return View(bookWithCategoriesVMobj);

        }

        public IActionResult Details(int id)
        {
            var book = _dbContext.Books.Include(b => b.category).FirstOrDefault(b => b.BookId == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var book = _dbContext.Books.Include(b => b.category).FirstOrDefault(b => b.BookId == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var book = _dbContext.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(book.ImgUrl))
            {
                var imgPath = Path.Combine(_environment.WebRootPath, book.ImgUrl.TrimStart('\\'));
                if (System.IO.File.Exists(imgPath))
                {
                    System.IO.File.Delete(imgPath);
                }
            }

            _dbContext.Books.Remove(book);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
