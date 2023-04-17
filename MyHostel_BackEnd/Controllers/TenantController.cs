using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHostel_BackEnd.DTOs;
using MyHostel_BackEnd.Models;

namespace MyHostel_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private IConfiguration _configuration;
        private MyHostelContext _context;
        public TenantController(IConfiguration configuration, MyHostelContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetRoomInfo(int id)
        {
            try
            {
                var tenant = _context.Members.Where(m => m.Id == id).SingleOrDefault();
                if(tenant == null)
                {
                    return BadRequest("Member not exist");
                } 
                var resident = _context.Residents.Where(r => r.MemberId == id && r.Status==1).SingleOrDefault();
                if (resident == null)
                {
                    return BadRequest("Member not in hostel");
                }
                var room = _context.Rooms.Where(r => r.Id == resident.RoomId).SingleOrDefault();
                if (room == null)
                {
                    return BadRequest("Room not exist");
                }
                var hostel = _context.Hostels.Where(h => h.Id == room.HostelId).Include(h=>h.WardsCodeNavigation).ThenInclude(w=>w.DistrictCodeNavigation).SingleOrDefault();
                if (hostel == null)
                {
                    return BadRequest("Room not exist");
                }
                var roomMember = _context.Residents.Where(r => r.RoomId == room.Id && r.Status == 1 && r.MemberId != id).ToList();
                RoomInfoDTO result = new RoomInfoDTO
                {
                    RoomId = room.Id,
                    Name = room.Name,
                    Price =  string.Format("{0:0.##}", replaceString(room.Price.Value)),
                    Area = (int)room.RoomArea,
                    Roommate = new List<object>(),
                    Hostel = new
                    {
                        Name= hostel.Name,
                        DetailLocation = hostel.DetailLocation,
                        WardName= hostel.WardsCodeNavigation.FullName,
                        DistrictName=hostel.WardsCodeNavigation.DistrictCodeNavigation.FullName
                    }
                };
                if (roomMember.Any())
                {
                    foreach (var member in roomMember)
                    {
                        var roomate = _context.Members.Where(m => m.Id == member.MemberId).SingleOrDefault();
                        result.Roommate.Add(new
                        {
                            MemberId = roomate.Id,
                            FullName = roomate.FirstName + " " + roomate.LastName,
                            Avatar = roomate.Avatar

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
        [HttpGet("{id}/unpaid")]
        public async Task<ActionResult> GetUnpaidBill(int id)
        {
            try
            {
                var tenant = _context.Members.Where(m => m.Id == id).SingleOrDefault();
                if (tenant == null)
                {
                    return BadRequest("Member not exist");
                }
                var resident = _context.Residents.Where(r => r.MemberId == id && r.Status == 1).SingleOrDefault();
                if (resident == null)
                {
                    return BadRequest("Member not in hostel");
                }
                var room = _context.Rooms.Where(r => r.Id == resident.RoomId).SingleOrDefault();
                if (room == null)
                {
                    return BadRequest("Room not exist");
                }
                
                var transactions = _context.Transactions.Where(t => t.RoomId == room.Id && t.Status != 2 && t.Status != 3).ToList();
                List<UnpaidBillDTO> result = new List<UnpaidBillDTO>();
                if (transactions.Any())
                {
                    foreach(var transaction in transactions)
                    {
                        var otherCost = new List<OtherCostDTO>();
                        string[] others = transaction.Other != null && transaction.Other.Trim() != "" ? transaction.Other.Trim().Split('-') : null;
                        decimal total = 0;
                        if (transaction.Rent != null)
                        {
                            total += (decimal)transaction.Rent;
                        }
                        if (transaction.Electricity != null)
                        {
                            total += (decimal)transaction.Electricity;
                        }
                        if (transaction.Water != null)
                        {
                            total += (decimal)transaction.Water;
                        }
                        if (transaction.Internet != null)
                        {
                            total += (decimal)transaction.Internet;
                        }
                        if (others != null)
                        {
                            foreach (var other in others)
                            {
                                decimal price = decimal.Parse(other.Split(':')[1]);
                                total += price;
                                otherCost.Add(new OtherCostDTO
                                {
                                    Name = other.Split(':')[0],
                                    Cost = price
                                });
                            }
                        }
                        if (transaction.PaidAmount == null)
                        {
                            transaction.PaidAmount = 0;
                        }
                        decimal pay = (decimal)(total - transaction.PaidAmount);
                        result.Add(new UnpaidBillDTO
                        {
                            Id = transaction.Id,
                            AtTime = transaction.AtTime,
                            Rent = transaction.Rent == null ? 0 : transaction.Rent.Value,
                            Electricity = transaction.Electricity == null ? 0 : transaction.Electricity.Value,
                            Water = transaction.Water == null ? 0 : transaction.Water.Value,
                            Internet = transaction.Internet == null ? 0 : transaction.Internet.Value,
                            OtherCost = otherCost,
                            PaidAmount = transaction.PaidAmount == null ? 0 : transaction.PaidAmount.Value,
                            NeedToPay = pay
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
    }
}
