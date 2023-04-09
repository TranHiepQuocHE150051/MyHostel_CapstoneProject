using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MyHostel_Admin.Models;

namespace MyHostel_Admin.Pages.Admin_Page.Hostels
{
    public class ResidentsModel : PageModel
    {
        private readonly MyHostelContext context;
        private readonly IConfiguration Configuration;
        public ResidentsModel(MyHostelContext _context, IConfiguration configuration)
        {
            context = _context;
            Configuration = configuration;
        }
        public PaginatedList<Resident> Resident { get; set; }
        public Hostel hostel { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id, int? pageIndex)
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
            hostel = context.Hostels.Where(h => h.Id == id).SingleOrDefault();
            IQueryable<Resident> personsIQ;

            personsIQ = from p in context.Residents
                        .Include(p => p.Member)
                        .Include(p => p.Room)
                        where p.HostelId == id && p.Status==1
                        select p;

            var pageSize = Configuration.GetValue("PageSize", 4);
            Resident = await PaginatedList<Resident>.CreateAsync(personsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
            return Page();
        }
    }
}