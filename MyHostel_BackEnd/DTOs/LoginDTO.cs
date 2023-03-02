namespace MyHostel_BackEnd.DTOs
{
    public class LoginDTO
    {
        public string socialType { get; set; } = null!;
        public int role { get; set; }
        public string id { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string AvatarURL { get; set; } = null!;
        
    }
}
