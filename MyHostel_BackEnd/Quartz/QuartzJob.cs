﻿using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHostel_BackEnd.Models;
using Quartz;
using System.Globalization;

namespace MyHostel_BackEnd.Quartz
{
    public class QuartzJob : IJob
    {
        private MyHostelContext _context = new MyHostelContext();
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Start sent message ");
            var transactions = await _context.Transactions.Include(t => t.Room).Where(t => t.Status != 2).ToListAsync();
            foreach (var transaction in transactions)
            {
                var residents = await _context.Residents.Where(r => r.RoomId == transaction.RoomId && r.Status == 1).ToListAsync();

                string[] others = transaction.Other != "" ? transaction.Other.Split('-') : null;
                decimal total = (decimal)(transaction.Rent
                    + transaction.Electricity
                    + transaction.Water
                    + transaction.Internet);
                if (others != null && others.Length > 0)
                {
                    foreach (var other in others)
                    {
                        decimal price = decimal.Parse(other.Split(':')[1]);
                        total += price;
                    }
                }
                foreach (var resident in residents)
                {

                    var member = _context.Members.Where(m => m.Id == resident.MemberId).SingleOrDefault();
                    var registrationToken = member.FcmToken;
                    if (registrationToken != null && registrationToken != "")
                    {
                        var message = new FirebaseAdmin.Messaging.Message()
                        {
                            Data = new Dictionary<string, string>()
                    {
                        { "Tiền cần đóng của "+transaction.Room.Name+": ", total.ToString("N0", new CultureInfo("vn-VN")) + "VNĐ" }
                    },
                            Token = registrationToken,
                        };
                        string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);

                    }
                    //Console.WriteLine("Successfully sent message: " + response);
                }

            }
            return;
        }
    }
}
