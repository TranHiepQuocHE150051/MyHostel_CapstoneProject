namespace MyHostel_BackEnd.DTOs
{
    public class FacebookLoginDTO
    {
        public string FacebookId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Avatar { get; set; } = null!;
    }
}
