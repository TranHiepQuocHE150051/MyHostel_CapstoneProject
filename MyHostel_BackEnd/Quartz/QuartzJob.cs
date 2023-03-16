using Quartz;

namespace MyHostel_BackEnd.Quartz
{
    public class QuartzJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Hello World");
            return Task.FromResult(true);
        }
    }
}
