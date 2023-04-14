namespace MyHostel_BackEnd.DTOs
{
    public class GroupChatDTO
    {
        public int ChatId { get; set; }
        public string? Name { get; set; }
        public byte IsGroup { get; set; }
        public string? Avatar { get; set; }
        public GroupChatMessageDTO? LastMsg { get; set; }
    }
}
