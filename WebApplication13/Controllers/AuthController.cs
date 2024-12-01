using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication13.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto register)
    {

        try
        {
            if (_context.Users.Any(u => u.Email == register.Email))
            {

                return BadRequest(new { message = "Пользователь с данным email уже зарегестрирован" });
            }
            var user = new User
            {
                Username = register.Username,
                Email = register.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(register.PasswordHash),
                Role = register.Role
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Вы успешно зарегистрировались" });
        }
        catch (Exception ex)
        {

            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto login)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == login.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(login.PasswordHash, user.PasswordHash))
            return Unauthorized(new { message = "Неправильный email или пароль" });

        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }
    [HttpGet("signin-google")]
    public IActionResult SignInWithGoogle()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(HandleOAuthCallback))
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("oauth-callback")]
    public async Task<IActionResult> HandleOAuthCallback()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded)
        {
            return BadRequest(new { message = "OAuth аутентификация не удалась." });
        }

        // Извлечение данных пользователя
        var claims = result.Principal?.Claims;
        var userId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var username = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        if (userId == null || email == null)
        {
            return BadRequest(new { message = "Не удалось извлечь данные пользователя." });
        }

        if (string.IsNullOrEmpty(username))
        {
            // Если имя пользователя отсутствует, используем часть email до @
            username = email.Split('@')[0];
        }

        string role;

        // Проверка и добавление пользователя в БД
        using (var scope = HttpContext.RequestServices.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == email);

            if (existingUser == null)
            {
                // Создание нового пользователя
                var newUser = new User
                {
                    ExternalId = userId,
                    Email = email,
                    Username = username,
                    Role = "User" // Укажите стандартную роль для нового пользователя
                };

                dbContext.Users.Add(newUser);
                await dbContext.SaveChangesAsync();
                role = newUser.Role;
            }
            else
            {
                // Используем роль существующего пользователя
                role = existingUser.Role;
            }
        }

        // Генерация JWT токена
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("Am0Fquq4SV-l433bBp9xrelKeG416wDM9ijAZHDO-ik");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role) // Добавляем роль в токен
        }),
            Expires = DateTime.UtcNow.AddHours(1000),
            Issuer = "MessagingApp",
            Audience = "MessagingApp",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        Console.WriteLine($"JWT Generated: {jwt}");

        // Перенаправление на /home/messages с токеном
        return Redirect($"/home/messages?token={jwt}");
    }



    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("role", user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1000),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
