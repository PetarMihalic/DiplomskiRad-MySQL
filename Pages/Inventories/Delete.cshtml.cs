using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopMySQL.Data;
using SneakerShopMySQL.Models;
using System.Diagnostics;

namespace SneakerShopMySQL.Pages.Inventories
{
    public class DeleteModel : PageModel
    {
        private readonly SneakerShopContext _context;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(SneakerShopContext context, ILogger<DeleteModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Inventory Inventory { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Inventory == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventory.Include(i => i.Sneaker).FirstOrDefaultAsync(m => m.ID == id);

            if (inventory == null)
            {
                return NotFound();
            }
            else
            {
                Inventory = inventory;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Inventory == null)
            {
                return NotFound();
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var inventory = await _context.Inventory.FindAsync(id);

            if (inventory != null)
            {
                Inventory = inventory;
                _context.Inventory.Remove(Inventory);
                await _context.SaveChangesAsync();
            }
            stopwatch.Stop();
            _logger.LogInformation("Inventory Delete Time: {0}", stopwatch.ElapsedMilliseconds);
            return RedirectToPage("./Index");
        }
    }
}
