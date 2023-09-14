using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopMySQL.Data;
using SneakerShopMySQL.Models;
using System.Diagnostics;

namespace SneakerShopMySQL.Pages.Sneakers
{
    public class EditModel : PageModel
    {
        private readonly SneakerShopContext _context;
        private readonly ILogger<EditModel> _logger;
        public IFormFile? imageFile1;
        public IFormFile? imageFile2;
        public EditModel(SneakerShopContext context, ILogger<EditModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Sneaker Sneaker { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Sneakers == null)
            {
                return NotFound();
            }

            var sneaker = await _context.Sneakers.FirstOrDefaultAsync(m => m.ID == id);
            if (sneaker == null)
            {
                return NotFound();
            }
            Sneaker = sneaker;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var sneaker = await _context.Sneakers.AsNoTracking().FirstOrDefaultAsync(m => m.ID == Sneaker.ID);

            if (Request.Form.Files.GetFile("picture1") == null)
            {
                Sneaker.Picture1 = sneaker.Picture1;
            }
            else
            {
                imageFile1 = Request.Form.Files.GetFile("picture1");
                MemoryStream dataStream = new MemoryStream();
                await imageFile1.CopyToAsync(dataStream);
                Sneaker.Picture1 = dataStream.ToArray();
            }
            if (Request.Form.Files.GetFile("picture2") == null)
            {
                Sneaker.Picture1 = sneaker.Picture1;
            }
            else
            {
                imageFile2 = Request.Form.Files.GetFile("picture2");
                MemoryStream dataStream = new MemoryStream();
                await imageFile2.CopyToAsync(dataStream);
                Sneaker.Picture2 = dataStream.ToArray();
            }

            _context.Attach(Sneaker).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SneakerExists(Sneaker.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            stopwatch.Stop();
            _logger.LogInformation("Sneaker Edit Time: {0}", stopwatch.ElapsedMilliseconds);
            return RedirectToPage("/Index");
        }

        private bool SneakerExists(int id)
        {
            return (_context.Sneakers?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
