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
using System.Xml;
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
                    string URL = "";
                    if (facility.Id==1)
                    {
                         URL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?keyword=Chợ&location=" + latitude + "," + longitude + "&radius=1500"+ "&key=AIzaSyDgE-j9prihJMmwRqEdjIv8ZdBHYTfOsU4";
                    }
                    else
                    {
                        URL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?&location=" + latitude + "," + longitude + "&radius=1500&type=" + facility.Code + "&key=AIzaSyDgE-j9prihJMmwRqEdjIv8ZdBHYTfOsU4";
                    }
                    WebRequest request = WebRequest.Create(URL);
                    WebResponse response = request.GetResponse();
                    Stream data = response.GetResponseStream();
                    StreamReader reader = new StreamReader(data);
                    string responseFromServer = reader.ReadToEnd();
                    if (responseFromServer != null)
                    {
                        var results = JsonSerializer.Deserialize<PlacesNearbySearchResponse>(responseFromServer);
                        if (results.results.Length > 0)
                        {
                            foreach(Place nearByResult in results.results)
                            {
                                Console.WriteLine(nearByResult);
                                double reslat = nearByResult.geometry.location.lat;
                                double reslng = nearByResult.geometry.location.lng;
                                double orglat = Double.Parse(latitude.Replace(".", ","));
                                double orglng = Double.Parse(longitude.Replace(".", ","));
                                DistanceAndDuration distanceAndDuration = DistanceTo(orglat, orglng, reslat, reslng);
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
                            //Place nearByResult = results.results.First();
                        }
                    }
                }
            }catch(Exception e)
            {
                return;
            }            
        }
        private DistanceAndDuration DistanceTo(double lat1, double lon1, double lat2, double lon2)
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
                dist = dist * 1.609344;
                double duration = dist * 15;
                return new DistanceAndDuration
                {
                    Distance = (decimal)dist,
                    Duration = (decimal)duration
                };
        }
        double GetDistance(string lat, string lng, string lat2, string lng2)
        {
            string url = String.Format("https://maps.googleapis.com/maps/api/distancematrix/xml?units=imperial&origins={0},{1}&destinations={2},{3}&key=AIzaSyDgE-j9prihJMmwRqEdjIv8ZdBHYTfOsU4", lat, lng, lat2, lng2);
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
    }
}
