namespace MyHostel_BackEnd.DTOs
{
    public class AddReviewDTO
    {
        public int MemberId { get; set; }
        public string Comment { get; set; } = null!;
        public byte Rate { get; set; }
        public short? IsAnonymousComment { get; set; }

    }
}
