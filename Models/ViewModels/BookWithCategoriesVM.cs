using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BooksSpring26.Models.ViewModels
{
    public class BookWithCategoriesVM
    {
        public Book Book { get; set; } = new Book(); //initialize the Book property with a new Book object
        [ValidateNever]
        public IEnumerable<SelectListItem> ListOfCategories { get; set; } = new List<SelectListItem>(); //initialize the ListOfCategories property with a new list of SelectListItem objects
    }
}
