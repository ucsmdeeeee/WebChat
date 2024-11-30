using Quartz;
using WebApplication13.Services;

namespace WebApplication13.Jobs
{
    public class LogProcessingJob : IJob
    {
        private readonly ILogProcessor _logProcessor;

        public LogProcessingJob(ILogProcessor logProcessor)
        {
            _logProcessor = logProcessor;
        }

        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Executing log processing job...");
            _logProcessor.ProcessLogFile("access.log");
            Console.WriteLine("Log processing job completed.");
            return Task.CompletedTask;
        }
    }
}
