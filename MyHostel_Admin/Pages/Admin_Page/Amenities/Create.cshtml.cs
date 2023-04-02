using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyHostel_Admin.Models;
using System;

namespace MyHostel_Admin.Pages.Admin_Page.Amenities
{
    public class CreateModel : PageModel
    {
        private readonly MyHostelContext context;

        public CreateModel(MyHostelContext _context)
        {
            context = _context;
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("currentUser") == null)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                String currentUser = HttpContext.Session.GetString("currentUser");
                Admin user = context.Admins.SingleOrDefault(p => p.AccountName.ToLower().Equals(currentUser.ToLower()));
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
            return Page();
        }

        [BindProperty]
        public Amenity Amenity { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["Alert"] = "Create Fail!";
                return Page();
            }

            context.Amenities.Add(Amenity);
            await context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
