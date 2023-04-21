using GoogleApi.Entities.Maps.Common;
using GoogleApi.Entities.Maps.Directions.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyHostel_Admin.Models;
using MyHostel_Admin.RequestObject;
using MyHostel_BackEnd.DTOs;
using MyHostel_BackEnd.GoogleMapsResponseObject;
using System.Net;
using System.Text.Json;
using Place = MyHostel_BackEnd.GoogleMapsResponseObject.Place;

namespace MyHostel_Admin.Pages.Admin_Page.Hostels
{
    public class ApproveModel : PageModel
    {
        private readonly MyHostelContext _context;

        public ApproveModel(MyHostelContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Hostel Hostel { get; set; }

        [BindProperty]
        public ApproveRequest approveRequest { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (HttpContext.Session.GetString("currentUser") == null)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                String currentUser = HttpContext.Session.GetString("currentUser");
                Admin user = _context.Admins.SingleOrDefault(p => p.AccountName.ToLower().Equals(currentUser.ToLower()));
                if (user == null)
                {
                    return RedirectToPage("/Index");
                }
                else
                {
                    if (user.RoleId != 3)
                    {
                        return RedirectToPage("/Index");
                    }
                }
            }
            if (id == null)
            {
                return NotFound();
            }

            Hostel = await _context.Hostels.FirstOrDefaultAsync(m => m.Id == id);

            if (Hostel == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(ApproveRequest request)
        {

            if (approveRequest.HostelId == null)
            {
                return NotFound();
            }

            Hostel = await _context.Hostels.FindAsync(approveRequest.HostelId);

            if (Hostel != null)
            {
                Hostel.Status = 1;
                if (approveRequest.Lat != null && approveRequest.Lng != null)
                {
                    Hostel.GoogleLocationLat = approveRequest.Lat;
                    Hostel.GoogleLocationLnd = approveRequest.Lng;
                    CheckNearbyAmenity(approveRequest.Lat, approveRequest.Lng, approveRequest.HostelId);
                }
                _context.Hostels.Update(Hostel);
                await _context.SaveChangesAsync();
                Chat chat = new Chat
                {
                    HostelId = approveRequest.HostelId,
                    Name = Hostel.Name+" group chat",
                    CreatedAt = DateTime.Now,
                    IsGroup=1

                };
                _context.Chats.Add(chat);
                _context.SaveChanges();
                var member = _context.Members.Where(m => m.Id == Hostel.LandlordId).SingleOrDefault();
                Participant lanlord = new Participant
                {
                    ChatId = chat.Id,
                    MemberId = Hostel.LandlordId,
                    JoinedAt = DateTime.Now,
                    Role = 1,
                    AnonymousTime = 3,
                    NickName = member.FirstName + " " + member.LastName
                };
                _context.Participants.Add(lanlord);
                _context.SaveChanges();
            }

            return RedirectToPage("/Admin_Page/Hostels/Index");
        }
        private void CheckNearbyAmenity(string latitude, string longitude, int hostelid)
        {
            try
            {
                var facilities = _context.Facilities.ToList();
                foreach (var facility in facilities)
                {
                    string URL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?&location=" + latitude + "," + longitude + "&radius=3000&type=" + facility.UtilityName + "&key=AIzaSyDgE-j9prihJMmwRqEdjIv8ZdBHYTfOsU4";
                    WebRequest request = WebRequest.Create(URL);
                    WebResponse response = request.GetResponse();
                    Stream data = response.GetResponseStream();

                    StreamReader reader = new StreamReader(data);
                    string responseFromServer = reader.ReadToEnd();
                    Console.WriteLine(responseFromServer);
                    if (responseFromServer != null)
                    {
                        var results = JsonSerializer.Deserialize<PlacesNearbySearchResponse>(responseFromServer);
                        if (results.results.Length > 0)
                        {
                            Place nearByResult = results.results.First();
                            Console.WriteLine(nearByResult);
                            double reslat = nearByResult.geometry.location.lat;
                            double reslng = nearByResult.geometry.location.lng;
                            double orglat = Double.Parse(latitude);
                            double orglng = Double.Parse(longitude);
                            DistanceAndDuration distanceAndDuration = CalculateDistanceAndDuration(orglng, orglat, reslng, reslat);
                            NearbyFacility nearbyFacility = new NearbyFacility
                            {
                                UltilityId = facility.Id,
                                HostelId = hostelid,
                                Name = nearByResult.name,
                                Distance = distanceAndDuration.Distance,
                                Duration = distanceAndDuration.Duration

                            };
                            _context.NearbyFacilities.Add(nearbyFacility);
                            _context.SaveChanges();
                        }
                    }
                }
            }catch(Exception e)
            {
                return;
            }
            
        }
        private DistanceAndDuration CalculateDistanceAndDuration(double longitude, double latitude, double otherLongitude, double otherLatitude)
        {
            Random random = new Random();

            var distance = random.Next(200, 3000);
            var duration = (distance/40);
            return new DistanceAndDuration
            {
                Distance = distance,
                Duration = duration
            };
        }
        private DistanceAndDuration CalculateDistanceAndDuration2(double orglat, double orglng, double deslat, double deslng)
        {
            DirectionsRequest request = new DirectionsRequest();

            request.Key = GlobalVariables.API_KEY;

            request.Origin = new LocationEx(new CoordinateEx(orglat, orglng));
            request.Destination = new LocationEx(new CoordinateEx(deslat, deslng));
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
