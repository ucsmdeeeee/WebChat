using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication13.Models;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        Console.Write("Введите токен: ");
        var token = Console.ReadLine();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Ввод и обработка даты начала
        Console.Write("Введите дату начала (yyyy-MM-dd): ");
        var startDate = ReadDateWithTimezone();

        // Ввод и обработка даты конца
        Console.Write("Введите дату конца (yyyy-MM-dd): ");
        var endDate = ReadDateWithTimezone();

        // Ввод группировки
        Console.Write("Группировать по (ip/date): ");
        var groupBy = Console.ReadLine();

        // Формирование URL
        var url = "https://localhost:7296/api/log/filter?";
        if (startDate.HasValue) url += $"startDate={startDate.Value:yyyy-MM-ddTHH:mm:ssZ}&";
        if (endDate.HasValue) url += $"endDate={endDate.Value:yyyy-MM-ddTHH:mm:ssZ}&";
        if (!string.IsNullOrEmpty(groupBy)) url += $"groupBy={groupBy}";

        // Убираем лишний символ '&' в конце
        url = url.TrimEnd('&');


        Console.WriteLine($"Request URL: {url}");

        // Отправка запроса
        try
        {
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ошибка запроса: {response.StatusCode}");
                Console.WriteLine($"Ответ сервера: {errorContent}");
                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("JSON-ответ от сервера:");
            Console.WriteLine(content);

            // Десериализация в список логов
            var logs = JsonSerializer.Deserialize<List<Log>>(content);

            Console.WriteLine("Результаты:");
            foreach (var log in logs)
            {
                Console.WriteLine($"ID: {log.Id}, IP: {log.IP}, Date: {log.Date}, Request: {log.Request}, StatusCode: {log.StatusCode}, BytesSent: {log.BytesSent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Общая ошибка: {ex.Message}");
        }



    }

    /// <summary>
    /// Читает дату с консоли и преобразует её в DateTimeOffset с временной зоной +03:00.
    /// </summary>
    /// <returns>Дата в формате DateTimeOffset с временной зоной +03:00.</returns>
    static DateTimeOffset? ReadDateWithTimezone()
    {
        while (true)
        {
            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                return null; // Если ввод пустой, возвращаем null
            }

            if (DateTime.TryParseExact(input, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                return new DateTimeOffset(parsedDate, TimeSpan.FromHours(3)); // Преобразуем в +03:00
            }

            Console.WriteLine("Неверный формат даты. Введите дату в формате yyyy-MM-dd:");
        }
    }
}
