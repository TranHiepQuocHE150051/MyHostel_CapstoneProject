using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyHostel_Admin.Models;
using System;

namespace MyHostel_Admin.Pages.Admin_Page.Members
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
        public PaginatedList<Member> Member{ get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentType { get; set; }
        public async Task<IActionResult> OnGetAsync(string currentFilter, string currentType, string searchString, int? pageIndex)
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
            if (currentType == null) { currentType = "0"; }
            CurrentType = currentType;
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;
            IQueryable<Member> personsIQ;

                personsIQ = from p in context.Members
                            select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                personsIQ = personsIQ.Where(p => p.LastName.ToLower().Contains(searchString.ToLower())
                || p.FirstName.ToLower().Contains(searchString.ToLower()));
            }
            var pageSize = Configuration.GetValue("PageSize", 4);
            Member = await PaginatedList<Member>.CreateAsync(personsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
            return Page();
        }
    }
}