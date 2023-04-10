using Quartz;
using MyHostel_BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin.Messaging;

namespace MyHostel_BackEnd.Quartz
{
    public class SendAtHourJob : IJob
    {
        private MyHostelContext _context = new MyHostelContext();
        public async Task Execute(IJobExecutionContext context)
        {
            var notifications = await _context.Notifications.ToListAsync();
            foreach (var notification in notifications)
            {
                if (notification.Type == 1 
                    && notification.SendAtHour == DateTime.Now.Hour 
                    && notification.CreateAt.Value.Date == DateTime.Now.Date)
                {
                    var registrationToken = "eoCu8IdWRZiP8StEZku0O7:APA91bGf_t2j0z4tEukJO8RMTfEyu9FpfxX6WI9Zqm0zdlk0x_fAGWERbgURnZ2pGAAyY5BXaA6gpGHCEJoyJhHnEiL6AtCIdZ_DH6PNVGqwULTgcwMHVVzGBkTOvI2ZR0IG_TNjn-dV";
                    var data = notification.Message.Split(':');
                    var message = new FirebaseAdmin.Messaging.Message()
                    {
                        Data = new Dictionary<string, string>()
                    {
                        { data[0]+": ", data[1] }
                    },
                        Token = registrationToken,
                    };
                    string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                    Console.WriteLine("Successfully sent message: " + response);
                }
            }
            return;
        }
    }
}
