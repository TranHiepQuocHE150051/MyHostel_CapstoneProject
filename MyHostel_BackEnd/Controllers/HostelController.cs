﻿using FirebaseAdmin.Messaging;
using GoogleApi.Entities.Maps.Common;
using GoogleApi.Entities.Maps.Directions.Request;
using GoogleApi.Entities.Search.Video.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHostel_BackEnd.DTOs;
using MyHostel_BackEnd.Models;
using System.Globalization;
using System.Net;
using System.Xml;

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
            [FromQuery] int? capacity,
            [FromQuery] int? pageIndex,
            [FromQuery] int? pageSize
            )
        {
            try
            {
                if (pageIndex == null || pageIndex <= 0)
                {
                    pageIndex = 1;
                }
                if (pageSize == null || pageSize <= 0)
                {
                    pageSize = 6;
                }
                IQueryable<Hostel> hostels = from h
                                            in _context.Hostels
                                            .Include(h => h.WardsCodeNavigation)
                                            .Include(h => h.HostelImages)
                                            .Where(h => h.Status == 1)
                                             select h;
                var hostelAmenities = await _context.HostelAmenities.ToListAsync();
                if (locationCode == null)
                {
                    return NotFound();
                }
                if (locationCode.Length == 5)
                {
                    hostels = hostels.Where(h => h.WardsCode == locationCode);
                }
                if (locationCode.Length == 3)
                {
                    hostels = hostels.Where(h => h.WardsCodeNavigation.DistrictCode == locationCode);
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
                        hostels = hostels.Where(h => h.Price >= decimal.Parse(prices[0]) && h.Price <= decimal.Parse(prices[1]));
                    }
                    else
                    {
                        if (prices[0] != "" && prices[1] == "")
                        {
                            hostels = hostels.Where(h => h.Price >= decimal.Parse(prices[0]));
                        }
                        else if (prices[0] == "" && prices[1] != "")
                        {
                            hostels = hostels.Where(h => h.Price <= decimal.Parse(prices[1]));
                        }
                    }
                }
                if (capacity != null)
                {
                    hostels = hostels.Where(h => (int)(object)(h.Capacity.Trim()) >= capacity);
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
                        hostels = hostels.Where(h => resultsId.Contains(h.Id.ToString()));
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
                        hostels = hostels.Where(h => resultsId.Contains(h.Id.ToString()));
                    }
                }
                PaginatedList<Hostel> hostelsPL = await PaginatedList<Hostel>.CreateAsync(hostels.AsNoTracking(), (int)pageIndex, (int)pageSize);
                List<HostelSearchDTO> result = new List<HostelSearchDTO>();
                foreach (var hostel in hostelsPL)
                {
                    string imgUrl = "";
                    if (hostel.HostelImages.FirstOrDefault() != null)
                    {
                        imgUrl = hostel.HostelImages.FirstOrDefault().ImageUrl;
                    }
                    List<object> amenitites = new List<object>();
                    var HostelAmenities = _context.HostelAmenities.Where(h => h.HostedId == hostel.Id).ToList();
                    foreach (var HostelAmenity in HostelAmenities)
                    {
                        var amenity = _context.Amenities.Where(a => a.Id == HostelAmenity.AmenitiesId).SingleOrDefault();
                        amenitites.Add(new
                        {
                            Id = amenity.Id,
                            Name = amenity.AmenitiyName,
                            Icon = amenity.Icon
                        });
                    }
                    var nearbyFacility = _context.NearbyFacilities.Where(n => n.HostelId == hostel.Id).Include(n => n.Ultility).ToList();
                    HashSet<string> hostelNearbyFacility = new HashSet<string>();
                    foreach (var nearby in nearbyFacility)
                    {
                        hostelNearbyFacility.Add(nearby.Ultility.UtilityName);
                    }
                    var residents = _context.Residents.Where(r => r.HostelId == hostel.Id).ToList();
                    int NoRate = 0;
                    double Rate = 0;
                    foreach (var resident in residents)
                    {
                        if (resident.Rate > 0)
                        {
                            Rate += resident.Rate;
                            NoRate++;
                        }
                    }
                    if (NoRate == 0)
                    {
                        result.Add(new HostelSearchDTO()
                        {
                            DetailLocation = hostel.DetailLocation,
                            Id = hostel.Id,
                            Name = hostel.Name,
                            Price = replaceString(hostel.Price),
                            imgUrl = imgUrl,
                            Amenities = amenitites,
                            NearbyFacilities = hostelNearbyFacility.ToList(),
                            Review = new
                            {
                                Star = 0
                            }
                        });
                    }
                    else
                    {
                        result.Add(new HostelSearchDTO()
                        {
                            DetailLocation = hostel.DetailLocation,
                            Id = hostel.Id,
                            Name = hostel.Name,
                            Price = replaceString(hostel.Price),
                            imgUrl = imgUrl,
                            Amenities = amenitites,
                            NearbyFacilities = hostelNearbyFacility.ToList(),
                            Review = new
                            {
                                Star = Rate / NoRate
                            }
                        });
                    }

                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetHostel(int id, [FromQuery] int? priceConvert)
        {
            try
            {
                var hostel = await _context.Hostels.Include(h => h.HostelImages)
                    .Include(h => h.NearbyFacilities)
                    .Include(h => h.HostelAmenities)
                    .Include(h => h.WardsCodeNavigation)
                    .ThenInclude(w => w.DistrictCodeNavigation)
                    .ThenInclude(d => d.ProvinceCodeNavigation)
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
                    var facilities = _context.Facilities.ToList();
                    foreach (var facility in facilities)
                    {
                        var nearbyfacilities = _context.NearbyFacilities.Where(n => n.HostelId == id && n.UltilityId == facility.Id).ToList();
                        if (nearbyfacilities.Any())
                        {
                            NearbyFacilitiesGetHostelDTO nearby = new NearbyFacilitiesGetHostelDTO
                            {
                                Id = facility.Id,
                                Name = facility.UtilityName,
                                Places = new List<object>()

                            };
                            foreach (var nearbyfacility in nearbyfacilities)
                            {
                                nearby.Places.Add(new
                                {
                                    Name = nearbyfacility.Name,
                                    Distance = nearbyfacility.Distance,
                                    Duration = nearbyfacility.Duration

                                });
                            }
                            nearbyFacilitiesResult.Add(nearby);
                        }
                    }
                    result.NearbyFacilities = nearbyFacilitiesResult.ToArray();

                    List<AmenitiesGetHostelDTO> AmenitiesResult = new List<AmenitiesGetHostelDTO>();
                    foreach (var amenity in hostel.HostelAmenities)
                    {
                        var amen = await _context.Amenities.Where(a => a.Id == amenity.AmenitiesId).FirstOrDefaultAsync();
                        AmenitiesResult.Add(new AmenitiesGetHostelDTO
                        {
                            Id = amen.Id,
                            Icon = amen.Icon,
                            Name = amen.AmenitiyName
                        });
                    }
                    result.Amenities = AmenitiesResult.ToArray();
                    result.DetailLocation = hostel.DetailLocation;
                    result.LocationLat = hostel.GoogleLocationLat;
                    result.LocationLng = hostel.GoogleLocationLnd;
                    result.WardCode = hostel.WardsCode;
                    result.WardName = hostel.WardsCodeNavigation.FullName;
                    result.DistrictCode = hostel.WardsCodeNavigation.DistrictCode;
                    result.DistricName = hostel.WardsCodeNavigation.DistrictCodeNavigation.FullName;
                    result.ProvinceCode = hostel.WardsCodeNavigation.DistrictCodeNavigation.ProvinceCode;
                    result.ProvinceName = hostel.WardsCodeNavigation.DistrictCodeNavigation.ProvinceCodeNavigation.FullName;
                    result.RoomArea = hostel.RoomArea.Trim();
                    result.Capacity = hostel.Capacity.Trim();
                    result.Rooms = (await _context.Rooms.Where(h => h.HostelId == result.Id).CountAsync()).ToString();
                    result.Description = hostel.Description;
                    result.Price = priceConvert == 0 ? string.Format("{0:0.##}", hostel.Price) : replaceString(hostel.Price);
                    result.Electricity = priceConvert == 0 ? string.Format("{0:0.##}", hostel.Electricity) : replaceString(hostel.Electricity);
                    result.Water = priceConvert == 0 ? string.Format("{0:0.##}", hostel.Water) : replaceString(hostel.Water);
                    result.Internet = priceConvert == 0 ? string.Format("{0:0.##}", hostel.Internet) : replaceString(hostel.Internet);
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
                        GetHostelReviewDTO review = new GetHostelReviewDTO();
                        double rate = 0.0;
                        int noRate = 0;
                        int noComment = 0;
                        foreach (var item in reviews)
                        {
                            if (CheckResidentChangedRoom(id, item.MemberId))
                            {
                                if (item.Status != 1)
                                {
                                    continue;
                                }
                            }
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
                        result.Review = new GetHostelReviewDTO
                        {
                            Rate = rate / noRate,
                            NoRate = noRate,
                            NoComment = noComment

                        };
                    }
                    else
                    {
                        result.Review = new GetHostelReviewDTO
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
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("landlord/{id}")]
        public async Task<ActionResult> GetHostelForLandlord(int id)
        {
            try
            {
                var hostels = await _context.Hostels
                    .Include(h => h.HostelImages)
                    .Where(h => h.LandlordId == id && h.Status != 4).ToListAsync();
                var rooms = await _context.Rooms.ToListAsync();
                var residents = await _context.Residents.ToListAsync();
                List<HostelForLanlordResponse> result = new List<HostelForLanlordResponse>();
                foreach (var hostel in hostels)
                {
                    int RoomNo = 0;
                    int ResidentNo = 0;
                    int AvailableRooms = 0;
                    string ImgUrl = "";
                    if (rooms.Where(r => r.HostelId == hostel.Id).Any())
                    {
                        RoomNo = rooms.Where(r => r.HostelId == hostel.Id).Count();
                    }
                    if (residents.Where(r => r.HostelId == hostel.Id).Any())
                    {
                        ResidentNo = residents.Where(r => r.HostelId == hostel.Id && r.Status == 1).Count();
                    }
                    if (hostel.HostelImages.FirstOrDefault() != null)
                    {
                        ImgUrl = hostel.HostelImages.FirstOrDefault().ImageUrl;
                    }
                    var roomsInHostel = _context.Rooms.Where(r => r.HostelId == hostel.Id).ToList();

                    foreach (var room in roomsInHostel)
                    {
                        if (CountResidentInRoom(room) == 0)
                        {
                            AvailableRooms++;
                        }
                    }
                    result.Add(
                        new HostelForLanlordResponse
                        {
                            Id = hostel.Id,
                            DetailLocation = hostel.DetailLocation,
                            Name = hostel.Name,
                            RoomNo = RoomNo,
                            ImgUrl = ImgUrl,
                            Status = hostel.Status,
                            ResidentNo = ResidentNo,
                            AvailableRooms = AvailableRooms
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
                var hostel = await _context.Hostels.Include(h => h.Landlord).Where(h => h.Id == id).FirstOrDefaultAsync();
                var result = new
                {
                    Rate = 0.0,
                    RateNo = 0,
                    LandlordId = 0,
                    Comment = new List<object>()
                };
                List<object> comment = new List<object>();
                if (reviews.Count() > 0)
                {
                    double rate = 0.0;
                    int noRate = 0;
                    foreach (var item in reviews)
                    {
                        if (item.Rate != 0)
                        {
                            rate += item.Rate;
                            noRate++;
                            var AvatarUrl = "";
                            var member = _context.Members.Where(m => m.Id == item.MemberId).FirstOrDefault();
                            if (member != null)
                            {
                                AvatarUrl = member.Avatar;
                            }

                            comment.Add(new
                            {
                                IsAnonymous = item.IsAnonymousComment,
                                AvatarUrl = AvatarUrl,
                                Name = $"{member.FirstName} {member.LastName}",
                                MemberId = item.MemberId,
                                Text = item.Comment,
                                Rate = item.Rate,
                                CreatedDate = String.Format("{0:dd/MM/yyyy}", item.CreatedAt),

                            });
                        }

                    }
                    result = new
                    {
                        Rate = noRate == 0 ? 0 : (rate / noRate),
                        RateNo = noRate,
                        LandlordId = hostel.Landlord.Id,
                        Comment = comment
                    };
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("{hostelId}/review/member/{memberId}")]
        public async Task<ActionResult> GetHostelReview(int hostelId, int memberId)
        {
            try
            {
                var reviews = await _context.Residents.Where(r => r.MemberId == memberId && r.HostelId == hostelId).OrderByDescending(r => r.CreatedAt).FirstOrDefaultAsync();


                return Ok(new
                {
                    MemberId = memberId,
                    Rate = reviews.Rate,
                    Text = reviews.Comment,
                    IsAnonymous = reviews.IsAnonymousComment
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        private bool CheckResidentChangedRoom(int hostelId, int MemberId)
        {
            var residents = _context.Residents.Where(r => r.HostelId == hostelId && r.MemberId == MemberId).ToList();
            if (residents.Count > 1)
            {
                return true;
            }
            return false;

        }


        [HttpGet("{id}/rooms")]
        public async Task<ActionResult> GetHostelRooms(int id)
        {
            try
            {
                var rooms = await _context.Rooms.Where(r => r.HostelId == id).Include(r => r.Residents).ToListAsync();
                List<RoomDTO> result = new List<RoomDTO>();
                foreach (var room in rooms)
                {
                    List<ResidentsInRoomDTO> residents = new List<ResidentsInRoomDTO>();
                    RoomDTO room1 = new RoomDTO();
                    room1.RoomId = room.Id;
                    room1.Name = room.Name;
                    room1.Price = (decimal)room.Price;
                    room1.RoomArea = (int)room.RoomArea;
                    room1.ConvertPrice = replaceString((decimal)room.Price);
                    foreach (var resident in room.Residents)
                    {
                        var member = _context.Members.Where(m => m.Id == resident.MemberId && resident.Status == 1).FirstOrDefault();
                        if (member != null)
                        {
                            residents.Add(new ResidentsInRoomDTO
                            {
                                MemberId = member.Id,
                                FullName = member.FirstName + " " + member.LastName,
                                Avatar = member.Avatar,
                                JoinedAt = resident.CreatedAt.ToString("MM/dd/yyyy"),
                                InviteToken = member.InviteCode.Trim()
                            });
                        }

                    }
                    room1.Residents = residents;
                    result.Add(room1);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
        [HttpGet("search/nearby")]
        public async Task<IActionResult> SearchNearbyHostel(
            [FromQuery] string? provinceCode,
            [FromQuery] string? userLocationLat,
            [FromQuery] string? userLocationLng,
            [FromQuery] int? page,
            [FromQuery] int? limit
            )
        {
            try
            {
                if (page == null || page <= 0)
                {
                    page = 1;
                }
                if (limit == null || limit <= 0)
                {
                    limit = 6;
                }
                IQueryable<Hostel> hostels = from h
                                             in _context.Hostels
                                             .Include(h => h.HostelImages)
                                             .Include(h => h.HostelAmenities)
                                             .Include(h => h.WardsCodeNavigation)
                                             .ThenInclude(w => w.DistrictCodeNavigation)
                                             .ThenInclude(d => d.ProvinceCodeNavigation)
                                             .Where(h => h.WardsCodeNavigation.DistrictCodeNavigation.ProvinceCodeNavigation.Code.Equals(provinceCode) && h.Status == 1)
                                             select h;
                if (userLocationLat != null && userLocationLng != null && !userLocationLat.Equals("") && !userLocationLng.Equals(""))
                {
                    var checkList = hostels;
                    foreach (var hostel in checkList)
                    {
                        if (hostel.GoogleLocationLat == null || hostel.GoogleLocationLnd == null)
                        {
                            hostels = hostels.Where(h => h.Id != hostel.Id);
                            continue;
                        }
                        var distance = DistanceTo(Double.Parse(userLocationLat, new CultureInfo("en-US")),
                                Double.Parse(userLocationLng, new CultureInfo("en-US")),
                                Double.Parse(hostel.GoogleLocationLat, new CultureInfo("en-US")),
                                Double.Parse(hostel.GoogleLocationLnd, new CultureInfo("en-US")));
                        if (distance > 3000)
                        {
                            hostels = hostels.Where(h => h.Id != hostel.Id);
                        }
                        //var distance = GetDistance(userLocationLat, userLocationLng, hostel.GoogleLocationLat, hostel.GoogleLocationLnd);
                        //if(distance > 0)
                        //{
                        //    if (distance > 3000)
                        //    {
                        //        hostels = hostels.Where(h => h != hostel);
                        //    }
                        //}
                        //else
                        //{
                        //    distance = DistanceTo(double.Parse(userLocationLat.Replace(".",",")),
                        //        double.Parse(userLocationLng.Replace(".",",")),
                        //        double.Parse(hostel.GoogleLocationLat.Replace(".",",")),
                        //        double.Parse(hostel.GoogleLocationLnd.Replace(".",",")));
                        //    if (distance == 0)
                        //    {
                        //        continue;
                        //    }
                        //    if(distance > 3000)
                        //    {
                        //        hostels = hostels.Where(h => h != hostel);
                        //    }
                        //}
                    }
                }
                PaginatedList<Hostel> hostelsPL = await PaginatedList<Hostel>.CreateAsync(hostels.AsNoTracking(), (int)page, (int)limit);
                List<NearbyHostelReponse> reponses = new List<NearbyHostelReponse>();
                foreach (var hostel in hostelsPL)
                {
                    string ImgUrl = "";
                    if (hostel.HostelImages.FirstOrDefault() != null)
                    {
                        ImgUrl = hostel.HostelImages.FirstOrDefault().ImageUrl;
                    }
                    List<AmenitiesGetHostelDTO> AmenitiesResult = new List<AmenitiesGetHostelDTO>();
                    foreach (var amenity in hostel.HostelAmenities)
                    {
                        var amen = await _context.Amenities.Where(a => a.Id == amenity.AmenitiesId).FirstOrDefaultAsync();
                        AmenitiesResult.Add(new AmenitiesGetHostelDTO
                        {
                            Id = amen.Id,
                            Icon = amen.Icon,
                            Name = amen.AmenitiyName
                        });
                    }
                    double totalRate = 0.0;
                    double Rate = 0.0;
                    int noRate = 0;
                    var reviews = _context.Residents.Where(r => r.HostelId == hostel.Id).ToList();
                    if (reviews.Count() > 0)
                    {
                        GetHostelReviewDTO review = new GetHostelReviewDTO();
                        foreach (var item in reviews)
                        {
                            if (CheckResidentChangedRoom(hostel.Id, item.MemberId))
                            {
                                if (item.Status != 1)
                                {
                                    continue;
                                }
                            }
                            totalRate += item.Rate;
                            if (item.Rate != 0)
                            {
                                noRate++;
                            }
                        }
                        if (noRate > 0)
                        {
                            Rate = totalRate / noRate;
                        }
                    }
                    reponses.Add(new NearbyHostelReponse
                    {
                        HostelId = hostel.Id,
                        Name = hostel.Name,
                        DetailLocation = hostel.DetailLocation,
                        WardName = hostel.WardsCodeNavigation.FullName,
                        DistrictName = hostel.WardsCodeNavigation.DistrictCodeNavigation.FullName,
                        Price = replaceString(hostel.Price),
                        ImgUrl = ImgUrl,
                        Amenities = AmenitiesResult.ToArray(),
                        Rate = Rate,
                        NoRate = noRate
                    })
                ;
                }
                return Ok(new
                {
                    Hostels = reponses.ToArray()
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("{id}/resident")]
        public async Task<ActionResult> AddResident(int id, [FromBody] AddResidentRequest resident)
        {
            if (resident.InviteCode == null || resident.RoomId == 0)
            {
                return Ok(new
                {
                    IsSuccess = false,
                    IsInHostel = false,
                    Message = "invite Code not valid"

                });
            }
            var hostel = _context.Hostels.Where(h => h.Id == id).SingleOrDefault();
            if (hostel == null)
            {
                //return BadRequest("Hostel not exist");
                return Ok(new
                {
                    IsSuccess = false,
                    IsInHostel = false,
                    Message = "Hostel not exist"
                });
            }
            else
            {
                var lanlord = _context.Members.Where(m => m.Id == hostel.LandlordId).SingleOrDefault();
                if (resident.InviteCode.Trim().Equals(lanlord.InviteCode.Trim()))
                {
                    return Ok(new
                    {
                        IsSuccess = false,
                        IsInHostel = false,
                        Message = "This is landlord inviteCode"
                    });
                }
                bool RoomInHostel = false;
                var rooms = _context.Rooms.Where(r => r.HostelId == id).ToList();
                foreach (var room in rooms)
                {
                    if (resident.RoomId == room.Id)
                    {
                        RoomInHostel = true;
                    }

                }
                if (!RoomInHostel)
                {
                    //return BadRequest("Room is not in hostel");
                    return Ok(new
                    {
                        IsSuccess = false,
                        IsInHostel = false,
                        Message = "Room is not in hostel"
                    });
                }
                Room requestRoom = _context.Rooms.Where(r => r.Id == resident.RoomId).SingleOrDefault();
                if (requestRoom != null)
                {
                    if (CountResidentInRoom(requestRoom) >= int.Parse(hostel.Capacity.Trim()))
                    {
                        //return BadRequest("Room is full");
                        return Ok(new
                        {
                            IsSuccess = false,
                            IsInHostel = false,
                            Message = "Room is full"
                        });
                    }
                }
            }
            var member = _context.Members.Where(m => m.InviteCode.Trim().Equals(resident.InviteCode.Trim())).SingleOrDefault();
            var landlord = _context.Members.Where(m => m.Id == hostel.LandlordId).SingleOrDefault();
            if (member == null)
            {
                //return BadRequest("Account not exist");
                return Ok(new
                {
                    IsSuccess = false,
                    IsInHostel = false,
                    Message = "Cannot find user"
                });
            }
            var residentInfo = _context.Residents.Where(r => r.MemberId == member.Id && r.Status == 1).SingleOrDefault();
            if (residentInfo != null)
            {
                if (residentInfo.HostelId == id)
                {
                    if (residentInfo.RoomId == resident.RoomId)
                    {
                        return Ok(new
                        {
                            IsSuccess = false,
                            IsInHostel = true
                        });
                    }
                    var roomInfo = _context.Rooms.Where(r => r.Id == residentInfo.RoomId).SingleOrDefault();
                    return Ok(new
                    {
                        IsSuccess = false,
                        IsInHostel = true,
                        RoomId = residentInfo.RoomId,
                        RoomName = roomInfo.Name
                    });
                }
                else
                {
                    return Ok(new
                    {
                        IsSuccess = false,
                        IsInHostel = false,
                        Message = "User is in another hostel"
                    });
                }
            }

            try
            {
                Resident resident1 = new Resident
                {
                    HostelId = id,
                    MemberId = member.Id,
                    RoomId = resident.RoomId,
                    Status = 1,
                    Rate = 0,
                    Comment = "",
                    CreatedAt = DateTime.Now,
                    IsAnonymousComment = 0
                };
                _context.Residents.Add(resident1);
                await _context.SaveChangesAsync();
                var chat = _context.Chats.Where(c => c.HostelId == id).SingleOrDefault();
                if (chat != null)
                {
                    var chatAdmin = _context.Participants.Where(p => p.ChatId == chat.Id && p.MemberId == hostel.LandlordId).SingleOrDefault();
                    if (chatAdmin == null)
                    {
                        Participant admin = new Participant
                        {
                            ChatId = chat.Id,
                            MemberId = hostel.LandlordId,
                            JoinedAt = DateTime.Now,
                            Role = 1,
                            AnonymousTime = 3,
                            NickName = landlord.FirstName + " " + landlord.LastName,
                            Status = 0

                        };
                        _context.Participants.Add(admin);
                    }
                    Participant participant = new Participant
                    {
                        ChatId = chat.Id,
                        MemberId = member.Id,
                        JoinedAt = DateTime.Now,
                        Role = 0,
                        AnonymousTime = 3,
                        NickName = member.FirstName + " " + member.LastName,
                        Status = 0

                    };
                    _context.Participants.Add(participant);
                    _context.SaveChanges();
                }
                else
                {
                    Chat newChat = new Chat
                    {
                        HostelId = id,
                        Name = hostel.Name + " group chat",
                        CreatedAt = DateTime.Now,
                        IsGroup = 1
                    };
                    _context.Chats.Add(newChat);
                    _context.SaveChanges();

                    Participant chatAdmin = new Participant
                    {
                        ChatId = newChat.Id,
                        MemberId = hostel.LandlordId,
                        JoinedAt = DateTime.Now,
                        Role = 1,
                        AnonymousTime = 3,
                        NickName = landlord.FirstName + " " + landlord.LastName,
                        Status = 0

                    };
                    _context.Participants.Add(chatAdmin);
                    Participant participant = new Participant
                    {
                        ChatId = newChat.Id,
                        MemberId = member.Id,
                        JoinedAt = DateTime.Now,
                        Role = 0,
                        AnonymousTime = 3,
                        NickName = member.FirstName + " " + member.LastName,
                        Status = 0

                    };
                    _context.Participants.Add(participant);
                    _context.SaveChanges();

                }

                return Ok(new
                {
                    IsSuccess = true,
                    IsInHostel = false
                });
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    IsSuccess = false,
                    IsInHostel = false,
                    Message = e.Message
                });
            }
        }
        double GetDistance(string lat, string lng, string lat2, string lng2)
        {
            string url = String.Format("https://maps.googleapis.com/maps/api/distancematrix/xml?units=imperial&origins={0},{1}&destinations={2},{3}&key=AIzaSyCdUxZNK9Gx72Hw_dCrIHuB3n_UAiTMIXw", lat, lng, lat2, lng2);
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            StreamReader reader = new StreamReader(data);
            string responseFromServer = reader.ReadToEnd();
            Console.WriteLine(responseFromServer);
            response.Close();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(responseFromServer);
            if (xmldoc.GetElementsByTagName("status")[0].ChildNodes[0].InnerText == "OK")
            {
                XmlNodeList distance = xmldoc.GetElementsByTagName("distance");
                return Convert.ToDouble(distance[0].ChildNodes[0].InnerText);
            }
            return 0;
        }
        private double DistanceTo(double lat1, double lon1, double lat2, double lon2)
        {
            try
            {
                double rlat1 = Math.PI * lat1 / 180;
                double rlat2 = Math.PI * lat2 / 180;
                double theta = lon1 - lon2;
                double rtheta = Math.PI * theta / 180;
                double dist =
                    Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                    Math.Cos(rlat2) * Math.Cos(rtheta);
                dist = Math.Acos(dist);
                dist = dist * 180 / Math.PI;
                dist = dist * 60 * 1.1515;

                return dist * 1609.344;
            }
            catch (Exception e)
            {
                return 0;
            }



        }
        private string replaceString(decimal price)
        {
            double result = (double)price;
            string result1 = "";
            if (result >= 1000 && result < 1000000)
            {
                result = result / 1000;
                result1 = result.ToString() + "K";
            }
            else if (price >= 1000000)
            {
                result = result / 1000000;
                result1 = result.ToString() + "M";
            }
            return result1;
        }
        private int CountResidentInRoom(Room room)
        {
            var residents = _context.Residents.Where(r => r.RoomId == room.Id).ToList();
            residents = residents.Where(r => r.Status == 1).ToList();
            return residents.Count();
        }
        [HttpPut("{id}/inspect")]
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDTO status)
        {
            try
            {
                var hostel = _context.Hostels.Where(h => h.Id == id).SingleOrDefault();
                if (hostel == null)
                {
                    return BadRequest("Hostel not exist");
                }
                else
                {
                    hostel.Status = status.Status;
                }

                _context.Hostels.Update(hostel);
                await _context.SaveChangesAsync();

                return Ok("Update inspect success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateHostel(int id, [FromBody] UpdateHostelDTO updateHostelDTO)
        {
            try
            {
                var hostel = await _context.Hostels.Where(h => h.Id == id).SingleOrDefaultAsync();
                if (hostel == null)
                {
                    return BadRequest("Hostel not exist");
                }
                else
                {
                    hostel.Name = updateHostelDTO.Name;
                    hostel.Price = updateHostelDTO.Price;
                    hostel.Capacity = updateHostelDTO.Capacity;
                    hostel.DetailLocation = updateHostelDTO.DetailLocation;
                    hostel.WardsCode = updateHostelDTO.WardsCode;
                    hostel.Phone = updateHostelDTO.Phone;
                    hostel.Description = updateHostelDTO.Description;
                    hostel.RoomArea = updateHostelDTO.RoomArea;
                    hostel.Electricity = updateHostelDTO.Electricity;
                    hostel.Water = updateHostelDTO.Water;
                    hostel.Internet = updateHostelDTO.Internet;
                }
                _context.Hostels.Update(hostel);
                await _context.SaveChangesAsync();
                var amenities = await _context.HostelAmenities.Where(h => h.HostedId == id).ToListAsync();
                foreach (var amenity in amenities)
                {
                    _context.HostelAmenities.Remove(amenity);
                }
                await _context.SaveChangesAsync();
                var amenityIdList = new List<string>(updateHostelDTO.Amenities.Trim().Split(' '));
                foreach (var amenityId in amenityIdList)
                {
                    var amenity = _context.Amenities.SingleOrDefault(a => a.Id == int.Parse(amenityId));
                    if (amenity != null)
                    {
                        HostelAmenity hostelAmenity = new HostelAmenity
                        {
                            HostedId = hostel.Id,
                            AmenitiesId = int.Parse(amenityId)

                        };
                        _context.HostelAmenities.Add(hostelAmenity);
                        await _context.SaveChangesAsync();
                    }
                }
                var images = await _context.HostelImages.Where(h => h.HostelId == id).ToListAsync();
                foreach (var image in updateHostelDTO.ImagesUrl.deleted)
                {
                    var deletedImage = images.Where(i => i.ImageUrl.Equals(image)).FirstOrDefault();
                    if (deletedImage != null)
                    {
                        _context.HostelImages.Remove(deletedImage);
                        await _context.SaveChangesAsync();
                    }
                }
                foreach (string image in updateHostelDTO.ImagesUrl.added)
                {
                    HostelImage hostelImage = new HostelImage
                    {
                        HostelId = hostel.Id,
                        ImageUrl = image

                    };
                    _context.HostelImages.Add(hostelImage);
                    _context.SaveChanges();
                }
                return Ok("Update hostel success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}/room")]
        public async Task<ActionResult> UpdateRoom(int id, [FromBody] UpdateRoomDTO updateRoomDTO)
        {
            try
            {
                var hostel = await _context.Hostels.Where(h => h.Id == id).SingleOrDefaultAsync();
                if (hostel == null)
                {
                    return BadRequest("Hostel not exist");
                }
                var room = await _context.Rooms.Where(r => r.Id == updateRoomDTO.RoomId).SingleOrDefaultAsync();
                if (room == null)
                {
                    return BadRequest("Room not exist");
                }
                if (updateRoomDTO.Name != null)
                {
                    room.Name = updateRoomDTO.Name;
                }
                if (updateRoomDTO.Price != 0)
                {
                    room.Price = updateRoomDTO.Price;
                }
                if (updateRoomDTO.RoomArea != 0)
                {
                    room.RoomArea = updateRoomDTO.RoomArea;
                }
                _context.Rooms.Update(room);
                await _context.SaveChangesAsync();
                return Ok("Update room name success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("{id}/room")]
        public async Task<ActionResult> AddNewRoom(int id, [FromBody] AddNewRoomDTO addNewRoomDTO)
        {
            try
            {
                var hostel = await _context.Hostels.Where(h => h.Id == id).SingleOrDefaultAsync();
                if (hostel == null)
                {
                    return BadRequest("Hostel not exist");
                }
                if (addNewRoomDTO.Name == null || addNewRoomDTO.Name == "")
                {
                    addNewRoomDTO.Name = hostel.Name + " " + "New room";
                }
                if (addNewRoomDTO.Price == 0)
                {
                    addNewRoomDTO.Price = hostel.Price;
                }
                if (addNewRoomDTO.RoomArea == 0)
                {
                    addNewRoomDTO.RoomArea = int.Parse(hostel.RoomArea);
                }
                Room room = new Room
                {
                    HostelId = hostel.Id,
                    Name = addNewRoomDTO.Name,
                    Price = addNewRoomDTO.Price,
                    RoomArea = addNewRoomDTO.RoomArea
                };
                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();
                return Ok("Add new room success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("{id}/rooms/change")]
        public async Task<ActionResult> ChangeRoom(int id, [FromBody] ChangeRoomDTO changeRoomDTO)
        {
            try
            {
                var hostel = await _context.Hostels.Where(h => h.Id == id).SingleOrDefaultAsync();
                var member = await _context.Members.Where(m => m.InviteCode.Trim().Equals(changeRoomDTO.InviteCode.Trim())).SingleOrDefaultAsync();
                if (hostel == null)
                {
                    return BadRequest("Hostel not exist");
                }
                if (member == null)
                {
                    return BadRequest("Member not exist");
                }
                var fromRoom = await _context.Rooms.Where(fr => fr.Id == changeRoomDTO.FromRoomId
                && fr.HostelId == id).SingleOrDefaultAsync();
                var toRoom = await _context.Rooms.Where(tr => tr.Id == changeRoomDTO.ToRoomId
                && tr.HostelId == id).SingleOrDefaultAsync();
                if (fromRoom == null)
                {
                    return BadRequest("From room not exist");
                }
                if (toRoom == null)
                {
                    return BadRequest("To room not exist");
                }

                if (CountResidentInRoom(toRoom) >= int.Parse(hostel.Capacity.Trim()))
                {
                    return BadRequest("To room is full");
                }
                var checkInToRoomExist = await _context.Residents.Where(r => r.MemberId == member.Id
                && r.RoomId == changeRoomDTO.ToRoomId && r.Status == 1).SingleOrDefaultAsync();
                if (checkInToRoomExist != null)
                {
                    return BadRequest("User is already in To Room");
                }

                var checkInFromRoomExist = await _context.Residents.Where(r => r.MemberId == member.Id
                && r.RoomId == changeRoomDTO.FromRoomId && r.Status == 1).SingleOrDefaultAsync();
                if (checkInFromRoomExist == null)
                {
                    return BadRequest("User is not in From Room");
                }
                Resident resident = new Resident
                {
                    HostelId = id,
                    MemberId = member.Id,
                    RoomId = changeRoomDTO.ToRoomId,
                    Status = 1,
                    Rate = checkInFromRoomExist.Rate,
                    Comment = checkInFromRoomExist.Comment,
                    CreatedAt = DateTime.Now
                };
                checkInFromRoomExist.Rate = 0;
                checkInFromRoomExist.Comment = "";
                checkInFromRoomExist.Status = 0;
                checkInFromRoomExist.LeftAt = DateTime.Now;
                _context.Residents.Update(checkInFromRoomExist);
                _context.Residents.Add(resident);
                await _context.SaveChangesAsync();
                return Ok("Change room success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpDelete("{id}/room")]
        public async Task<ActionResult> DeleteRoom(int id, [FromBody] DeleteRoomDTO deleteRoom)
        {
            var hostel = await _context.Hostels.Where(h => h.Id == id).SingleOrDefaultAsync();
            if (hostel == null)
            {
                return BadRequest("Hostel not exist");
            }
            var room = await _context.Rooms.Where(r => r.Id == deleteRoom.RoomId).SingleOrDefaultAsync();
            if (room == null)
            {
                return BadRequest("Room not exist");
            }
            if (!IsRoomInHostel(room, hostel))
            {
                return BadRequest("Room is not in hostel");
            }
            var residents = _context.Residents.Where(r => r.HostelId == id && r.RoomId == deleteRoom.RoomId).ToList();
            if (residents.Count > 0)
            {
                return BadRequest("Cannot delete this room");
            }
            _context.Rooms.Remove(room);
            if (_context.SaveChanges() > 0)
            {
                return Ok("Delete room successfully");
            }
            return BadRequest("Cannot delete this room");
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteHostel(int id)
        {
            var hostel = await _context.Hostels.Where(h => h.Id == id).SingleOrDefaultAsync();
            if (hostel == null)
            {
                return BadRequest("Hostel not exist");
            }
            hostel.Status = 4;
            _context.Hostels.Update(hostel);
            var residents = _context.Residents.Where(r => r.HostelId == id).ToList();
            if (residents.Count() > 0)
            {
                foreach (var resident in residents)
                {
                    if (resident.Status == 1)
                    {
                        resident.Status = 2;
                        resident.LeftAt = DateTime.Now;
                        _context.Residents.Update(resident);
                    }

                }
            }
            if (_context.SaveChanges() > 0)
            {
                return Ok("Delete hostel successfully");
            }
            return BadRequest("Cannot delete this hostel");
        }
        [HttpGet("{id}/other-cost")]
        public async Task<ActionResult> GetOtherCost(int id)
        {
            var hostel = await _context.Hostels.Where(h => h.Id == id).SingleOrDefaultAsync();
            if (hostel == null)
            {
                return BadRequest("Hostel not exist");
            }

            if (hostel.OtherCost != null)
            {

                string[] othercost = hostel.OtherCost.Split('|');
                for (int i = 0; i < othercost.Length; i++)
                {
                    othercost[i] = othercost[i].Trim();
                }
                return Ok(new
                {
                    OtherCost = othercost
                });
            }
            List<string> other = new List<string>();
            return Ok(new
            {
                OtherCost = new string[] { }
            });

        }
        [HttpPut("{id}/other-cost")]
        public async Task<ActionResult> UpdateOtherCost(int id, [FromBody] UpdateOtherCostDTO othercost)
        {
            var hostel = await _context.Hostels.Where(h => h.Id == id).SingleOrDefaultAsync();
            if (hostel == null)
            {
                return BadRequest("Hostel not exist");
            }
            hostel.OtherCost = "";
            foreach (var it in othercost.OtherCost)
            {
                hostel.OtherCost = hostel.OtherCost + it + " | ";
            }
            hostel.OtherCost = hostel.OtherCost == "" ? null : hostel.OtherCost.Substring(0, hostel.OtherCost.Length - 3);
            _context.Hostels.Update(hostel);
            if (_context.SaveChanges() > 0)
            {
                return Ok("Update hostel other cost successfully");
            }
            return BadRequest("Update failed");
        }
        private bool IsRoomInHostel(Room RoomCheck, Hostel HostelCheck)
        {
            bool RoomInHostel = false;
            var rooms = _context.Rooms.Where(r => r.HostelId == HostelCheck.Id).ToList();
            foreach (var room in rooms)
            {
                if (RoomCheck.Id == room.Id)
                {
                    RoomInHostel = true;
                }
            }
            return RoomInHostel;
        }
        [HttpPost("{id}/review")]
        public async Task<ActionResult> AddReview(int id, [FromBody] AddReviewDTO addReviewDTO)
        {
            try
            {
                var resident = await _context.Residents.Where(r => r.HostelId == id && r.MemberId == addReviewDTO.MemberId && r.Status == 1).SingleOrDefaultAsync();
                if (resident != null)
                {
                    resident.Comment = addReviewDTO.Comment;
                    resident.Rate = addReviewDTO.Rate;
                    resident.IsAnonymousComment = addReviewDTO.IsAnonymousComment;
                }
                _context.Residents.Update(resident);
                await _context.SaveChangesAsync();
                return Ok("Rate success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPut("{id}/review")]
        public async Task<ActionResult> UpdateReview(int id, [FromBody] AddReviewDTO addReviewDTO)
        {
            try
            {
                var resident = await _context.Residents.Where(r => r.HostelId == id && r.MemberId == addReviewDTO.MemberId && r.Status == 1).SingleOrDefaultAsync();
                if (resident != null)
                {
                    resident.Comment = addReviewDTO.Comment;
                    resident.Rate = addReviewDTO.Rate;
                    resident.IsAnonymousComment = addReviewDTO.IsAnonymousComment;
                }
                _context.Residents.Update(resident);
                await _context.SaveChangesAsync();
                return Ok("Rate success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("{hostelId}/member/{memberId}/check")]
        public async Task<IActionResult> CheckHostelResident(int hostelId, int memberId)
        {
            try
            {
                var hostel = await _context.Hostels.Where(h => h.Id == hostelId).SingleOrDefaultAsync();
                if (hostel == null)
                {
                    return BadRequest("Hostel not exist");
                }
                if (hostel.LandlordId == memberId)
                {
                    return Ok(false);
                }
                var residents = _context.Residents.Where(r => r.HostelId == hostelId).ToList();
                List<int> memberIds = new List<int>();
                foreach (var res in residents)
                {
                    memberIds.Add(res.MemberId);
                }
                if (memberIds.Contains(memberId))
                {
                    return Ok(true);
                }
                return Ok(false);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("suggest")]
        public async Task<IActionResult> SuggestHostel(
            [FromQuery] string provinceCode,
            [FromQuery] int newHostel,
            [FromQuery] int? page,
            [FromQuery] int? limit
            )
        {
            try
            {
                if (page == null || page <= 0)
                {
                    page = 1;
                }
                if (limit == null || limit <= 0)
                {
                    limit = 10;
                }
                if (provinceCode == null)
                {
                    return NotFound();
                }
                IQueryable<Hostel> hostels = from h
                                            in _context.Hostels
                                             .Include(h => h.HostelImages)
                                             .Include(h => h.HostelAmenities)
                                             .Include(h => h.WardsCodeNavigation)
                                            .ThenInclude(w => w.DistrictCodeNavigation)
                                            .Where(h => h.Status == 1)
                                             select h;
                var hostelAmenities = await _context.HostelAmenities.ToListAsync();
                hostels = hostels.Where(h => h.WardsCodeNavigation.DistrictCodeNavigation.ProvinceCode == provinceCode);
                if (newHostel == 1)
                {
                    hostels = hostels.OrderByDescending(h => h.CreatedAt);
                }
                PaginatedList<Hostel> hostelsPL = await PaginatedList<Hostel>.CreateAsync(hostels.AsNoTracking(), (int)page, (int)limit);
                List<SuggestHostelDTO> reponses = new List<SuggestHostelDTO>();
                foreach (var hostel in hostelsPL)
                {
                    string ImgUrl = "";
                    var hostelImg = _context.HostelImages.Where(h => h.HostelId == hostel.Id).FirstOrDefault();
                    if (hostelImg != null)
                    {
                        ImgUrl = hostelImg.ImageUrl;
                    }
                    List<AmenitiesGetHostelDTO> AmenitiesResult = new List<AmenitiesGetHostelDTO>();
                    foreach (var amenity in hostel.HostelAmenities)
                    {
                        var amen = await _context.Amenities.Where(a => a.Id == amenity.AmenitiesId).FirstOrDefaultAsync();
                        AmenitiesResult.Add(new AmenitiesGetHostelDTO
                        {
                            Id = amen.Id,
                            Icon = amen.Icon,
                            Name = amen.AmenitiyName
                        });
                    }
                    double totalRate = 0.0;
                    double Rate = 0.0;
                    int noRate = 0;
                    var reviews = _context.Residents.Where(r => r.HostelId == hostel.Id).ToList();
                    if (reviews.Count() > 0)
                    {
                        GetHostelReviewDTO review = new GetHostelReviewDTO();
                        foreach (var item in reviews)
                        {
                            if (CheckResidentChangedRoom(hostel.Id, item.MemberId))
                            {
                                if (item.Status != 1)
                                {
                                    continue;
                                }
                            }
                            totalRate += item.Rate;
                            if (item.Rate != 0)
                            {
                                noRate++;
                            }
                        }
                        if (noRate > 0)
                        {
                            Rate = totalRate / noRate;
                        }
                    }
                    reponses.Add(new SuggestHostelDTO
                    {
                        HostelId = hostel.Id,
                        Name = hostel.Name,
                        DetailLocation = hostel.DetailLocation,
                        WardName = hostel.WardsCodeNavigation.FullName,
                        DistrictName = hostel.WardsCodeNavigation.DistrictCodeNavigation.FullName,
                        Price = replaceString(hostel.Price),
                        ImgUrl = ImgUrl,
                        Amenities = AmenitiesResult.ToArray(),
                        Rate = Rate,
                        NoRate = noRate
                    })
                ;
                }
                return Ok(new
                {
                    Hostels = reponses.ToArray()
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}