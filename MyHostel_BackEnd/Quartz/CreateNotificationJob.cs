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
                if(transaction.PaidAt != null)
                {
                    continue;
                }
                if((DateTime.Now - (DateTime)transaction.CreatedAt).TotalDays <= 10){
                    continue;
                }
                if(int.Parse(transaction.AtTime.ToString().Substring(3)) < DateTime.Now.Year)
                {
                    //add noti
                }
                else if (int.Parse(transaction.AtTime.ToString().Substring(3)) == DateTime.Now.Year)
                {
                    if (int.Parse(transaction.AtTime.ToString().Substring(0,2)) < DateTime.Now.Month)
                    {
                        //add noti
                    }
                }
            }
            return;
        }
    }
}
