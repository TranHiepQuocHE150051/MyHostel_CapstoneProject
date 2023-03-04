namespace MyHostel_BackEnd.GoogleMapsResponseObject
{
    public class PlaceReview
    {
        public string author_name { get; set; }
        public double rating { get; set; }
        public string relative_time_description { get; set; }
        public int time { get; set; }
        public string author_url { get; set; }
        public string language { get; set; }
        public string original_language { get; set; }
        public string profile_photo_url { get; set; }
        public string text { get; set; }
        public bool translated { get; set; }
    }
}
