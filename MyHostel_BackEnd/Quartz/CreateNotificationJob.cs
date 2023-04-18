using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using MyHostel_BackEnd.Models;
using Quartz;
using Notification = MyHostel_BackEnd.Models.Notification;

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
                if (transaction.Status == 2 || transaction.Status==3)
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
                    if (transaction.PaidAmount != null)
                    {
                        total = total - transaction.PaidAmount;
                    }
                    foreach (var resident in residents)
                    {
                        Notification notification = new Notification
                        {
                            SendTo = resident.MemberId,
                            SendAt = DateTime.Now,
                            CreateAt = DateTime.Now,
                            SendAtHour = 9,
                            Type = 1,
                            Message = "Tiền cần đóng của " + room.Name + ": " +
                        total.ToString()
                        };
                        _context.Notifications.Add(notification);
                        SendNotification(resident, notification);
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
                        if (transaction.PaidAmount != null)
                        {
                            total = (decimal)(total - transaction.PaidAmount);
                        }
                        foreach (var resident in residents)
                        {
                            Notification notification = new Notification
                            {
                                SendTo = resident.MemberId,
                                SendAt = DateTime.Now,
                                CreateAt = DateTime.Now,
                                SendAtHour = 9,
                                Type = 1,
                                Message = "Tiền cần đóng của " + room.Name + ": " +
                            total.ToString()
                            };
                            _context.Notifications.Add(notification);
                            SendNotification(resident,notification);
                        }
                        _context.SaveChanges();
                        // send noti
                    }
                }
            }
            return;
        }
        private async void SendNotification(Resident resident, Notification notification)
        {
            try
            {
                var member = _context.Members.Where(m => m.Id == resident.MemberId).SingleOrDefault();
                var registrationToken = member.FcmToken;
                if (registrationToken.Equals(""))
                {
                    return;
                }
                var data = notification.Message.Split(':');
                var message = new FirebaseAdmin.Messaging.Message()
                {
                    Data = new Dictionary<string, string>()
                    {
                        { data[0]+": ", data[1] }
                    },
                    Token = registrationToken,
                };
            }
            catch (Exception e)
            {
                return;
            }
        }
    }
}
