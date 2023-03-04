namespace MyHostel_BackEnd.GoogleMapsResponseObject
{
    public class PlaceOpeningHours
    {
        public bool open_now { get; set; }
        public PlaceOpeningHoursPeriod[] periods { get; set; }
        public PlaceSpecialDay[] special_days { get; set; }
        public string type { get; set; }
        public string[] weekday_text { get; set; }
    }
}
