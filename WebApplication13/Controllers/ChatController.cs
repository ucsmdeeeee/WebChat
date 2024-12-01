using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication13.Models;

namespace WebApplication13.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChatController> _logger;

        public ChatController(ApplicationDbContext context, ILogger<ChatController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("chats/{email}")]
        public async Task<IActionResult> GetChats(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден." });
            }

            var chats = await _context.Chats
                .Where(c => c.User1Id == user.Id || c.User2Id == user.Id)
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Select(c => new
                {
                    ChatId = c.Id,
                    ReceiverName = c.User1Id == user.Id ? c.User2.Username : c.User1.Username,
                    ReceiverEmail = c.User1Id == user.Id ? c.User2.Email : c.User1.Email
                })
                .ToListAsync();

            return Ok(chats);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatDto chatDto)
        {
            var user1 = await _context.Users.FirstOrDefaultAsync(u => u.Email == chatDto.User1Email);
            var user2 = await _context.Users.FirstOrDefaultAsync(u => u.Email == chatDto.User2Email);

            if (user1 == null || user2 == null)
            {
                return NotFound(new { message = "Один из пользователей не существует" });
            }

            var existingChat = await _context.Chats
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == user1.Id && c.User2Id == user2.Id) ||
                    (c.User1Id == user2.Id && c.User2Id == user1.Id));

            if (existingChat != null)
            {
                return Conflict(new { message = "Чат уже существует" });
            }

            var chat = new Chat
            {
                User1Id = user1.Id,
                User2Id = user2.Id
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Чат создан успешно!" });
        }

        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto messageDto)
        {
            var sender = await _context.Users.FirstOrDefaultAsync(u => u.Email == messageDto.SenderEmail);
            var receiver = await _context.Users.FirstOrDefaultAsync(u => u.Email == messageDto.ReceiverEmail);

            if (sender == null || receiver == null)
            {
                return NotFound(new { message = "Пользователь не найден." });
            }

            var message = new Message
            {
                ChatId = messageDto.ChatId,
                SenderId = sender.Id,
                ReceiverId = receiver.Id,
                Content = messageDto.Content,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Сообщение отправлено успешно!" });
        }
        [HttpPost("uploadFile")]
        public async Task<IActionResult> UploadFile([FromForm] int chatId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Invalid file." });
            }

            // Проверяем, существует ли чат
            var chatExists = await _context.Chats.AnyAsync(c => c.Id == chatId);
            if (!chatExists)
            {
                return NotFound(new { message = "Chat not found." });
            }

            var filePath = Path.Combine("UploadedFiles", chatId.ToString(), file.FileName);

            try
            {
                // Создаем папку для чата, если она отсутствует
                Directory.CreateDirectory(Path.Combine("UploadedFiles", chatId.ToString()));

                // Сохраняем файл
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Сохраняем информацию о файле в БД
                var uploadedFile = new UploadedFile
                {
                    ChatId = chatId,
                    FileName = file.FileName,
                    FilePath = filePath,
                    Size = file.Length,
                    UploadedAt = DateTime.UtcNow
                };

                _context.UploadedFiles.Add(uploadedFile);
                await _context.SaveChangesAsync();

                return Ok(new { message = "File uploaded successfully.", file = uploadedFile });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error uploading file.", error = ex.Message });
            }
        }




        [HttpGet("downloadFile/{fileId}")]
        public IActionResult DownloadFile(int fileId)
        {
            var file = _context.UploadedFiles.FirstOrDefault(f => f.Id == fileId);
            if (file == null || !System.IO.File.Exists(file.FilePath))
            {
                return NotFound(new { message = "File not found." });
            }

            var fileBytes = System.IO.File.ReadAllBytes(file.FilePath);
            return File(fileBytes, "application/octet-stream", file.FileName);
        }
    



    public class CreateChatDto
        {
            public string User1Email { get; set; }
            public string User2Email { get; set; }
        }

        public class SendMessageDto
        {
            public int ChatId { get; set; }
            public string SenderEmail { get; set; }
            public string ReceiverEmail { get; set; }
            public string Content { get; set; }
        }

    }
}