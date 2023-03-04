using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHostel_BackEnd.DTOs;
using MyHostel_BackEnd.Models;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using GoogleApi.Entities.Maps;
using GoogleApi.Entities.Places.Search.NearBy.Request;
using GoogleApi.Entities.Maps.Common;
using GoogleApi.Entities.Places.Search.Common.Enums;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using GoogleApi;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Interfaces;
using GoogleApi.Entities.Places.Details.Response;
using GoogleApi.Entities.Maps.Directions.Request;
using GoogleApi.Entities.Search.Common;
using System.Net;
using System.Xml.Linq;
using System.Text.Json;
using MyHostel_BackEnd.GoogleMapsResponseObject;
using PlacesNearbySearchResponse = MyHostel_BackEnd.GoogleMapsResponseObject.PlacesNearbySearchResponse;
using Place = MyHostel_BackEnd.GoogleMapsResponseObject.Place;

namespace MyHostel_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class landlordController : ControllerBase
    {
        private IConfiguration _configuration;
        private MyHostelContext _context;
        private GlobalVariables _globalVariables;
        public landlordController(IConfiguration configuration, MyHostelContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpPost("register")]
        public async Task<IActionResult> HostelRegister([FromBody] HostelRegisterDTO hostel)
        {
            
            try
            {
                Hostel hostel1 = new Hostel
                {
                    Name = hostel.Name,
                    Price = hostel.Price,
                    Capacity = hostel.Capacity,
                    DetailLocation = hostel.DetailLocation,
                    GoogleLocationLat = hostel.LocationLat,
                    GoogleLocationLnd = hostel.LocationLng,
                    WardsCode = hostel.WardsCode,
                    Phone = hostel.Phone,
                    Description = hostel.Description,
                    RoomArea = hostel.RoomArea,                   
                    LandlordId = 1002
                };
                _context.Hostels.Add(hostel1);
                await _context.SaveChangesAsync();
                foreach (string image in hostel.imagesUrl)
                {
                    HostelImage hostelImage = new HostelImage
                    {
                        HostelId = hostel1.Id,
                        ImageUrl = image

                    };
                    _context.HostelImages.Add(hostelImage);
                    _context.SaveChanges();
                }
                foreach (int amenityId in hostel.Amenities)
                {
                    var amenity = _context.Amenities.SingleOrDefault(a => a.Id == amenityId);
                    if (amenity != null) {
                        HostelAmenity hostelAmenity = new HostelAmenity
                        {
                            HostedId = hostel1.Id,
                            AmenitiesId = amenityId

                        };
                        _context.HostelAmenities.Add(hostelAmenity);
                        _context.SaveChanges();
                    }
                    
                }
                for (int i = 1; i <= hostel.rooms; i++)
                {
                    Room room = new Room
                    {
                        HostelId = hostel1.Id,
                        Name = hostel.Name + " room " + i

                    };
                    _context.Rooms.Add(room);
                    _context.SaveChanges();
                }
                CheckNearbyAmenity(hostel.LocationLat, hostel.LocationLng, hostel1.Id);
                return Ok("Add new hostel Success");
            } catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        private void CheckNearbyAmenity(string latitude, string longitude , int hostelid)
        {
            var facilities = _context.Facilities.ToList();
            foreach (var facility in facilities)
            {
                string URL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?keyword=&location="+latitude+"%2C"+longitude+"&radius=1500&type="+facility.UtilityName+"&key=AIzaSyDLsmLiVOBVQKt-PP6Xf5b1zmNR3wqiURI";               
                WebRequest request = WebRequest.Create(URL);
                WebResponse response = request.GetResponse();
                Stream data = response.GetResponseStream();

                StreamReader reader = new StreamReader(data);
                string responseFromServer = reader.ReadToEnd();
                Console.WriteLine(responseFromServer);
                if (responseFromServer != null)
                {
                    var results = JsonSerializer.Deserialize<PlacesNearbySearchResponse>(responseFromServer);
                    if (results.results.Length > 0) {
                        Place nearByResult = results.results.First();
                        double reslat = nearByResult.geometry.formatted_address.lat;
                        double reslng = nearByResult.geometry.formatted_address.lng;
                        double orglat = Double.Parse(latitude);
                        double orglng = Double.Parse(longitude);
                        DistanceAndDuration distanceAndDuration = CalculateDistanceAndDuration(orglat, orglng, reslat, reslng);
                        NearbyFacility nearbyFacility = new NearbyFacility
                        {
                            UltilityId = facility.Id,
                            HostelId = hostelid,
                            Name = nearByResult.name,
                            Distance = distanceAndDuration.Distance,
                            Duration = distanceAndDuration.Duration

                        };
                    }
                    
                }
                
            }
        }
        private DistanceAndDuration CalculateDistanceAndDuration(double orglat, double orglng, double deslat, double deslng)
        {
            DirectionsRequest request = new DirectionsRequest();

            request.Key = GlobalVariables.API_KEY;

            request.Origin = new LocationEx(new CoordinateEx(orglat, orglng) );
            request.Destination = new LocationEx(new CoordinateEx(deslat, deslng) );
            var response = GoogleApi.GoogleMaps.Directions.Query(request);
            DistanceAndDuration distanceAndDuration = new DistanceAndDuration
            {
                Distance = response.Routes.First().Legs.First().Distance.Value,
                Duration = response.Routes.First().Legs.First().DurationInTraffic.Value

            };
            return distanceAndDuration;
        }
    }
}
