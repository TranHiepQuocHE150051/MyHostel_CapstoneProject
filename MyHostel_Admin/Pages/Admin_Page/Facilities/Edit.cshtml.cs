using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyHostel_Admin.Models;

namespace MyHostel_Admin.Pages.Admin_Page.Facilities
{
    public class EditModel : PageModel
    {
        private readonly MyHostelContext _context;

        public EditModel(MyHostelContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Facility Facility { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (HttpContext.Session.GetString("currentUser") == null)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                String currentUser = HttpContext.Session.GetString("currentUser");
                Admin user = _context.Admins.SingleOrDefault(p => p.AccountName.ToLower().Equals(currentUser.ToLower()));
                if (user == null)
                {
                    return RedirectToPage("/Index");
                }
                else
                {
                    if (user.RoleId != 3)
                    {
                        return RedirectToPage("/Index");
                    }
                }
            }
            if (id == null)
            {
                return NotFound();
            }

            Facility = await _context.Facilities.FirstOrDefaultAsync(m => m.Id == id);

            if (Facility == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            _context.Attach(Facility).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(Facility.Id))
                {

                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            Facility = await _context.Facilities.FirstOrDefaultAsync(m => m.Id == Facility.Id);
            ViewData["Alert_success"] = "Update Success!";
            return Page();
        }

        private bool GenreExists(int id)
        {
            return _context.Facilities.Any(e => e.Id == id);
        }
    }
}