using System.Threading.Tasks;

namespace WebApplication13.Services
{
    public interface ILogWriter
    {
        Task WriteLogAsync(string ipAddress, DateTime timestamp, string request, int statusCode, long bytesSent);
    }
}
