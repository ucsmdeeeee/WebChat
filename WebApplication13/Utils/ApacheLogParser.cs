using System.Globalization;
using System.Text.RegularExpressions;
using WebApplication13.Models;

namespace WebApplication13.Utils
{
    public class ApacheLogParser
    {
        private static readonly Regex LogPattern = new Regex(
            @"^(?<IP>\S+) \S+ \S+ \[(?<Date>[^\]]+)\] ""(?<Request>[^""]+)"" (?<StatusCode>\d{3}) (?<BytesSent>\d+|-)",
            RegexOptions.Compiled);

        public static Log Parse(string logLine)
        {
            var match = LogPattern.Match(logLine);
            if (!match.Success)
            {
                throw new FormatException($"Invalid log format: {logLine}");
            }

            var date = DateTime.ParseExact(
                match.Groups["Date"].Value,
                "dd/MMM/yyyy:HH:mm:ss zzz",
                CultureInfo.InvariantCulture);

            return new Log
            {
                IP = match.Groups["IP"].Value,
                Date = date,
                Request = match.Groups["Request"].Value,
                StatusCode = int.Parse(match.Groups["StatusCode"].Value),
                BytesSent = match.Groups["BytesSent"].Value == "-" ? 0 : long.Parse(match.Groups["BytesSent"].Value)
            };
        }
    }
}
