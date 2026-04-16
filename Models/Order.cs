using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BooksSpring26.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        [ValidateNever]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; } //navigational property
        

        //we are adding bv this may be shipping address
        public string CustomerName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }

        public decimal OrderTotal { get; set; }
        [ValidateNever]
        public string OrderStatus { get; set; }
        [ValidateNever]
        public string PaymentStatus { get; set; }
        [ValidateNever]
        public DateOnly OrderDate { get; set; }

        public string? Carrier { get; set; }
        public string? TrackingNumber { get; set; }
        public DateOnly? ShippingDate { get; set; }


    }
}
