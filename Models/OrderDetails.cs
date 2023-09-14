using System.ComponentModel.DataAnnotations;

namespace SneakerShopMySQL.Models
{
    public class OrderDetails
    {
        public int ID { get; set; }
        public int InventoryID { get; set; }
        public int OrderID { get; set; }
        public int Quantity { get; set; }
        public Inventory Inventory { get; set; }
    }
}
