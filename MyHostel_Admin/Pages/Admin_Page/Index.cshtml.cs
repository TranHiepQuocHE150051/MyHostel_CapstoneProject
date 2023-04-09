using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyHostel_Admin.Models;

namespace MyHostel_Admin.Pages.Admin_Page
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
        public int TotalHostel { get; set; }
        public double hostelDiff { get; set; }
        public int TotalUser { get; set; }
        public int userDiff { get; set; }
        public int TotalResidents { get; set; }
        public int  residentDiff { get; set; }
        public int TotalRooms { get; set; }
        
        public void OnGet()
        {
            TotalHostel = context.Hostels.Where(h => h.Status == 1).ToList().Count;
            TotalUser = context.Members.ToList().Count;
            TotalResidents = context.Residents.Where(r => r.Status == 1).ToList().Count;
            TotalRooms = context.Rooms.Include(h=>h.Hostel).Where(r=>r.Hostel.Status==1).ToList().Count;


            int totalhostellastmonth;
            totalhostellastmonth = context.Hostels.Where(h=>h.Status==1&&h.CreatedAt<DateTime.Now.AddDays(-30)).ToList().Count;
            hostelDiff = (((double)TotalHostel- (double)totalhostellastmonth)/ (double)totalhostellastmonth) * 100;
            
            int totaluserlastmonth;
            totaluserlastmonth = context.Members.Where(h =>  h.CreatedAt < DateTime.Now.AddDays(-30)).ToList().Count;
            userDiff = (TotalUser - totaluserlastmonth);
            int totalresidentlastmonth;
            totalresidentlastmonth = context.Residents.Where(h => h.Status == 1 && h.CreatedAt < DateTime.Now.AddDays(-30)).ToList().Count;
            residentDiff = (TotalResidents - totalresidentlastmonth);
            
        }
    }
}
