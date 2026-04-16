using System.ComponentModel.DataAnnotations.Schema;

namespace BooksSpring26.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }

        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; } //navigational property

        public string UserId { get; set; }


        [ForeignKey("UserId")]

        public ApplicationUser ApplicationUser { get; set; }
        

        public int Quantity { get; set; }

        [NotMapped] //not mapped to database so this is not a colum more like equation
        public decimal Subtotal { get; set; }
    }
}
