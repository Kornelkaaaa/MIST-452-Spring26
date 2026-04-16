using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BooksSpring26.Models
{
    public class Category
    {
        //[Key] only if its not obvious
        public int CategoryId { get; set; }

        // string? = nullable
        [DisplayName("Category Name: ")]
        [Required(ErrorMessage = "Category name is required")]
        public string Name { get; set; }

        [DisplayName("Description: ")]
        [Required(ErrorMessage = "Description is required")]
        //[MaxLength(100, ErrorMessage = "Description cannot be more than 100 characters")]
        public string Description { get; set; }



    }
}
