using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHostel_BackEnd.DTOs;
using MyHostel_BackEnd.Models;

namespace MyHostel_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private IConfiguration _configuration;
        private MyHostelContext _context;
        public ChatController(IConfiguration configuration, MyHostelContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpPost("single")]
        public async Task<IActionResult> CreateSingleChat([FromBody] CreateSingleChatDTO participants)
        {
            if (participants.Participant.Count != 2)
            {
                return BadRequest("Cannot create single chat with less or more than 2 people");
            }
            Chat chat = new Chat
            {
                CreatedAt = DateTime.Now,
                IsGroup= 0

            };
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            foreach (var participant in participants.Participant)
            {
                var member = _context.Members.Where(m=>m.Id== participant.MemberId).SingleOrDefault();
                if(member == null)
                {
                    return BadRequest("Member not exist");
                }
                string nickname = "";
                if (participant.NickName == null)
                {
                    nickname = member.FirstName + " " + member.LastName;
                }
                else
                {
                     nickname = participant.NickName;
                }
                Participant participant1 = new Participant
                {
                    ChatId = chat.Id,
                    MemberId = member.Id,
                    JoinedAt = DateTime.Now,
                    Role = (byte)member.RoleId,
                    AnonymousTime = 3,
                    NickName = nickname
                };
                _context.Participants.Add(participant1);
            }
            await _context.SaveChangesAsync();
            return Ok("Create chat successfully");
        }
        [HttpPost("group")]
        public async Task<IActionResult> CreateGroupChat([FromBody] CreateGroupChatDTO groupChat)
        {
            var hostel = _context.Hostels.Where(h => h.Id == groupChat.HostelId).SingleOrDefault();
            if (hostel == null)
            {
                return BadRequest("Hostel not exist");
            }
            Chat chat = new Chat
            {
                HostelId = hostel.Id,
                Name = groupChat.Name,
                CreatedAt = DateTime.Now,
                IsGroup = 1,
                AvatarUrl= groupChat.AvatarUrl

            };
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            return Ok("create group chat successfully");
        }
        [HttpPost("message")]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDTO message)
        {
            var member = _context.Members.Where(m => m.Id == message.MemberId).SingleOrDefault();
            if (member == null)
            {
                return BadRequest("Member is not exist");
            }
            Chat chat = _context.Chats.Where(c=>c.Id == message.ChatId).SingleOrDefault();
            if (chat == null)
            {
                return BadRequest("Chat is not exist");
            }
            var participants = _context.Participants.Where(p => p.ChatId == message.ChatId).ToList();
            bool IsInChat = false;
            foreach(var participant in participants)
            {
                if(member.Id== participant.MemberId)
                {
                    IsInChat = true;
                    break;
                }
            }
            if (!IsInChat)
            {
                return BadRequest("Member is not in chat");
            }
            if(chat.LastMsgAt == null)
            {
                Message message1 = new Message
                {
                    ChatId = message.ChatId,
                    SenderId = message.MemberId,
                    MsgText = message.MsgText,
                    CreatedAt = DateTime.Now,
                    AnonymousFlg = (byte)message.AnonymousFlg
                };
                _context.Messages.Add(message1);
                await _context.SaveChangesAsync();
                return Ok("Create message success");
            }
            else
            {
                Message message1 = new Message
                {
                    ChatId = message.ChatId,
                    SenderId = message.MemberId,
                    ParentMsgId = message.ParentMsgId,
                    MsgText = message.MsgText,
                    CreatedAt = DateTime.Now,
                    AnonymousFlg = (byte)message.AnonymousFlg
                };
                _context.Messages.Add(message1);
                await _context.SaveChangesAsync();
                return Ok("Create message success");
            }
        }
    }
}
