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
                    checkTime = checkTime + "0" + month + "/" + year;
                }
                if (month > 10)
                {
                    checkTime = checkTime + month + "/" + year;
                }

                var transaction = _context.Transactions.Where(t => t.RoomId == id && t.AtTime.Equals(checkTime)).FirstOrDefault();
                if (transaction != null)
                {
                    var residents = _context.Residents.Where(r => r.RoomId == id).ToList();
                    var otherCost = new List<OtherCostDTO>();
                    var residentlist = new List<ResidentDTO>();
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
                    return Ok(new GetTransactionForRoomDTO
                    {
                        Id = transaction.Id,
                        RoomId = transaction.RoomId,
                        Rent = transaction.Rent,
                        Electricity = transaction.Electricity,
                        Water = transaction.Water,
                        Internet = transaction.Internet,
                        Residents = residentlist.ToArray(),
                        PaidAmount = transaction.PaidAmount,
                        Status = transaction.Status,
                        IsPaid = transaction.Status == 0 ? false : true,
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
        [HttpPost("{id}/transaction")]
        public async Task<ActionResult> AddTransaction(int id, [FromBody] AddTransactionRequest transaction)
        {
            try
            {
                var room = _context.Rooms.Where(r => r.Id == id).SingleOrDefault();
                if (room == null)
                {
                    return BadRequest("Room not exist");
                }
                string other = "";
                if (transaction.Other != null)
                {
                    foreach (var it in transaction.Other)
                    {
                        other = other + it + " - ";
                    }
                    other = other.Substring(0, other.Length - 3);
                }
                Transaction transaction1 = new Transaction
                {
                    RoomId = id,
                    Electricity = transaction.Electricity,
                    Water = transaction.Water,
                    Internet = transaction.Internet,
                    Rent = transaction.Rent,
                    AtTime = transaction.AtTime,
                    Other = other,
                    CreatedAt = DateTime.Now,
                    Status=0,
                    PaidAmount=0
                };
                _context.Transactions.Add(transaction1);
                var residents = _context.Residents.Where(r => r.RoomId == id && r.Status == 1).ToList();
                var total = CalculateTotalMoney(transaction1);
                foreach (var res in residents)
                {
                    Notification notification = new Notification()
                    {
                        SendTo = res.MemberId,
                        SendAt = DateTime.Now,
                        CreateAt = DateTime.Now,
                        SendAtHour = DateTime.Now.Hour,
                        Type = 0,
                        Message = "Tiền cần đóng của " + room.Name + ": " +
                        total.ToString()
                    };
                    _context.Notifications.Add(notification);
                }
                _context.SaveChanges();
                //Send notification here
                return Ok("Add new transaction success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        private decimal CalculateTotalMoney(Transaction transaction)
        {
            string[] others = transaction.Other != "" ? transaction.Other.Split('-') : null;
            decimal total = (decimal)(transaction.Rent
                + transaction.Electricity
                + transaction.Water
                + transaction.Internet);
            if(others != null && others.Length > 0)
            {
                foreach (var other in others)
                {
                    decimal price = decimal.Parse(other.Split(':')[1]);
                    total += price;
                }
            }
            return total;
        }
        [HttpPut("{id}/transaction")]
        public async Task<ActionResult> UpdateTransaction(int id, [FromBody] UpdateTransactionDTO updateTransactionDTO)
        {             
            try
            {
                var room = _context.Rooms.Where(r => r.Id == id).SingleOrDefault();
                if (room == null)
                {
                    return BadRequest("Room not exist");
                }
                var transaction = _context.Transactions.Where(t => t.Id == updateTransactionDTO.Id).SingleOrDefault();
                if (transaction == null)
                {
                    return BadRequest("Transaction not exist");
                }
                if (transaction.RoomId != id)
                {
                    return BadRequest("Transaction not in this room");
                }
                string other = "";
                if(updateTransactionDTO.Other != null)
                {
                    foreach (var it in updateTransactionDTO.Other)
                    {
                        other = other + it + " - ";
                    }
                    other = other.Substring(0, other.Length - 3);
                    transaction.Other = other;
                }
                if (updateTransactionDTO.Electricity >= 0)
                {
                    transaction.Electricity = updateTransactionDTO.Electricity;
                }
                if (updateTransactionDTO.Internet >= 0)
                {
                    transaction.Internet = updateTransactionDTO.Internet;
                }
                if (updateTransactionDTO.Water >= 0)
                {
                    transaction.Water = updateTransactionDTO.Water;
                }
                if (updateTransactionDTO.Rent >= 0)
                {
                    transaction.Rent = updateTransactionDTO.Rent;
                }
                
                transaction.PaidAmount=   updateTransactionDTO.PaidAmount;
                transaction.PaidAt = DateTime.Now;
                
                var residents = _context.Residents.Where(r => r.RoomId == id && r.Status == 1).ToList();
                var total = CalculateTotalMoney(transaction);
                if (total > transaction.PaidAmount)
                {
                    transaction.Status = 1;
                    _context.Transactions.Update(transaction);
                }
                else                   
                {
                    transaction.Status = 2;
                    _context.Transactions.Update(transaction);
                }
                var moneyToPay = total - updateTransactionDTO.PaidAmount;
                foreach (var res in residents)
                {
                    Notification notification = new Notification()
                    {
                        SendTo = res.MemberId,
                        SendAt = DateTime.Now,
                        CreateAt = DateTime.Now,
                        SendAtHour = DateTime.Now.Hour,
                        Type = 0,
                        Message = "Tiền cần đóng của " + room.Name + ": " +
                        moneyToPay.ToString()
                    };
                    _context.Notifications.Add(notification);
                }
                _context.SaveChanges();
                //Send notification here
                return Ok("Update transaction success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPut("{id}/transaction/confirm")]
        public async Task<ActionResult> ConfirmTransaction(int id, [FromBody] ConfirmTransactionDTO confirmTransactionDTO)
        {
            try
            {
                var room = _context.Rooms.Where(r => r.Id == id).SingleOrDefault();
                if (room == null)
                {
                    return BadRequest("Room not exist");
                }
                var transaction = _context.Transactions.Where(t => t.Id == confirmTransactionDTO.Id).SingleOrDefault();
                if (transaction == null)
                {
                    return BadRequest("Transaction not exist");
                }
                if(transaction.RoomId != id)
                {
                    return BadRequest("Transaction not in this room");
                }
                transaction.PaidAt = DateTime.Now;
                _context.Transactions.Update(transaction);
                var residents = _context.Residents.Where(r => r.RoomId == id && r.Status == 1).ToList();
                var total = CalculateTotalMoney(transaction);
                foreach (var res in residents)
                {
                    Notification notification = new Notification()
                    {
                        SendTo = res.MemberId,
                        SendAt = DateTime.Now,
                        CreateAt = DateTime.Now,
                        SendAtHour = DateTime.Now.Hour,
                        Type = 0,
                        Message = room.Name + " đã thanh toán hóa đơn"
                    };
                    _context.Notifications.Add(notification);
                }
                _context.SaveChanges();
                //Send notification here
                return Ok("Confirm transaction success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
