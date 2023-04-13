using System.Security.Policy;

namespace MyHostel_BackEnd.DTOs
{
    public class ChatDTO
    {
        public int ChatId { get; set; }
        public string? Name { get; set; }
        public byte IsGroup { get; set; }
        public string? Avatar { get; set; }
        public MessageDTO? lastMSG { get; set; }
    }
}
