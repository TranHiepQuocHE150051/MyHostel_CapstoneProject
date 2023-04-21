using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyHostel_Admin.Models;

namespace MyHostel_Admin.Pages.Admin_Page.Hostels
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
        public PaginatedList<Hostel> Hostel { get; set; }
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
            IQueryable<Hostel> personsIQ;

            personsIQ = from p in context.Hostels
                        .Include(p => p.WardsCodeNavigation)
                        .Include(p => p.Landlord)
                        select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                personsIQ = personsIQ.Where(p => p.Name.ToLower().Contains(searchString.ToLower())
                || p.DetailLocation.ToLower().Contains(searchString.ToLower()));
            }
            var pageSize = Configuration.GetValue("PageSize", 6);
            Hostel = await PaginatedList<Hostel>.CreateAsync(personsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
            return Page();
        }
        public ActionResult OnPost()
        {
            var hostels = context.Hostels.Include(h=>h.WardsCodeNavigation).ToList();
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "hostels.xlsx";
            using(var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("Hostels");
                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Ward";
                worksheet.Cell(1, 4).Value = "Price";
                worksheet.Cell(1, 5).Value = "Capacity";
                worksheet.Cell(1, 6).Value = "Phone";
                worksheet.Cell(1, 7).Value = "RoomArea";
                worksheet.Cell(1, 8).Value = "Location";
                worksheet.Cell(1, 9).Value = "Electricity";
                worksheet.Cell(1, 10).Value = "Water";
                worksheet.Cell(1, 11).Value = "Internet";
                worksheet.Cell(1, 12).Value = "Description";
                worksheet.Cell(1, 13).Value = "Status";
                for (int i = 1; i <= hostels.Count; i++)
                {
                    worksheet.Cell(i + 1,1).Value = hostels[i-1].Id;
                    worksheet.Cell(i + 1,2).Value = hostels[i-1].Name;
                    worksheet.Cell(i + 1,3).Value = hostels[i-1].WardsCodeNavigation.FullName;
                    worksheet.Cell(i + 1,4).Value = hostels[i-1].Price;
                    worksheet.Cell(i + 1,5).Value = hostels[i-1].Capacity.Trim();
                    worksheet.Cell(i + 1,6).Value = hostels[i-1].Phone;
                    worksheet.Cell(i + 1,7).Value = hostels[i-1].RoomArea.Trim();
                    worksheet.Cell(i + 1,8).Value = hostels[i-1].DetailLocation;
                    worksheet.Cell(i + 1,9).Value = hostels[i-1].Electricity;
                    worksheet.Cell(i + 1, 10).Value = hostels[i - 1].Water;
                    worksheet.Cell(i + 1,11).Value = hostels[i-1].Internet;
                    worksheet.Cell(i + 1,12).Value = hostels[i-1].Description;
                    if (hostels[i-1].Status == 0)
                    {
                        worksheet.Cell(i + 1, 13).Value = "Pending";
                    }
                    if (hostels[i - 1].Status == 1)
                    {
                        worksheet.Cell(i + 1, 13).Value = "Approved";
                    }
                    if (hostels[i - 1].Status == 2)
                    {
                        worksheet.Cell(i + 1, 13).Value = "Rejected";
                    }
                    if (hostels[i - 1].Status == 3)
                    {
                        worksheet.Cell(i + 1, 13).Value = "Deleted";
                    }

                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
            
        }
    }
}