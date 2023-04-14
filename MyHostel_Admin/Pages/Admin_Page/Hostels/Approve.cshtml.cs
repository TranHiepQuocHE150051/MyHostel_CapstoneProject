using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyHostel_Admin.Models;

namespace MyHostel_Admin.Pages.Admin_Page.Hostels
{
    public class ApproveModel : PageModel
    {
        private readonly MyHostelContext _context;

        public ApproveModel(MyHostelContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Hostel Hostel { get; set; }

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

            Hostel = await _context.Hostels.FirstOrDefaultAsync(m => m.Id == id);

            if (Hostel == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            Hostel = await _context.Hostels.FindAsync(id);

            if (Hostel != null)
            {
                Hostel.Status = 1;
                _context.Hostels.Update(Hostel);
                await _context.SaveChangesAsync();
                Chat chat = new Chat
                {
                    HostelId = id,
                    Name = Hostel.Name+" group chat",
                    CreatedAt = DateTime.Now,
                    IsGroup=1

                };
                _context.Chats.Add(chat);
                _context.SaveChanges();
                var member = _context.Members.Where(m => m.Id == Hostel.LandlordId).SingleOrDefault();
                Participant lanlord = new Participant
                {
                    ChatId = chat.Id,
                    MemberId = Hostel.LandlordId,
                    JoinedAt = DateTime.Now,
                    Role = 1,
                    AnonymousTime = 3,
                    NickName = member.FirstName + " " + member.LastName
                };
                _context.Participants.Add(lanlord);
                _context.SaveChanges();
            }

            return RedirectToPage("/Admin_Page/Hostels/Index");
        }
    }
}
