namespace MyHostel_BackEnd.DTOs
{
    public class NearbyHostelReponse
    {
        public string Name { get; set; } = null!;
        public string DetailLocation { get; set; } = null!;
        public string Price { get; set; }

        public string ImgUrl { get; set; } = null!;
    }
}
