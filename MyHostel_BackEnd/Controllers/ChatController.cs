using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHostel_BackEnd.DTOs;
using MyHostel_BackEnd.Models;
using Microsoft.AspNetCore.SignalR;
using MyHostel_BackEnd.ChatHubController;
using Newtonsoft.Json;

namespace MyHostel_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private IConfiguration _configuration;
        private MyHostelContext _context;
        public ChatController(IConfiguration configuration, MyHostelContext context, IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
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
                    Role = 0,
                    AnonymousTime = 3,
                    NickName = nickname,
                    Status=0
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

            Message message1 = new Message
            {
                ChatId = message.ChatId,
                SenderId = message.MemberId,
                MsgText = message.MsgText,
                CreatedAt = DateTime.Now,
                AnonymousFlg = (byte)message.AnonymousFlg,
                Status=1
            };
            _context.Messages.Add(message1);
            await _context.SaveChangesAsync();
            if (message.Img.Length > 0)
            {
                foreach (string image in message.Img)
                {
                    MessageImage messageImage = new MessageImage
                    {
                        MessageId = message1.Id,
                        ImageUrl = image

                    };
                    _context.MessageImages.Add(messageImage);
                    await _context.SaveChangesAsync();
                }
            }
            foreach(var participant in participants)
            {
                participant.Status=0;
                _context.Participants.Update(participant);
            }
            await _context.SaveChangesAsync();
            //string jsonStringResult = JsonConvert.SerializeObject(message1);
            string jsonStringResult = JsonConvert.SerializeObject(message1, Formatting.Indented,
        new JsonSerializerSettings()
    {
        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    }
    );
            foreach (var participant in participants)
            {
                if (member.Id != participant.MemberId)
                {
                    await _hubContext.Clients.All.SendAsync($"ReceiveMessage-{participant.MemberId}", "API", jsonStringResult);
                }
            }
            return Ok("Create message success");

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
                IQueryable<Message> messages = from c in _context.Messages.Include(m => m.Sender) where c.ChatId == id && c.Status == 1 orderby c.CreatedAt descending select c;
                PaginatedList<Message> messagesPL = await PaginatedList<Message>.CreateAsync(messages.AsNoTracking(), (int)page, (int)limit);
                List<object> result = new List<object>();
                foreach (var message in messagesPL)
                {
                    if (message.AnonymousFlg == 0)
                    {
                        ParentMessageDTO parentMessage = new ParentMessageDTO();
                        var parentMessageWithImage = new object();
                        MemberInMessageDTO memberInMessage = new MemberInMessageDTO();
                        memberInMessage.Id = message.SenderId;
                        memberInMessage.Avatar = message.Sender.Avatar;
                        memberInMessage.FullName = message.Sender.FirstName + " " + message.Sender.LastName;
                        if (message.ParentMsgId != null)
                        {

                            var parentMsg = await _context.Messages.Where(m => m.Id == message.ParentMsgId).FirstOrDefaultAsync();

                            if (parentMsg != null)
                            {
                                var imgNo = _context.MessageImages.Where(m => m.MessageId == parentMsg.Id).Count();
                                if (parentMsg.MsgText != null)
                                {
                                    parentMessage.Id = parentMsg.Id;
                                    parentMessage.MsgText = parentMsg.MsgText;
                                    result.Add(new MessageDTO()
                                    {
                                        MessageId = message.Id,
                                        AnonymousFlg = message.AnonymousFlg,
                                        MsgText = message.MsgText,
                                        CreatedAt = message.CreatedAt.ToString("dd/MM/yyyy hh:mm"),
                                        ParentMsg =  parentMessage ,
                                        Member = memberInMessage
                                    });
                                }
                                else
                                {
                                    parentMessageWithImage = new
                                    {
                                        Id = parentMsg.Id,
                                        MsgText = parentMsg.MsgText,
                                        ImgNo = imgNo
                                    };
                                    result.Add(new 
                                    {
                                        MessageId = message.Id,
                                        AnonymousFlg = message.AnonymousFlg,
                                        MsgText = message.MsgText,
                                        CreatedAt = message.CreatedAt.ToString("dd/MM/yyyy hh:mm"),
                                        ParentMsg = parentMessageWithImage,
                                        Member = memberInMessage
                                    });
                                }
                            }
                        }
                        else
                        {
                            result.Add(new MessageDTO()
                            {
                                MessageId = message.Id,
                                AnonymousFlg = message.AnonymousFlg,
                                MsgText = message.MsgText,
                                CreatedAt = message.CreatedAt.ToString("dd/MM/yyyy hh:mm"),
                                ParentMsg = null,
                                Member = memberInMessage
                            });
                        }
                        
                    }
                    else
                    {
                        ParentMessageDTO parentMessage = new ParentMessageDTO();
                        var parentMessageWithImage = new object();                       
                        if (message.ParentMsgId != null)
                        {

                            var parentMsg = await _context.Messages.Where(m => m.Id == message.ParentMsgId).FirstOrDefaultAsync();

                            if (parentMsg != null)
                            {
                                var imgNo = _context.MessageImages.Where(m => m.MessageId == parentMsg.Id).Count();
                                if (parentMsg.MsgText != null)
                                {
                                    parentMessage.Id = parentMsg.Id;
                                    parentMessage.MsgText = parentMsg.MsgText;
                                    result.Add(new MessageDTO()
                                    {
                                        MessageId = message.Id,
                                        AnonymousFlg = message.AnonymousFlg,
                                        MsgText = message.MsgText,
                                        CreatedAt = message.CreatedAt.ToString("dd/MM/yyyy hh:mm"),
                                        ParentMsg = parentMessage,
                                    });
                                }
                                else
                                {
                                    parentMessageWithImage = new
                                    {
                                        Id = parentMsg.Id,
                                        MsgText = parentMsg.MsgText,
                                        ImgNo = imgNo
                                    };
                                    result.Add(new
                                    {
                                        MessageId = message.Id,
                                        AnonymousFlg = message.AnonymousFlg,
                                        MsgText = message.MsgText,
                                        CreatedAt = message.CreatedAt.ToString("dd/MM/yyyy hh:mm"),
                                        ParentMsg = parentMessageWithImage,
                                    });
                                }
                            }
                        }
                        else
                        {
                            result.Add(new MessageDTO()
                            {
                                MessageId = message.Id,
                                AnonymousFlg = message.AnonymousFlg,
                                MsgText = message.MsgText,
                                CreatedAt = message.CreatedAt.ToString("dd/MM/yyyy hh:mm"),
                                ParentMsg = null,
                            });
                        }
                        
                    }

                    

                }
                return Ok(result.ToArray());
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
                    Role = 0,
                    AnonymousTime = 0,
                    NickName = member.FirstName + " " + member.LastName,
                    Status = 0
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
        [HttpPut("{id}/participant")]
        public async Task<IActionResult> UpdateStatusParticipant(int id,[FromBody] int ParticipantId)
        {
            try
            {
                var chat = await _context.Chats.Where(c => c.Id == id).FirstOrDefaultAsync();
                if (chat == null)
                {
                    return BadRequest("Chat not exist");
                }
                var participant = _context.Participants.Where(p => p.Id == ParticipantId).SingleOrDefault();
                if (participant.Status == 0)
                {
                    participant.Status = 1;
                }
                _context.Participants.Update(participant);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpDelete("{id}/message")]
        public async Task<IActionResult> DeleteMessage(int id, [FromQuery] int IsImg)
        {
            try
            {
                var message = await _context.Messages.Where(c => c.Id == id).FirstOrDefaultAsync();
                if (message == null)
                {
                    return BadRequest("Message not exist");
                }
                if (IsImg == 0)
                {
                    message.Status = 0;
                    _context.Messages.Update(message);
                    _context.SaveChanges();
                    return Ok("Delete successfully");
                }
                else
                {
                    var MessageImage = _context.MessageImages.Where(m=>m.MessageId==id).ToList();
                    foreach(var image in MessageImage)
                    {
                        _context.MessageImages.Remove(image);
                    }
                    _context.SaveChanges();
                    return Ok("Delete successfully");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
