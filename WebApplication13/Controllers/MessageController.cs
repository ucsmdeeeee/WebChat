using Microsoft.AspNetCore.Mvc;
using WebApplication13.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

[Route("api/message")]
[ApiController]
public class MessageController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MessageController> _logger;

    public MessageController(ApplicationDbContext context, ILogger<MessageController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Получить все чаты для пользователя по Email
    [HttpGet("getChats/{email}")]
    public async Task<IActionResult> GetChats(string email)
    {
        // Находим текущего пользователя по Email
        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (currentUser == null)
        {
            _logger.LogError($"User with Email {email} not found.");
            return NotFound(new { message = "Пользователь не найден." });
        }

        // Получаем чаты, в которых участвует пользователь
        var chats = await _context.Chats
            .Where(c => c.User1Id == currentUser.Id || c.User2Id == currentUser.Id)
            .Select(c => new
            {
                ChatId = c.Id,
                ReceiverEmail = c.User1Id == currentUser.Id
                    ? _context.Users.Where(u => u.Id == c.User2Id).Select(u => u.Email).FirstOrDefault()
                    : _context.Users.Where(u => u.Id == c.User1Id).Select(u => u.Email).FirstOrDefault(),
                ReceiverName = c.User1Id == currentUser.Id
                    ? _context.Users.Where(u => u.Id == c.User2Id).Select(u => u.Username).FirstOrDefault()
                    : _context.Users.Where(u => u.Id == c.User1Id).Select(u => u.Username).FirstOrDefault()
            })
            .ToListAsync();

        _logger.LogInformation($"Chats loaded for user {email}: {chats.Count}");
        return Ok(chats);
    }

    // Получить все сообщения для чата по ChatId
    [HttpGet("getMessages/{chatId}")]
    public async Task<IActionResult> GetMessages(int chatId)
    {
        // Проверяем, существует ли чат
        var chatExists = await _context.Chats.AnyAsync(c => c.Id == chatId);
        if (!chatExists)
        {
            _logger.LogError($"Chat with ID {chatId} not found.");
            return NotFound(new { message = "Чат не найден." });
        }

        // Загружаем сообщения для чата
        var messages = await _context.Messages
            .Where(m => m.ChatId == chatId)
            .Include(m => m.Sender)
            .Select(m => new
            {
                SenderEmail = m.Sender.Email,
                SenderName = m.Sender.Username,
                Message = m.Content,
                Timestamp = m.Timestamp
            })
            .OrderBy(m => m.Timestamp)
            .ToListAsync();

        _logger.LogInformation($"Messages loaded for chat {chatId}: {messages.Count}");
        return Ok(messages);
    }

    [HttpGet("getChatFiles/{chatId}")]
    public async Task<IActionResult> GetChatFiles(int chatId)
    {
        var files = await _context.UploadedFiles
            .Where(f => f.ChatId == chatId)
            .Select(f => new
            {
                f.Id,
                f.FileName,
                f.Size,
                f.UploadedAt
            })
            .ToListAsync();

        return Ok(files);
    }

}
