using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHostel_BackEnd.DTOs;
using MyHostel_BackEnd.Models;
using Org.BouncyCastle.Utilities;

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
                            prices[i] = prices[i].Replace("m", "00000");
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
                    
                }
                if (nearbyFacilities != null)
                {

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
    }
}
