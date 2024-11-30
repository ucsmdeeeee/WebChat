using System;
using System.IO;
using System.Globalization;
using WebApplication13.Models;
using WebApplication13.Utils;
using System.Net;

namespace WebApplication13.Services
{
    public class LogProcessor : ILogProcessor
    {
        private readonly ApplicationDbContext _dbContext;

        public LogProcessor(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ProcessLogFile(string filePath)
        {
            Console.WriteLine($"Processing log file at: {filePath}");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Log file not found at {filePath}");
            }

            var lines = File.ReadAllLines(filePath);
            Console.WriteLine($"Found {lines.Length} lines in the log file.");

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    Console.WriteLine("Skipping empty line.");
                    continue;
                }

                var logEntry = ParseLogLine(line);
                if (logEntry != null)
                {
       
                    if (_dbContext.Logs.Any(log => log.Date == logEntry.Date))
                    {
                        Console.WriteLine($"Skipping duplicate log entry for date: {logEntry.Date}");
                        continue;
                    }

                    Console.WriteLine($"Adding new log entry: {logEntry.Date}");
                    _dbContext.Logs.Add(logEntry);
                }
                else
                {
                    Console.WriteLine($"Skipped invalid log line: {line}");
                }
            }

            Console.WriteLine("Saving changes to database...");
            _dbContext.SaveChanges();
            Console.WriteLine("Log processing completed.");
        }

        public Log ParseLogLine(string line)
        {
            var parts = line.Split(' ');

            if (parts.Length < 7)
            {
                Console.WriteLine($"Invalid log line format: {line}");
                return null;
            }

            try
            {
                // Парсинг IP-адреса
                var rawIp = parts[0];
                var ipAddress = IPAddress.Parse(rawIp);

                // Парсинг даты
                var rawDate = parts[3].TrimStart('[') + " " + parts[4].TrimEnd(']');
                var parsedDate = DateTime.ParseExact(rawDate, "dd/MMM/yyyy:HH:mm:ss zzz", CultureInfo.InvariantCulture);

                // Преобразование даты в UTC
                var utcDate = parsedDate.ToUniversalTime();

                // Парсинг остальных полей
                var request = string.Join(' ', parts.Skip(5).Take(3)).Trim('"');
                var statusCode = int.TryParse(parts[^2], out var code) ? code : 0;
                var bytesSent = long.TryParse(parts[^1], out var bytes) ? bytes : 0;

                return new Log
                {
                    IP = ipAddress.ToString(),
                    Date = utcDate, // Сохраняем время в UTC
                    Request = request,
                    StatusCode = statusCode,
                    BytesSent = bytesSent
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse log line: {line}. Error: {ex.Message}");
                return null;
            }
        }


    }

}
