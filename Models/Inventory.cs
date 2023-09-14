using Microsoft.EntityFrameworkCore.Migrations;

namespace SneakerShopMySQL.Models
{
    public class Inventory
    {
        public int ID { get; set; }
        public int SneakerID { get; set; }
        public float Size { get; set; }
        public int Quantity { get; set; }
        public Sneaker Sneaker { get; set; }

    }
}
