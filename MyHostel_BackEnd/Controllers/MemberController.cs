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
                                         orderby c.LastMsgAt descending
                                         select c;
                PaginatedList<Chat> chatsPL = await PaginatedList<Chat>.CreateAsync(chats.AsNoTracking(), (int)page, (int)limit);
                var result = new GetListChatDTO { MemberId = id,
                                                  Chats = new List<object>()
                                                        };

                foreach (var chat in chatsPL)
                {
                    var messages = await _context.Messages.Where(m => m.ChatId == chat.Id).Include(c => c.Sender).ToListAsync();
                    if (chat.IsGroup == 0)
                    {
                        SingleChatMessageDTO lastMessage = new SingleChatMessageDTO();
                        var lastMessageWithImage = new object();
                        var otherparticipant = _context.Participants.Where(p => p.ChatId == chat.Id && p.MemberId != id).SingleOrDefault();
                        var otherMember = _context.Members.Where(m => m.Id == otherparticipant.MemberId).SingleOrDefault();
                        if (messages.Any())
                        {
                            var lastMsg = messages.OrderBy(m => m.CreatedAt).Last();
                            
                            if (lastMsg != null)
                            {
                                var imgNo = _context.MessageImages.Where(m => m.MessageId == lastMsg.Id).Count();
                                if (lastMsg.MsgText != null)
                                {
                                    lastMessage.AnonymousFlg = lastMsg.AnonymousFlg;
                                    lastMessage.MemberId = lastMsg.SenderId;
                                    lastMessage.AvatarUrl = lastMsg.Sender.Avatar;
                                    lastMessage.MsgText = lastMsg.MsgText;
                                    lastMessage.CreatedAt = lastMsg.CreatedAt.ToString("dd/MM/yyyy hh:mm");
                                    result.Chats.Add(new SingleChatDTO()
                                    {
                                        ChatId = chat.Id,
                                        Name = otherMember.FirstName + " " + otherMember.LastName,
                                        IsGroup = 0,
                                        LastMsg = lastMessage,
                                        Participant = new SingleChatParicipantDTO
                                        {
                                            MemberId = otherMember.Id,
                                            AvatarUrl = otherMember.Avatar,
                                            Name = otherMember.FirstName + " " + otherMember.LastName

                                        }
                                    });
                                }
                                else
                                {
                                    lastMessageWithImage = new
                                    {
                                    AnonymousFlg = lastMsg.AnonymousFlg,
                                    MemberId = lastMsg.SenderId,
                                    AvatarUrl = lastMsg.Sender.Avatar,
                                    MsgText = lastMsg.MsgText,
                                    CreatedAt = lastMsg.CreatedAt.ToString("dd/MM/yyyy hh:mm"),
                                    ImgNo = imgNo
                                    };
                                    result.Chats.Add(new 
                                    {
                                        ChatId = chat.Id,
                                        Name = otherMember.FirstName + " " + otherMember.LastName,
                                        IsGroup = 0,
                                        LastMsg =  lastMessageWithImage,
                                        Participant = new SingleChatParicipantDTO
                                        {
                                            MemberId = otherMember.Id,
                                            AvatarUrl = otherMember.Avatar,
                                            Name = otherMember.FirstName + " " + otherMember.LastName

                                        }
                                    });
                                }
                            }
                        }
                        else
                        {
                            result.Chats.Add(new SingleChatDTO()
                            {
                                ChatId = chat.Id,
                                Name = otherMember.FirstName + " " + otherMember.LastName,
                                IsGroup = 0,
                                LastMsg = null,
                                Participant = new SingleChatParicipantDTO
                                {
                                    MemberId = otherMember.Id,
                                    AvatarUrl = otherMember.Avatar,
                                    Name = otherMember.FirstName + " " + otherMember.LastName
                                }
                            });
                        }
                        
                    }
                    else
                    {
                        GroupChatMessageDTO lastMessage = new GroupChatMessageDTO();
                        var lastMessageWithImage = new object();
                        if (messages.Any())
                        {
                            var lastMsg = messages.OrderBy(m => m.CreatedAt).Last();
                            if (lastMsg != null)
                            {
                                var imgNo = _context.MessageImages.Where(m => m.MessageId == lastMsg.Id).Count();
                                if (lastMsg.MsgText != null)
                                {
                                    lastMessage.AnonymousFlg = lastMsg.AnonymousFlg;
                                    lastMessage.MsgText = lastMsg.MsgText;
                                    lastMessage.CreatedAt = lastMsg.CreatedAt.ToString("dd/MM/yyyy hh:mm");
                                    result.Chats.Add(new GroupChatDTO()
                                    {
                                        ChatId = chat.Id,
                                        Name = chat.Name,
                                        Avatar = chat.AvatarUrl,
                                        IsGroup = 1,
                                        LastMsg =  lastMessage
                                    });
                                }
                                else
                                {
                                    lastMessageWithImage = new
                                    {
                                        AnonymousFlg = lastMsg.AnonymousFlg,
                                        MsgText = lastMsg.MsgText,
                                        CreatedAt = lastMsg.CreatedAt.ToString("dd/MM/yyyy hh:mm"),
                                        ImgNo = imgNo
                                    };
                                    result.Chats.Add(new
                                    {
                                        ChatId = chat.Id,
                                        Name = chat.Name,
                                        IsGroup = 1,
                                        LastMsg = lastMessageWithImage,                                    
                                    });
                                }

                            }
                        }
                        else
                        {
                            result.Chats.Add(new GroupChatDTO()
                            {
                                ChatId = chat.Id,
                                Name = chat.Name,
                                Avatar = chat.AvatarUrl,
                                IsGroup = 1,
                                LastMsg = null
                            });
                        }
                        
                    }
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
    }
}
