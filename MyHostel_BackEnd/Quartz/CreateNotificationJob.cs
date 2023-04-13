using Microsoft.EntityFrameworkCore;
using MyHostel_BackEnd.Models;
using Quartz;

namespace MyHostel_BackEnd.Quartz
{
    public class CreateNotificationJob : IJob
    {
        private MyHostelContext _context = new MyHostelContext();
        public async Task Execute(IJobExecutionContext context)
        {
            var transactions = await _context.Transactions.ToListAsync();
            foreach (var transaction in transactions)
            {
                var room = _context.Rooms.Where(r => r.Id == transaction.RoomId).SingleOrDefault();
                var residents = await _context.Residents.Where(r => r.RoomId == transaction.RoomId && r.Status == 1).ToListAsync();
                if (transaction.PaidAt != null)
                {
                    continue;
                }
                if((DateTime.Now - (DateTime)transaction.CreatedAt).TotalDays <= 10){
                    continue;
                }
                if(room == null || residents.Count == 0)
                {
                    continue;
                }
                if(int.Parse(transaction.AtTime.ToString().Substring(3)) < DateTime.Now.Year)
                {
                    string[] others = transaction.Other.Split('-');
                    var total = transaction.Rent
                        + transaction.Electricity
                        + transaction.Water
                        + transaction.Internet;
                    foreach (var other in others)
                    {
                        decimal price = decimal.Parse(other.Split(':')[1]);
                        total += price;
                    }
                    foreach (var resident in residents)
                    {
                        Notification notification = new Notification
                        {
                            SendTo = resident.MemberId,
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
                    // send noti
                }
                else if (int.Parse(transaction.AtTime.ToString().Substring(3)) == DateTime.Now.Year)
                {
                    if (int.Parse(transaction.AtTime.ToString().Substring(0,2)) < DateTime.Now.Month)
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
                        foreach (var resident in residents)
                        {
                            Notification notification = new Notification
                            {
                                SendTo = resident.MemberId,
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
                        // send noti
                    }
                }
            }
            return;
        }
    }
}
