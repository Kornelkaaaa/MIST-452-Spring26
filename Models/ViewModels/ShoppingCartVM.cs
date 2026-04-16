using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BooksSpring26.Models.ViewModels
{
    public class ShoppingCartVM
    {
        [ValidateNever]
        public IEnumerable<CartItem> CartItems { get; set; }  //initialize the CartItems property with a new list of CartItem objects
         public Order Order { get; set; }
    }
}
