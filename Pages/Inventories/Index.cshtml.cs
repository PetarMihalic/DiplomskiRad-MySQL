using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopMySQL.Data;
using SneakerShopMySQL.Models;
using System.Diagnostics;

namespace SneakerShopMySQL.Pages.Inventories
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

        public IList<Inventory> Inventory { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (_context.Inventory != null)
            {
                Inventory = await _context.Inventory
                .Include(i => i.Sneaker).ToListAsync();
            }
            stopwatch.Stop();
            _logger.LogInformation("Inventory Index Time: {0}", stopwatch.ElapsedMilliseconds);
        }
    }
}
