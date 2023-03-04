namespace MyHostel_BackEnd.GoogleMapsResponseObject
{
    public class PlacesNearbySearchResponse
    {
        public string[] html_attributions { get; set; }
        public Place[] results { get; set; }
        public string status{get;set;} 
    }
}
