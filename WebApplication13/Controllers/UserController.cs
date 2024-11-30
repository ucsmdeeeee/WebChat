using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserController> _logger;

    public UserController(ApplicationDbContext context, ILogger<UserController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Получить список всех пользователей (кроме текущего)
    [HttpGet("all/{email}")]
    public async Task<IActionResult> GetUsers(string email)
    {
        // Находим текущего пользователя по Email
        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (currentUser == null)
        {
            _logger.LogError($"User with Email {email} not found.");
            return NotFound("User not found.");
        }

        // Ищем всех пользователей, кроме текущего
        var users = await _context.Users
            .Where(u => u.Email != email) // Исключаем текущего пользователя
            .Select(u => new { Email = u.Email, Username = u.Username }) // Возвращаем Email и Username
            .ToListAsync();

        _logger.LogInformation($"Users loaded: {string.Join(", ", users.Select(u => u.Username))}");
        return Ok(users);
    }






}
