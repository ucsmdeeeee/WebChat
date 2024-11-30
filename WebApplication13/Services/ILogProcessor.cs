using WebApplication13.Models;

namespace WebApplication13.Services
{
    public interface ILogProcessor
    {
        void ProcessLogFile(string filePath);
        Log ParseLogLine(string line);
    }
}
