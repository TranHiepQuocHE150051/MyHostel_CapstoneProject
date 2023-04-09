﻿using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("{id}")]
        public async Task<ActionResult> GetTransactionForRoom(int id, [FromQuery] int? month, [FromQuery] int? year)
        {
            try
            {
                var transactions = await _context.Transactions.Where(t => t.RoomId == id).ToListAsync();
                if (month != null)
                {
                    transactions = transactions.Where(t => t.CreatedAt.Value.Month == month).ToList();
                }
                if (year != null)
                {
                    transactions = transactions.Where(t => t.CreatedAt.Value.Year == year).ToList();
                }
                var result = new List<GetTransactionForRoomDTO>();
                if (transactions.Any())
                {
                    foreach (var transaction in transactions)
                    {
                        var otherCost = new List<OtherCostDTO>();
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
                        result.Add(new GetTransactionForRoomDTO
                        {
                            Id = transaction.Id,
                            RoomId = transaction.RoomId,
                            Rent = transaction.Rent,
                            Electricity = transaction.Electricity,
                            Water = transaction.Water,
                            Internet = transaction.Internet,
                            IsPaid = transaction.PaidAt == null ? false : true,
                            Total = total,
                            OtherCost = otherCost.ToArray()
                        });
                    }
                }
                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
