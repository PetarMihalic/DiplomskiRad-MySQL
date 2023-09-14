namespace SneakerShopMySQL.Models
{
    public class Cart
    {
        public int ID { get; set; }
        public int InventoryID { get; set; }
        public int? UserID { get; set; }
        public string? SessionID { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public Inventory Inventory { get; set; }
        public User user { get; set; }
    }
}
