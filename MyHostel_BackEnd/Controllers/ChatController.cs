using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                IsGroup = 0

            };
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            foreach (var participant in participants.Participant)
            {
                var member = _context.Members.Where(m => m.Id == participant.MemberId).SingleOrDefault();
                if (member == null)
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
                AvatarUrl = groupChat.AvatarUrl

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
            Chat chat = _context.Chats.Where(c => c.Id == message.ChatId).SingleOrDefault();
            if (chat == null)
            {
                return BadRequest("Chat is not exist");
            }
            var participants = _context.Participants.Where(p => p.ChatId == message.ChatId).ToList();
            bool IsInChat = false;
            foreach (var participant in participants)
            {
                if (member.Id == participant.MemberId)
                {
                    IsInChat = true;
                    break;
                }
            }
            if (!IsInChat)
            {
                return BadRequest("Member is not in chat");
            }
            if (chat.LastMsgAt == null)
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMessages(int id, [FromQuery] int? page, [FromQuery] int? limit)
        {
            try
            {
                var chat = await _context.Chats.Where(c => c.Id == id).FirstOrDefaultAsync();
                if (chat == null)
                {
                    return BadRequest("Chat not exist");
                }
                if (page == null || page <= 0)
                {
                    page = 1;
                }
                if (limit == null || limit <= 0)
                {
                    limit = 10;
                }
                IQueryable<Message> messages = from c in _context.Messages.Include(m => m.Sender) where c.ChatId == id select c;
                PaginatedList<Message> messagesPL = await PaginatedList<Message>.CreateAsync(messages.AsNoTracking(), (int)page, (int)limit);
                List<MessageDTO> result = new List<MessageDTO>();
                foreach (var message in messagesPL)
                {
                    ParentMessageDTO parentMessage = new ParentMessageDTO();
                    MemberInMessageDTO memberInMessage = new MemberInMessageDTO();
                    if(message.ParentMsgId != null)
                    {
                        var parentMsg = await _context.Messages.Where(m => m.Id == message.ParentMsgId).FirstOrDefaultAsync();
                        if(parentMsg != null)
                        {
                            parentMessage.Id = parentMsg.Id;
                            parentMessage.MsgText = parentMsg.MsgText;
                        }
                    }
                    if(message.AnonymousFlg == 0)
                    {
                        memberInMessage.Id = message.SenderId;
                        memberInMessage.Avatar = message.Sender.Avatar;
                        memberInMessage.FullName = message.Sender.FirstName + " " + message.Sender.LastName;
                    }
                    result.Add(new MessageDTO()
                    {
                        AnonymousFlg = message.AnonymousFlg,
                        MsgText = message.MsgText,
                        CreatedAt = message.CreatedAt.ToString("dd/MM/yyyy hh:mm"),
                        ParentMsg = message.ParentMsgId != null ? parentMessage : null,
                        Member = message.AnonymousFlg == 0 ? memberInMessage : null
                    });
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("participant")]
        public async Task<IActionResult> AddParticipant([FromBody] AddParticipantDTO addParticipantDTO)
        {
            try
            {
                var chat = await _context.Chats.Where(c => c.Id == addParticipantDTO.ChatId).FirstOrDefaultAsync();
                var member = await _context.Members.Where(m => m.Id == addParticipantDTO.MemberId).FirstOrDefaultAsync();
                var participantExist = await _context.Participants.Where(p => p.ChatId == addParticipantDTO.ChatId && p.MemberId == addParticipantDTO.MemberId).FirstOrDefaultAsync();
                if (chat == null)
                {
                    return BadRequest("Chat not exist");
                }
                if (member == null)
                {
                    return BadRequest("Member not exist");
                }
                if (participantExist != null)
                {
                    return BadRequest("This user is already a member of the chat");
                }
                Participant participant = new Participant
                {
                    MemberId = addParticipantDTO.MemberId,
                    ChatId = addParticipantDTO.ChatId,
                    JoinedAt = DateTime.Now,
                    Role = 1,
                    AnonymousTime = 0,
                    NickName = null
                };
                _context.Participants.Add(participant);
                await _context.SaveChangesAsync();
                return Ok("Add new Participant success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
