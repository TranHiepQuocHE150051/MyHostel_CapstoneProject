namespace MyHostel_BackEnd.DTOs
{
    public class ReviewGetHostelDTO
    {
        public double Rate { get; set; }
        public int NoRate { get; set; }
        public int NoComment { get; set; }
        public CommentReviewDTO[] Comment { get; set; } = null!;
    }
}
