using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MyHostel_Admin.Models;

namespace MyHostel_Admin.Pages.Admin_Page.Hostels
{
    public class RoomsModel : PageModel
    {
        private readonly MyHostelContext context;
        private readonly IConfiguration Configuration;
        public RoomsModel(MyHostelContext _context, IConfiguration configuration)
        {
            context = _context;
            Configuration = configuration;
        }
        public PaginatedList<Room> Room { get; set; }
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
            IQueryable<Room> personsIQ;

            personsIQ = from p in context.Rooms
                        where p.HostelId == id
                        select p;

            var pageSize = Configuration.GetValue("PageSize", 6);
            Room = await PaginatedList<Room>.CreateAsync(personsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
            return Page();
        }
    }
}