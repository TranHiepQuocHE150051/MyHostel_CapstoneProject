using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyHostel_Admin.Models;

namespace MyHostel_Admin.Pages.Admin_Page.HostelAmenities
{
    public class IndexModel : PageModel
    {
        private readonly MyHostelContext context;
        private readonly IConfiguration Configuration;
        public IndexModel(MyHostelContext _context, IConfiguration configuration)
        {
            context = _context;
            Configuration = configuration;
        }
        public PaginatedList<HostelAmenity> HostelAmenity { get; set; }
        public string CurrentFilter { get; set; }
        public async Task<IActionResult> OnGetAsync(string currentFilter, string searchString, int? pageIndex)
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
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;
            IQueryable<HostelAmenity> genresIQ = from s in context.HostelAmenities
                                                 .Include(p => p.Amenities)
                                                 .Include(p => p.Hosted)
                                                 select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                genresIQ = genresIQ.Where(s => s.Amenities.AmenitiyName.ToLower().Contains(searchString.ToLower())
                || s.Hosted.Name.ToLower().Contains(searchString.ToLower()));
            }
            var pageSize = Configuration.GetValue("PageSize", 2);
            HostelAmenity = await PaginatedList<HostelAmenity>.CreateAsync(genresIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
            return Page();
        }
    }
}
