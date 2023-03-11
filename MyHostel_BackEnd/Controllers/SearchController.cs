using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHostel_BackEnd.DTOs;
using MyHostel_BackEnd.Models;
using System.Security.Permissions;

namespace MyHostel_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : Controller
    {
        private IConfiguration _configuration;
        private MyHostelContext _context;
        private GlobalVariables _globalVariables;
        public SearchController(IConfiguration configuration, MyHostelContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpGet("provinces")]
        public async Task<IActionResult> searchProvince([FromQuery] string? name)
        {
            try
            {
                var provinces = await _context.Provinces.ToListAsync();
                if (name != null)
                {
                    provinces = provinces.Where(
                        p => p.Name.ToLower().Contains(name.ToLower()) 
                        || p.NameEn.ToLower().Contains(name.ToLower())
                        || p.FullName.ToLower().Contains(name.ToLower())
                        || p.FullNameEn.ToLower().Contains(name.ToLower())
                    ).ToList();
                }
                List<Provinces_District_Ward_Response> result = new List<Provinces_District_Ward_Response>();
                foreach (var province in provinces)
                { 
                    result.Add(new Provinces_District_Ward_Response()
                    {
                        Code = province.Code,
                        Name = province.Name
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
        [HttpGet("districts")]
        public async Task<IActionResult> searchDistrict([FromQuery] string? name, [FromQuery] string? province_code)
        {
            try
            {
                var districts = await _context.Districts.ToListAsync();
                if (province_code != null)
                {
                    districts = districts.Where(p => p.ProvinceCode.Equals(province_code)).ToList();
                }
                if (name != null)
                {
                    districts = districts.Where(
                        p => p.Name.ToLower().Contains(name.ToLower())
                        || p.NameEn.ToLower().Contains(name.ToLower())
                        || p.FullName.ToLower().Contains(name.ToLower())
                        || p.FullNameEn.ToLower().Contains(name.ToLower())
                    ).ToList();
                }
                List<Provinces_District_Ward_Response> result = new List<Provinces_District_Ward_Response>();
                foreach (var district in districts)
                {
                    result.Add(new Provinces_District_Ward_Response()
                    {
                        Code = district.Code,
                        Name = district.Name
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
        [HttpGet("wards")]
        public async Task<IActionResult> searchWard([FromQuery] string? name, [FromQuery] string? district_code)
        {
            try
            {
                var wards = await _context.Wards.ToListAsync();
                if (district_code != null)
                {
                    wards = wards.Where(p => p.DistrictCode.Equals(district_code)).ToList();
                }
                if (name != null)
                {
                    wards = wards.Where(
                        p => p.Name.ToLower().Contains(name.ToLower())
                        || p.NameEn.ToLower().Contains(name.ToLower())
                        || p.FullName.ToLower().Contains(name.ToLower())
                        || p.FullNameEn.ToLower().Contains(name.ToLower())
                    ).ToList();
                }
                List<Provinces_District_Ward_Response> result = new List<Provinces_District_Ward_Response>();
                foreach (var ward in wards)
                {
                    result.Add(new Provinces_District_Ward_Response()
                    {
                        Code = ward.Code,
                        Name = ward.Name
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
        [HttpGet("dw")]
        public async Task<IActionResult> searchDW([FromQuery] string? name, [FromQuery] string? province_code)
        {
            try
            {
                var districts = await _context.Districts.ToListAsync();
                if (province_code != null)
                {
                    districts = districts.Where(p => p.ProvinceCode.Equals(province_code)).ToList();
                }
                var districtCode = districts.Select(d => d.Code).ToList();
                var wards = await _context.Wards.Where(p => districtCode.Contains(p.DistrictCode) && (p.Name.ToLower().Contains(name.ToLower())
                        || p.NameEn.ToLower().Contains(name.ToLower()))).Take(3).ToListAsync();


                if (name != null)
                {
                    districts = districts.Where(
                        p => p.Name.ToLower().Contains(name.ToLower())
                        || p.NameEn.ToLower().Contains(name.ToLower())).Take(3).ToList();
                }
                List<Provinces_District_Ward_Response> result = new List<Provinces_District_Ward_Response>();
                foreach (var district in districts)
                {
                    result.Add(new Provinces_District_Ward_Response()
                    {
                        Code = district.Code,
                        Name = district.FullName
                    });
                }
                foreach (var ward in wards)
                {
                    var district = _context.Districts.FirstOrDefault(d => d.Code == ward.DistrictCode);
                    result.Add(new Provinces_District_Ward_Response()
                    {
                        Code = ward.Code,
                        Name = ward.FullName + ", " + district.FullName
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
    }
}
