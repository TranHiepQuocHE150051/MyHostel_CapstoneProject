namespace MyHostel_BackEnd.DTOs
{
    public class SingleChatDTO
    {
        public int ChatId { get; set; }
        public string? Name { get; set; }
        public byte IsGroup { get; set; }
        public SingleChatMessageDTO? LastMsg { get; set; }
        public SingleChatParicipantDTO? Participant { get; set; }

    }
}
