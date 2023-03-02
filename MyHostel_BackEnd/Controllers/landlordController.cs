using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHostel_BackEnd.DTOs;
using MyHostel_BackEnd.Models;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

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

        }

        private void CheckNearbyAmenity(string latitude, string longitude , int hostelid)
        {
            var facilities = _context.Facilities.ToList();
            foreach (var facility in facilities)
            {
                string URL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json";
                string urlParameters = "?location=" + latitude + "%2C" + longitude
                    + "&radius=2000&type=" + facility.UtilityName.ToString()
                    + "&key=" + GlobalVariables.API_KEY;
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(urlParameters).Result;  
                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadFromJsonAsync<IEnumerable<DataObject>>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                    foreach (var d in dataObjects)
                    {
                        
                    }

                    NearbyFacility nearbyFacility = new NearbyFacility
                    {
                        UltilityId= facility.Id,
                        HostelId = hostelid
                        
                    };
                }
            }
        }
    }
}
