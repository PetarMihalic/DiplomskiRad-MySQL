using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopMySQL.Data;
using SneakerShopMySQL.Models;
using System.Diagnostics;

namespace SneakerShopMySQL.Pages.Sneakers
{
    public class DetailsModel : PageModel
    {
        private readonly SneakerShopContext _context;
        private readonly ILogger<DetailsModel> _logger;
        public string errorMessage = "";
        public string successMessage = "";

        public DetailsModel(SneakerShopContext context, ILogger<DetailsModel> logger)
        {
            _context = context;
            _logger = logger;
        }
        [BindProperty(SupportsGet = true)]
        public Sneaker Sneaker { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public List<Inventory> Inventories { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Sneakers == null)
            {
                return NotFound();
            }

            var sneaker = await _context.Sneakers.AsNoTracking().FirstOrDefaultAsync(m => m.ID == id);
            var inventory = await _context.Inventory.Include(i => i.Sneaker).Where(m => m.SneakerID == id && m.Quantity > 0).ToListAsync();
            if (sneaker == null && inventory == null)
            {
                return NotFound();
            }
            else
            {
                Inventories = inventory;
                Sneaker = sneaker;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Cart Cart = new Cart();

            int sneakerID = int.Parse(Request.Form["ID"]);
            float size = float.Parse(Request.Form["size"]);
            int quantity = int.Parse(Request.Form["quantity"]);

            var sneakerOld = await _context.Sneakers.AsNoTracking().FirstOrDefaultAsync(m => m.ID == sneakerID);
            var inventoryOld = await _context.Inventory.Include(i => i.Sneaker).Where(m => m.SneakerID == sneakerID).ToListAsync();
            if (sneakerOld == null && inventoryOld == null)
            {
                return NotFound();
            }
            else
            {
                Inventories = inventoryOld;
                Sneaker = sneakerOld;
            }

            var inventory = await _context.Inventory.AsNoTracking().Where(m => m.SneakerID == sneakerID).Where(m => m.Size == size).FirstOrDefaultAsync();

            if (inventory.Quantity < quantity)
            {
                errorMessage = "Only " + inventory.Quantity + " available, lower quantity to order.";
                stopwatch.Stop();
                _logger.LogInformation("Cart Create (error) Time: {0}", stopwatch.ElapsedMilliseconds);
                return Page();
            }

            Cart.InventoryID = inventory.ID;
            Cart.Quantity = quantity;
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Name")))
            {
                Cart.SessionID = HttpContext.Session.Id;
            }
            else
            {
                Cart.UserID = int.Parse(HttpContext.Session.GetString("UserID"));
            }

            _context.Carts.Add(Cart);
            await _context.SaveChangesAsync();

            if (errorMessage == "")
            {
                int currentQuantity = int.Parse(HttpContext.Session.GetString("Cart"));
                int newQuantity = currentQuantity + quantity;
                HttpContext.Session.SetString("Cart", newQuantity.ToString());
                successMessage = "Added to cart";
            }
            stopwatch.Stop();
            _logger.LogInformation("Cart Create Time: {0}", stopwatch.ElapsedMilliseconds);
            return Page();
        }
    }
}
