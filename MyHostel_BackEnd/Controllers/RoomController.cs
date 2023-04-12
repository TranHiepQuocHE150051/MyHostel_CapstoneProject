using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHostel_BackEnd.DTOs;
using MyHostel_BackEnd.Models;

namespace MyHostel_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : Controller
    {
        private IConfiguration _configuration;
        private MyHostelContext _context;
        public RoomController(IConfiguration configuration, MyHostelContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpGet("{id}/transaction")]
        public async Task<ActionResult> GetTransactionForRoom(int id, [FromQuery] int? month, [FromQuery] int? year)
        {
            try
            {
                string checkTime = "";
                if (month >= 1 && month < 10)
                {
                    checkTime = checkTime +"0"+ month + "/" + year;
                }
                if (month > 10)
                {
                    checkTime = checkTime + month + "/" + year;
                }
                
                var transaction =  _context.Transactions.Where(t => t.RoomId == id && t.AtTime.Equals(checkTime)).FirstOrDefault();
                if (transaction!=null)
                {                 
                        var residents = _context.Residents.Where(r => r.RoomId == id).ToList();                       
                        var otherCost = new List<OtherCostDTO>();
                        var residentlist = new List<ResidentDTO>();
                        string[] others = transaction.Other != null && transaction.Other.Trim() != "" ? transaction.Other.Trim().Split('-') : null;
                        Decimal total = 0;
                        if(transaction.Rent != null)
                        {
                            total += (Decimal)transaction.Rent;
                        }
                        if(transaction.Electricity != null)
                        {
                            total += (Decimal)transaction.Electricity;
                        }
                        if(transaction.Water != null)
                        {
                            total += (Decimal)transaction.Water;
                        }
                        if(transaction.Internet != null)
                        {
                            total += (Decimal)transaction.Internet;
                        }
                        if (others != null)
                        {
                            foreach (var other in others)
                            {
                                decimal price = Decimal.Parse(other.Split(':')[1]);
                                total += price;
                                otherCost.Add(new OtherCostDTO
                                {
                                    Name = other.Split(':')[0],
                                    Cost = price
                                });
                            }
                        }
                        if (residents.Count > 0)
                        {
                            foreach(var res in residents)
                            {
                            if (res.CreatedAt.Year > year)
                            {                               
                                continue;
                            }
                            if (res.CreatedAt.Year == year && res.CreatedAt.Month> month)
                            {
                                continue;
                            }
                            if (res.LeftAt.HasValue)
                            {                      
                                if(res.LeftAt.Value.Year< year)
                                {
                                    continue;
                                }
                                if(res.LeftAt.Value.Year == year && res.LeftAt.Value.Month < month)
                                {
                                    continue;
                                }
                                
                            }
                            var member = _context.Members.Where(m => m.Id == res.MemberId).FirstOrDefault();
                                if (member != null)
                                {
                                    residentlist.Add(new ResidentDTO
                                    {
                                        MemberId = member.Id,
                                        FullName = member.FirstName + " " + member.LastName,
                                        Avatar = member.Avatar
                                    });
                                }
                                
                            }
                        }                     
                    return Ok(new GetTransactionForRoomDTO
                    {
                        Id = transaction.Id,
                        RoomId = transaction.RoomId,
                        Rent = transaction.Rent,
                        Electricity = transaction.Electricity,
                        Water = transaction.Water,
                        Internet = transaction.Internet,
                        Residents = residentlist.ToArray(),
                        IsPaid = transaction.PaidAt == null ? false : true,
                        Total = total,
                        OtherCost = otherCost.ToArray()
                    });
                }
                else
                {
                    var residents = _context.Residents.Where(r => r.RoomId == id).ToList();
                   
                    var residentlist = new List<ResidentDTO>();
                    if (residents.Count > 0)
                    {
                        foreach (var res in residents)
                        {
                            if (res.CreatedAt.Year > year)
                            {
                                continue;
                            }
                            if (res.CreatedAt.Year == year && res.CreatedAt.Month > month)
                            {
                                continue;
                            }
                            if (res.LeftAt.HasValue)
                            {
                                if (res.LeftAt.Value.Year < year)
                                {
                                    continue;
                                }
                                if (res.LeftAt.Value.Year == year && res.LeftAt.Value.Month < month)
                                {
                                    continue;
                                }

                            }
                            var member = _context.Members.Where(m => m.Id == res.MemberId).FirstOrDefault();
                            if (member != null)
                            {
                                residentlist.Add(new ResidentDTO
                                {
                                    MemberId = member.Id,
                                    FullName = member.FirstName + " " + member.LastName,
                                    Avatar = member.Avatar
                                });
                            }

                        }
                    }
                    return Ok(new 
                    {                       
                       RoomId = id,
                       Residents = residentlist.ToArray()
                    });
                }                
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
