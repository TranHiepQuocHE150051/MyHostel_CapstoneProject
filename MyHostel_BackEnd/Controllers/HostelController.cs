﻿using GoogleApi.Entities.Maps.Common;
using GoogleApi.Entities.Maps.Directions.Request;
using GoogleApi.Entities.Search.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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
            [FromQuery] string locationCode,
            [FromQuery] string? priceRange,
            [FromQuery] string? amenities,
            [FromQuery] string? nearbyFacilities,
            [FromQuery] int? capacity
            )
        {
            try
            {
                var hostels = await _context.Hostels.Include(h => h.WardsCodeNavigation).ToListAsync();
                var hostelAmenities = await _context.HostelAmenities.ToListAsync();
                if (locationCode.Length == 5)
                {
                    hostels = hostels.Where(h => h.WardsCode == locationCode).ToList();
                }
                if (locationCode.Length == 3)
                {
                    hostels = hostels.Where(h => h.WardsCodeNavigation.DistrictCode == locationCode).ToList();
                }
                if (priceRange != null)
                {
                    string[] prices = priceRange.Split("-");

                    for (int i = 0; i < prices.Length; i++)
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
                if (capacity != null)
                {
                    hostels = hostels.Where(h => Int32.Parse(h.Capacity) >= capacity).ToList();
                }
                if (amenities != null)
                {
                    var amenityIdList = new HashSet<string>(amenities.Split(' '));

                    HashSet<string> hostelIdList = new HashSet<string>();
                    var AllHostelAmenities = _context.HostelAmenities.ToList();
                    List<string> resultsId = new List<string>();
                    foreach (var hostelamenity in AllHostelAmenities)
                    {
                        hostelIdList.Add(hostelamenity.HostedId.ToString());
                    }
                    foreach (var hostelId in hostelIdList)
                    {
                        HashSet<string> amenityInHostelIdList = new HashSet<string>();
                        var hostelamenities = _context.HostelAmenities.Where(h => h.HostedId == int.Parse(hostelId)).ToList();
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
                foreach (var hostel in hostels)
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
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetHostel(int id)
        {
            try
            {
                var hostel = await _context.Hostels.Include(h => h.HostelImages)
                    .Include(h => h.NearbyFacilities)
                    .Include(h => h.HostelAmenities)
                    .Include(h => h.WardsCodeNavigation)
                    .Where(h => h.Id == id).FirstOrDefaultAsync();
                if (hostel != null)
                {
                    GetHostelDTO result = new GetHostelDTO();
                    result.Id = hostel.Id;
                    result.Name = hostel.Name;
                    List<string> imageList = new List<string>();
                    foreach (var image in hostel.HostelImages)
                    {
                        imageList.Add(image.ImageUrl);
                    }
                    result.ImgURL = imageList.ToArray();
                    List<NearbyFacilitiesGetHostelDTO> nearbyFacilitiesResult = new List<NearbyFacilitiesGetHostelDTO>();
                    foreach (var nearbyFacility in hostel.NearbyFacilities)
                    {
                        NearbyFacilitiesGetHostelDTO nearbyFacilitiesGetHostelDTO = new NearbyFacilitiesGetHostelDTO
                        {
                            Name = nearbyFacility.Name,
                            Distance = nearbyFacility.Distance,
                            Duration = nearbyFacility.Duration
                        };
                        nearbyFacilitiesResult.Add(nearbyFacilitiesGetHostelDTO);
                    }
                    result.NearbyFacilities = nearbyFacilitiesResult.ToArray();

                    List<int> AmenitiesResult = new List<int>();
                    foreach (var amenity in hostel.HostelAmenities)
                    {
                        AmenitiesResult.Add(amenity.AmenitiesId);
                    }
                    result.Amenities = AmenitiesResult.ToArray();
                    result.DetailLocation = hostel.DetailLocation;
                    result.WardName = hostel.WardsCodeNavigation.FullName;
                    result.RoomArea = hostel.RoomArea;
                    result.Description = hostel.Description;
                    var lanlord = _context.Members.Where(l => l.Id == hostel.LandlordId).SingleOrDefault();
                    result.Landlord = new LandlordGetHostelDTO
                    {
                        Id = hostel.LandlordId,
                        Name = lanlord.FirstName + lanlord.LastName,
                        Phone = hostel.Phone,
                        Avatar = lanlord.Avatar
                    };
                    var reviews = _context.Residents.Where(r => r.HostelId == id).ToList();
                    if (reviews.Count() > 0)
                    {
                        ReviewGetHostelDTO review = new ReviewGetHostelDTO();
                        double rate = 0.0;
                        int noRate = 0;
                        int noComment = 0;
                        foreach (var item in reviews)
                        {
                            rate += item.Rate;
                            if (item.Rate != 0)
                            {
                                noRate++;
                            }
                            if (!item.Comment.Equals("") || item.Comment != null)
                            {
                                noComment++;
                            }
                        }
                        result.Review = new ReviewGetHostelDTO
                        {
                            Rate = rate / noRate,
                            NoRate = noRate,
                            NoComment = noComment

                        };
                    }
                    else
                    {
                        result.Review = new ReviewGetHostelDTO
                        {
                            Rate = 0.0,
                            NoRate = 0,
                            NoComment = 0
                        };
                    }

                    return Ok(result);
                }
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("landlord/{id}")]
        public async Task<ActionResult> GetHostelForLandlord(int id)
        {
            try
            {
                var hostels = await _context.Hostels
                    .Include(h => h.HostelImages)
                    .Where(h => h.LandlordId == id).ToListAsync();
                var rooms = await _context.Rooms.ToListAsync();
                List<HostelForLanlordResponse> result = new List<HostelForLanlordResponse>();
                foreach (var hostel in hostels)
                {
                    int RoomNo = 0;
                    string ImgUrl = "";
                    if (rooms.Where(r => r.HostelId == hostel.Id).Any())
                    {
                        RoomNo = rooms.Where(r => r.HostelId == hostel.Id).Count();
                    }
                    if (hostel.HostelImages.FirstOrDefault() != null)
                    {
                        ImgUrl = hostel.HostelImages.FirstOrDefault().ImageUrl;
                    }
                    result.Add(
                        new HostelForLanlordResponse
                        {
                            Id = hostel.Id,
                            DetailLocation = hostel.DetailLocation,
                            Name = hostel.Name,
                            RoomNo = RoomNo,
                            ImgUrl = ImgUrl
                        }
                    );
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("{id}/reviews")]
        public async Task<ActionResult> GetHostelReviews(int id)
        {
            try
            {
                var reviews = await _context.Residents.Where(r => r.HostelId == id).ToListAsync();
                var result = new ReviewGetHostelDTO
                {
                    Rate = 0.0,
                    NoRate = 0,
                    NoComment = 0
                };
                List<CommentReviewDTO> comment = new List<CommentReviewDTO>();
                if (reviews.Count() > 0)
                {
                    ReviewGetHostelDTO review = new ReviewGetHostelDTO();
                    double rate = 0.0;
                    int noRate = 0;
                    int noComment = 0;
                    foreach (var item in reviews)
                    {
                        rate += item.Rate;
                        if (item.Rate != 0)
                        {
                            noRate++;
                        }
                        if (!item.Comment.Equals("") || item.Comment != null)
                        {
                            noComment++;
                        }

                        var AvatarUrl = "";
                        if (_context.Members.Where(m => m.Id == item.MemberId).FirstOrDefault() != null)
                        {
                            AvatarUrl = _context.Members.Where(m => m.Id == item.MemberId).FirstOrDefault().Avatar;
                        }
                        comment.Add(new CommentReviewDTO
                        {
                            MemberId = item.MemberId,
                            Text = item.Comment,
                            CreatedDate = String.Format("{0:dd/MM/yyyy}", item.CreatedAt),
                            AvatarUrl = AvatarUrl
                        });
                    }
                    result = new ReviewGetHostelDTO
                    {
                        Rate = rate / noRate,
                        NoRate = noRate,
                        NoComment = noComment,
                        Comment = comment.ToArray()
                    };
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("{id}/rooms")]
        public async Task<ActionResult> GetHostelRooms(int id)
        {
            try
            {
                var rooms = await _context.Rooms.Where(r => r.HostelId == id).ToListAsync();
                List<RoomDTO> result = new List<RoomDTO>();
                foreach (var room in rooms)
                {
                    result.Add(new RoomDTO
                    {
                        RoomId = room.Id,
                        Name = room.Name
                    });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
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
                var hostels = await _context.Hostels.Include(h => h.WardsCodeNavigation).ThenInclude(w => w.DistrictCodeNavigation)
                    .Where(h => h.WardsCodeNavigation.DistrictCodeNavigation.ProvinceCodeNavigation.Code.Equals(provinceCode)).ToListAsync();
                if (userLocationLat != null && userLocationLng != null && !userLocationLat.Equals("") && !userLocationLng.Equals(""))
                {
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
                return Ok(hostels);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{id}/resident")]
        public async Task<ActionResult> AddResident(int id, [FromBody] AddResidentRequest resident)
        {          
            var hostel = _context.Hostels.Where(h => h.Id == id).SingleOrDefault();
            if (hostel == null)
            {
                return BadRequest("Hostel not exist");
            } 
            else
            {
                bool RoomInHostel = false;
                var rooms = _context.Rooms.Where(r => r.HostelId == id).ToList();
                foreach(var room in rooms)
                {
                    if (resident.RoomId == room.Id)
                    {
                        RoomInHostel = true;
                    }
                }
                if (!RoomInHostel)
                {
                    return BadRequest("Room is not in hostel");
                }
            }
            var member = _context.Members.Where(m => m.Id == resident.MemberId).SingleOrDefault();
            if (member == null)
            {
                return BadRequest("Account not exist");
            }
            try
            {                
                Resident resident1 = new Resident
                {
                   HostelId = id,
                   MemberId = resident.MemberId,
                   RoomId = resident.RoomId,
                   ActiveFlg=1,
                   Rate=0,
                   Comment="",
                   CreatedAt=DateTime.Now
                };
                _context.Residents.Add(resident1);
                await _context.SaveChangesAsync();              
                
                return Ok("Add new resident success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("{id}/transaction")]
        public async Task<ActionResult> AddTransaction(int id, [FromBody] AddTransactionRequest transaction)
        {
            var hostel = _context.Hostels.Where(h => h.Id == id).SingleOrDefault();
            if (hostel == null)
            {
                return BadRequest("Hostel not exist");
            }
            else
            {
                HashSet<int> roomInHostel = new HashSet<int>();
                var rooms = _context.Rooms.Where(r => r.HostelId == id).ToList();
                foreach (var room in rooms)
                {
                    roomInHostel.Add(room.Id);
                }
                HashSet<int> roomIds = new HashSet<int>(transaction.roomId);
                if (!roomIds.IsSubsetOf(roomInHostel))
                {
                    return BadRequest("Room is not in hostel");
                }
            }           
            try
            {
                string message = "";
                foreach (var it in transaction.other)
                {
                    message = message+it+ " \n";
                }

                foreach (var item in transaction.roomId)
                {
                    Transaction transaction1 = new Transaction
                    {
                        RoomId = item,
                        Electricity=transaction.electricity,
                        Water = transaction.water,
                        Internet=transaction.internet,
                        Rent=transaction.rent,
                        Security=transaction.security,
                        PaidAt = DateTime.Parse(transaction.sendAt),
                        Other=message

                    };
                    _context.Transactions.Add(transaction1);
                    await _context.SaveChangesAsync();
                }
                return Ok("Add new transaction success");
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
