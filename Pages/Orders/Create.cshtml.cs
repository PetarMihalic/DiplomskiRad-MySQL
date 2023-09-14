using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopMySQL.Data;
using SneakerShopMySQL.Models;
using System.Diagnostics;
using static SneakerShopMySQL.Pages.Carts.IndexModel;

namespace SneakerShopMySQL.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly SneakerShopContext _context;
        private readonly ILogger<CreateModel> _logger;
        public CreateModel(SneakerShopContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public User User { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string PaymentType { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public Order Order { get; set; } = default!;

        public List<CartPreview> listCart { get; set; } = default!;
        public float TotalCost = 0;
        public async Task OnGetAsync()
        {
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
                                    inventoryID = inv.ID,
                                    total = (float)(car.Quantity * sne.Price)
                                }).ToList();
                }
                else
                {
                    User = await _context.Users.FirstOrDefaultAsync(u => u.ID == int.Parse(HttpContext.Session.GetString("UserID")));

                    listCart = (from inv in _context.Inventory
                                join sne in _context.Sneakers on inv.SneakerID equals sne.ID
                                join car in _context.Carts on inv.ID equals car.InventoryID
                                where car.UserID == User.ID
                                select new CartPreview
                                {
                                    cartID = car.ID,
                                    picture1 = "data:image;base64," + Convert.ToBase64String(sne.Picture1),
                                    name = sne.Name,
                                    size = inv.Size,
                                    quantity = car.Quantity,
                                    price = sne.Price,
                                    inventoryID = inv.ID,
                                    total = (float)(car.Quantity * sne.Price)
                                }).ToList();
                }
                foreach (var item in listCart)
                {
                    TotalCost += item.total;
                }
                TotalCost = (float)Math.Round(TotalCost, 2);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Order Order = new Order();
            if (User.ID > 0) Order.UserID = User.ID;
            Random random = new Random();
            Order.Name = "ORDER-" + DateTime.Now.ToString() + "-" + random.Next(100, 1000);
            Order.PaymentType = PaymentType;
            Order.CreatedDate = DateTime.Now;
            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();

            if (User.ID == 0)
            {
                listCart = (from inv in _context.Inventory
                            join sne in _context.Sneakers on inv.SneakerID equals sne.ID
                            join car in _context.Carts on inv.ID equals car.InventoryID
                            where car.SessionID == HttpContext.Session.Id
                            select new CartPreview
                            {
                                quantity = car.Quantity,
                                inventoryID = inv.ID
                            }).ToList();
            }
            else
            {
                listCart = (from inv in _context.Inventory
                            join sne in _context.Sneakers on inv.SneakerID equals sne.ID
                            join car in _context.Carts on inv.ID equals car.InventoryID
                            where car.UserID == User.ID
                            select new CartPreview
                            {
                                quantity = car.Quantity,
                                inventoryID = inv.ID
                            }).ToList();
            }

            foreach (CartPreview cartPreview in listCart)
            {
                OrderDetails OrderDetails = new OrderDetails();
                OrderDetails.OrderID = Order.ID;
                OrderDetails.InventoryID = cartPreview.inventoryID;
                OrderDetails.Quantity = cartPreview.quantity;
                Inventory inventory = await _context.Inventory.FirstOrDefaultAsync(x => x.ID == cartPreview.inventoryID);
                inventory.Quantity = inventory.Quantity - cartPreview.quantity;
                if (inventory.Quantity == 0)
                {
                    var cartItems = await _context.Carts.Where(c => c.InventoryID == inventory.ID).ToListAsync();
                    _context.Carts.RemoveRange(cartItems);
                }
                _context.Attach(inventory).State = EntityState.Modified;
                _context.OrderDetails.Add(OrderDetails);
                await _context.SaveChangesAsync();
            }

            if (User.ID > 0)
            {
                var cartItems = await _context.Carts.Where(c => c.UserID == User.ID).ToListAsync();
                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("Cart", "0");
            }
            else
            {
                var cartItems = await _context.Carts.Where(c => c.SessionID == HttpContext.Session.Id).ToListAsync();
                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("Cart", "0");
            }
            stopwatch.Stop();
            _logger.LogInformation("Order Create Time: {0}", stopwatch.ElapsedMilliseconds);
            return RedirectToPage("/Index");
        }
    }
}
