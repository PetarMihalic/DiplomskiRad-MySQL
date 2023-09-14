using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopMySQL.Data;
using SneakerShopMySQL.Models;
using System.Diagnostics;

namespace SneakerShopMySQL.Pages.Orders
{
    public class MyOrdersModel : PageModel
    {
        private readonly SneakerShopContext _context;
        private readonly ILogger<MyOrdersModel> _logger;
        public MyOrdersModel(SneakerShopContext context, ILogger<MyOrdersModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<Order> Order { get; set; }

        public IList<OrderDetails> Details { get; set; }

        public async Task OnGetAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int UserID = int.Parse(HttpContext.Session.GetString("UserID"));
            if (_context.Orders != null)
            {
                Order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Inventory)
                .ThenInclude(i => i.Sneaker)
                .Where(o => o.UserID == UserID).ToListAsync();
            }
            stopwatch.Stop();
            _logger.LogInformation("My Orders Time: {0}", stopwatch.ElapsedMilliseconds);
        }
    }
}
