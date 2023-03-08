using GoogleApi.Entities.Maps.Common;
using GoogleApi.Entities.Maps.Directions.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHostel_BackEnd.DTOs;
using MyHostel_BackEnd.Models;
using Org.BouncyCastle.Utilities;
using System.Linq;

namespace MyHostel_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HostelController : Controller
    {
        private IConfiguration _configuration;
        private MyHostelContext _context;
        private GlobalVariables _globalVariables;
        public HostelController(IConfiguration configuration, MyHostelContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchHostel(
            [FromQuery] string? locationCode, 
            [FromQuery] int? locationType, 
            [FromQuery] string? priceRange, 
            [FromQuery] string? amenities, 
            [FromQuery] string? nearbyFacilities,
            [FromQuery] int? capacity
            )
        {
            try
            {
                var hostels = await _context.Hostels.ToListAsync();
                var hostelAmenities = await _context.HostelAmenities.ToListAsync();
                if (locationType != null)
                {
                    if(locationType == 1)
                    {
                        hostels = hostels.Where(h => h.WardsCode == locationCode).ToList();
                    }
                    if (locationType == 2)
                    {
                        hostels = hostels.Where(h => h.WardsCodeNavigation.DistrictCode == locationCode).ToList();
                    }
                }
                if(priceRange != null)
                {
                    string[] prices = priceRange.Split("-");

                    for(int i = 0; i < prices.Length; i++)
                    {
                        if (prices[i].ToLower().Contains("k"))
                        {
                            prices[i] = prices[i].Replace("k", "000");
                        }
                        if (prices[i].ToLower().Contains("m"))
                        {
                            prices[i] = prices[i].Replace(".", "");
                            prices[i] = prices[i].Replace("m", "000000");
                        }
                    }
                    if (prices[0] != "" && prices[1] != "")
                    {
                        hostels = hostels.Where(h => h.Price >= Decimal.Parse(prices[0]) && h.Price <= Decimal.Parse(prices[1])).ToList();
                    }
                    else
                    {
                        if (prices[0] != "" && prices[1] == "")
                        {
                            hostels = hostels.Where(h => h.Price >= Decimal.Parse(prices[0])).ToList();
                        }
                        else if (prices[0] == "" && prices[1] != "")
                        {
                            hostels = hostels.Where(h => h.Price <= Decimal.Parse(prices[1])).ToList();
                        }
                    }
                }
                if(capacity != null)
                {
                    hostels = hostels.Where(h => Int32.Parse(h.Capacity) >= capacity).ToList();
                }
                if(amenities != null)
                {
                    var amenityIdList = new HashSet<string>(amenities.Split(' '));
                    
                    HashSet<string> hostelIdList = new HashSet<string>();
                    var AllHostelAmenities = _context.HostelAmenities.ToList();
                    List<string> resultsId = new List<string>(); 
                    foreach(var hostelamenity in AllHostelAmenities)
                    {
                        hostelIdList.Add(hostelamenity.HostedId.ToString());
                    }
                    foreach(var hostelId in hostelIdList)
                    {
                        HashSet<string> amenityInHostelIdList = new HashSet<string>();
                        var hostelamenities= _context.HostelAmenities.Where(h=>h.HostedId==int.Parse(hostelId)).ToList();
                        foreach (var hostelamenity in hostelamenities)
                        {
                            amenityInHostelIdList.Add(hostelamenity.AmenitiesId.ToString());
                        }
                        if (amenityIdList.IsSubsetOf(amenityInHostelIdList))
                        {
                            resultsId.Add(hostelId);
                        }
                    }
                    if (resultsId.Count == 0)
                    {
                        return NotFound("Cannot find required hostel");
                    }
                    else
                    {
                        hostels = hostels.Where(h => resultsId.Contains(h.Id.ToString())).ToList();
                    }
                                      
                }
                if (nearbyFacilities != null)
                {
                    var nearbyFacilityIdList = new HashSet<string>(nearbyFacilities.Split(' '));

                    HashSet<string> hostelIdList = new HashSet<string>();
                    var AllNearbyFacilities = _context.NearbyFacilities.ToList();
                    List<string> resultsId = new List<string>();
                    foreach (var nearbyFacility in AllNearbyFacilities)
                    {
                        hostelIdList.Add(nearbyFacility.HostelId.ToString());
                    }
                    foreach (var hostelId in hostelIdList)
                    {
                        HashSet<string> nearbyIdList = new HashSet<string>();
                        var nearbyfacilites = _context.NearbyFacilities.Where(h => h.HostelId == int.Parse(hostelId)).ToList();
                        foreach (var nearbyFacility in nearbyfacilites)
                        {
                            nearbyIdList.Add(nearbyFacility.UltilityId.ToString());
                        }
                        if (nearbyFacilityIdList.IsSubsetOf(nearbyIdList))
                        {
                            resultsId.Add(hostelId);
                        }
                    }
                    if (resultsId.Count == 0)
                    {
                        return NotFound("Cannot find required hostel");
                    }
                    else
                    {
                        hostels = hostels.Where(h => resultsId.Contains(h.Id.ToString())).ToList();
                    }
                }
                List<HostelSearchDTO> result = new List<HostelSearchDTO>();
                foreach(var hostel in hostels)
                {
                    string imgUrl = "";
                    if (hostel.HostelImages.FirstOrDefault() != null)
                    {
                        imgUrl = hostel.HostelImages.FirstOrDefault().ImageUrl;
                    }
                    result.Add(new HostelSearchDTO()
                    {
                        DetailLocation = hostel.DetailLocation,
                        Id = hostel.Id,
                        Name = hostel.Name,
                        Price = hostel.Price,
                        imgUrl = imgUrl
                    });
                }
                if (result.Count != 0)
                    return Ok(result);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("searchNearbyHostel")]
        public async Task<IActionResult> SearchNearbyHostel(
            [FromQuery] string? provinceCode,
            [FromQuery] string? userLocationLat,
            [FromQuery] string? userLocationLng
            )
        {
            try
            {
                var hostels = await _context.Hostels.Where(h => h.WardsCodeNavigation.DistrictCodeNavigation.ProvinceCodeNavigation.Code.Equals(provinceCode)).ToListAsync();
                if(userLocationLat!=null && userLocationLng!=null && !userLocationLat.Equals("") && !userLocationLng.Equals("")){
                    foreach (var hostel in hostels)
                    {
                        int distance = CalculateDistance(Double.Parse(userLocationLat),
                            Double.Parse(userLocationLng),
                            Double.Parse(hostel.GoogleLocationLat),
                            Double.Parse(hostel.GoogleLocationLnd));
                        if (distance > 2000)
                        {
                            hostels.Remove(hostel);
                        }
                    }
                }
                if (hostels.Count != 0)
                    return Ok(hostels);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        private int CalculateDistance(double orglat, double orglng, double deslat, double deslng)
        {
            DirectionsRequest request = new DirectionsRequest();
            request.Key = GlobalVariables.API_KEY;
            request.Origin = new LocationEx(new CoordinateEx(orglat, orglng));
            request.Destination = new LocationEx(new CoordinateEx(deslat, deslng));
            var response = GoogleApi.GoogleMaps.Directions.Query(request);

            int distance = response.Routes.First().Legs.First().Distance.Value;
                return distance;

        }
    }
    
}
