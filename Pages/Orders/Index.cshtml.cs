using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopMySQL.Data;
using SneakerShopMySQL.Models;
using System.Diagnostics;

namespace SneakerShopMySQL.Pages.Orders
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

        public IList<Order> Order { get; set; } = default!;

        [BindProperty]
        public int OrderID { get; set; }
        [BindProperty]
        public string Status { get; set; }

        public async Task OnGetAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (_context.Orders != null)
            {
                Order = await _context.Orders
                .Include(o => o.User).ToListAsync();
            }
            stopwatch.Stop();
            _logger.LogInformation("Order Index Time: {0}", stopwatch.ElapsedMilliseconds);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Order order = await _context.Orders.FirstOrDefaultAsync(or => or.ID == OrderID);
            order.Status = Status;

            _context.Attach(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            stopwatch.Stop();
            _logger.LogInformation("Order Edit Time: {0}", stopwatch.ElapsedMilliseconds);
            return RedirectToPage("/Orders/Index");
        }
    }
}
