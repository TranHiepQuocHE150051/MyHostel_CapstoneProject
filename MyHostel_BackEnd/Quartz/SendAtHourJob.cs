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
                    var member= _context.Members.Where(m=>m.Id==notification.SendTo).FirstOrDefault();
                    var registrationToken = member.FcmToken;
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
