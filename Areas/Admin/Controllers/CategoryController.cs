using BooksSpring26.Data;
using BooksSpring26.Models;
using Microsoft.AspNetCore.Mvc;

namespace BooksSpring26.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private BooksDbContext _dbContext;

        public CategoryController(BooksDbContext dbContext)//indepent injection of database context
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var listOfCategories = _dbContext.Categories.ToList();

            return View(listOfCategories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category newCategory)
        {
            if(newCategory.Name.ToLower() == "test") //custom validation
            {
                ModelState.AddModelError("Name", "Category name cannot be 'test', do not be silly");
            }

            if(ModelState.IsValid) //if my model is valid , we are saving new category to database
            {
                _dbContext.Categories.Add(newCategory);
                _dbContext.SaveChanges();

                return RedirectToAction("Index", "Category"); //redirecting to index action of category controller
            }

            return View(); //if model is not valid, return the same view to show validation errors
        }

        [HttpGet] //to show the edit form
        public IActionResult Edit(int id)
        {
            Category categoryToEdit = _dbContext.Categories.Find(id);

            return View(categoryToEdit);
        }

        [HttpPost] //to handle the form submission
        public IActionResult Edit(int id, [Bind("CategoryId, Name, Description")] Category category) //you have the id and the form about updated category, user can change only cateogries in bin
        {
            if(ModelState.IsValid)
            {
                _dbContext.Categories.Update(category);
                _dbContext.SaveChanges();
                return RedirectToAction("Index", "Category");
            }

            return View(category); //if model is not valid, return the same view to show validation errors
        }

        [HttpGet] //to show the delete confirmation page
        public IActionResult Delete(int id)
        {
            Category categoryToDelete = _dbContext.Categories.Find(id);

            return View(categoryToDelete);
        }

        [HttpPost]
        public IActionResult Delete(Category categoryToDelete) //to handle the actual deletion
        {
           // Category categoryToDelete = _dbContext.Categories.Find(id); bc we define this as parameter 
            
            _dbContext.Categories.Remove(categoryToDelete);
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Category");
        }

        public IActionResult Details(int id)
        {
            Category categoryDetails = _dbContext.Categories.Find(id);
            return View(categoryDetails);
        }

    }
}
