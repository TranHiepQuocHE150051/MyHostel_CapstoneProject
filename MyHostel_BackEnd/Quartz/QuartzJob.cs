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
            Console.WriteLine("Start sent message ");
            var transactions = await _context.Transactions.Include(t => t.Room).Where(t => t.Status != 2).ToListAsync();
            var registrationToken = "eoCu8IdWRZiP8StEZku0O7:APA91bGf_t2j0z4tEukJO8RMTfEyu9FpfxX6WI9Zqm0zdlk0x_fAGWERbgURnZ2pGAAyY5BXaA6gpGHCEJoyJhHnEiL6AtCIdZ_DH6PNVGqwULTgcwMHVVzGBkTOvI2ZR0IG_TNjn-dV";
            foreach (var transaction in transactions)
            {
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
                var message = new FirebaseAdmin.Messaging.Message()
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "Tiền cần đóng của "+transaction.Room.Name+": ", total.ToString() }
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
