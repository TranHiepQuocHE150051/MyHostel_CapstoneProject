using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHostel_BackEnd.Models;
using Quartz;

namespace MyHostel_BackEnd.Quartz
{
    public class QuartzJob : IJob
    {
        private MyHostelContext _context = new MyHostelContext();
        public async Task Execute(IJobExecutionContext context)
        {
            var transactions = await _context.Transactions.Include(t => t.Room).Where(t => t.PaidAt == null).ToListAsync();
            var registrationToken = "eoCu8IdWRZiP8StEZku0O7:APA91bGf_t2j0z4tEukJO8RMTfEyu9FpfxX6WI9Zqm0zdlk0x_fAGWERbgURnZ2pGAAyY5BXaA6gpGHCEJoyJhHnEiL6AtCIdZ_DH6PNVGqwULTgcwMHVVzGBkTOvI2ZR0IG_TNjn-dV";
            foreach (var transaction in transactions)
            {
                string[] others = transaction.Other.Split('-');
                var total = transaction.Rent
                    + transaction.Electricity
                    + transaction.Water
                    + transaction.Security
                    + transaction.Internet;
                foreach (var other in others)
                {
                    decimal price = Decimal.Parse(other.Split(':')[1]);
                }
                var message = new FirebaseAdmin.Messaging.Message()
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "Tiền cần đóng của phòng "+transaction.Room.Name+": ", total.ToString() }
                    },
                    Token = registrationToken,
                };


                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine("Successfully sent message: " + response);
            }
            return;
        }
    }
}
