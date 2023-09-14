using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopMySQL.Data;
using System.Diagnostics;

namespace SneakerShopMySQL.Pages.Carts
{
    public class IndexModel : PageModel
    {
        private readonly SneakerShopContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(SneakerShopContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<CartPreview> listCart { get; set; } = default!;
        public float TotalCost = 0;
        public async Task OnGetAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (_context.Carts != null)
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("Name")))
                {
                    listCart = (from inv in _context.Inventory
                                join sne in _context.Sneakers on inv.SneakerID equals sne.ID
                                join car in _context.Carts on inv.ID equals car.InventoryID
                                where car.SessionID == HttpContext.Session.Id
                                select new CartPreview
                                {
                                    cartID = car.ID,
                                    picture1 = "data:image;base64," + Convert.ToBase64String(sne.Picture1),
                                    name = sne.Name,
                                    size = inv.Size,
                                    quantity = car.Quantity,
                                    price = sne.Price,
                                    total = (float)(car.Quantity * sne.Price)
                                }).ToList();
                }
                else
                {
                    listCart = (from inv in _context.Inventory
                                join sne in _context.Sneakers on inv.SneakerID equals sne.ID
                                join car in _context.Carts on inv.ID equals car.InventoryID
                                where car.UserID == int.Parse(HttpContext.Session.GetString("UserID"))
                                select new CartPreview
                                {
                                    cartID = car.ID,
                                    picture1 = "data:image;base64," + Convert.ToBase64String(sne.Picture1),
                                    name = sne.Name,
                                    size = inv.Size,
                                    quantity = car.Quantity,
                                    price = sne.Price,
                                    total = (float)(car.Quantity * sne.Price)
                                }).ToList();
                }
                foreach (var item in listCart)
                {
                    TotalCost += item.total;
                }
                TotalCost = (float)Math.Round(TotalCost, 2);
                stopwatch.Stop();
                _logger.LogInformation("Cart Index Time: {0}", stopwatch.ElapsedMilliseconds);
            }
        }

        public class CartPreview
        {
            public int cartID { get; set; }
            public string picture1 { get; set; }
            public string name { get; set; }
            public float size { get; set; }
            public int quantity { get; set; }
            public decimal price { get; set; }
            public float total { get; set; }
            public int inventoryID { get; set; }
        }
    }
}
