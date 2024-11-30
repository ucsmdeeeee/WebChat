using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace WebApplication13.Services
{
    public class LogWriter : ILogWriter
    {
        private readonly string _logFilePath;

        public LogWriter(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public async Task WriteLogAsync(string ipAddress, DateTime dateTime, string request, int statusCode, long bytesSent)
        {
            // Преобразование IPv6 в IPv4, если это IPv6
            if (IPAddress.TryParse(ipAddress, out var ip))
            {
                if (ip.IsIPv4MappedToIPv6)
                {
                    ip = ip.MapToIPv4();
                }

                // Преобразование локального адреса "::1" в "127.0.0.1"
                if (ip.ToString() == "::1")
                {
                    ip = IPAddress.Parse("127.0.0.1");
                }

                ipAddress = ip.ToString();
            }

            // Форматирование времени в локальное время
            var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));

            // Форматирование строки для записи
            var logLine = $"{ipAddress} - - [{localDateTime.ToString("dd/MMM/yyyy:HH:mm:ss zzz", CultureInfo.InvariantCulture)}] \"{request}\" {statusCode} {bytesSent}";

            // Запись строки в лог-файл
            await File.AppendAllTextAsync(_logFilePath, logLine + Environment.NewLine);

            Console.WriteLine($"Log written: {logLine}");
        }
    }
}
