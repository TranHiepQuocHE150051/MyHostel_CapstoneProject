using GoogleApi.Entities.Search.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHostel_BackEnd.DTOs;
using MyHostel_BackEnd.Models;

namespace MyHostel_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private IConfiguration _configuration;
        private MyHostelContext _context;
        public MemberController(IConfiguration configuration, MyHostelContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpGet("{id}/chats")]
        public async Task<IActionResult> GetListChat(int id, [FromQuery] int? page, [FromQuery] int? limit)
        {
            try
            {
                var member = await _context.Members.Where(m => m.Id == id).FirstOrDefaultAsync();
                if (member == null)
                {
                    return BadRequest("Member not exist");
                }
                if (page == null || page <= 0)
                {
                    page = 1;
                }
                if (limit == null || limit <= 0)
                {
                    limit = 10;
                }
                IQueryable<Chat> chats = from c in _context.Chats 
                                         join p in _context.Participants 
                                         on c.Id equals p.ChatId
                                         where p.MemberId == id
                                         select c;
                PaginatedList<Chat> chatsPL = await PaginatedList<Chat>.CreateAsync(chats.AsNoTracking(), (int)page, (int)limit);
                var result = new GetListChatDTO { MemberId = id };
                List<ChatDTO> chatList = new List<ChatDTO>();
                foreach (var chat in chatsPL)
                {
                    var messages = await _context.Messages.Where(m => m.ChatId == chat.Id).Include(c => c.Sender).ToListAsync();
                    MessageDTO lastMessage = new MessageDTO();
                    if (messages.Any())
                    {
                        var lastMsg = messages.OrderBy(m => m.CreatedAt).Last();
                        if (lastMsg != null)
                        {
                            lastMessage.AnonymousFlg = lastMsg.AnonymousFlg;
                            lastMessage.MemberId = lastMsg.SenderId;
                            lastMessage.AvatarUrl = lastMsg.Sender.Avatar;
                            lastMessage.MsgText = lastMsg.MsgText;
                            lastMessage.CreatedAt = lastMsg.CreatedAt.ToString("dd/MM/yyyy hh:mm");
                        }
                    }
                    chatList.Add(new ChatDTO()
                    {
                        ChatId = chat.Id,
                        Name = chat.Name,
                        IsGroup = chat.IsGroup,
                        Avatar = chat.AvatarUrl,
                        lastMSG = messages.Any() ? lastMessage : null
                    });
                }
                result.Chats = chatList.ToArray();
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
