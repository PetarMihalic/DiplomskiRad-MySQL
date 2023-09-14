using System.ComponentModel.DataAnnotations;

namespace SneakerShopMySQL.Models
{
    public class Order
    {
        public int ID { get; set; }
        [DisplayFormat(NullDisplayText = "Guest")]
        public int? UserID { get; set; }
        public string Name { get; set; }
        [Display(Name = "Created At")]
        public DateTime CreatedDate { get; set; }
        [Required]
        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; } = string.Empty;
        public string Status { get; set; } = "pending";
        public User User { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
