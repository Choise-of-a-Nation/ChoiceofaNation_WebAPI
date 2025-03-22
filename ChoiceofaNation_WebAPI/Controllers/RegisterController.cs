using ChoiceofaNation_WebAPI.Logic.DTO;
using Google.Apis.Auth;
using Logic.DTO;
using Logic.Entity;
using Logic.Services;
using Logic.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace ChoiceofaNation_WebAPI.Controllers
{
    [Route("/register")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly Data.DbContext _context;
        private readonly JwtService _jwtService;
        private readonly RefreshTokenService _refreshTokenService;
        private readonly string _googleClientId;
        private readonly IConfiguration _configuration;

        public RegisterController(Data.DbContext context,
                                  JwtService jwtService,
                                  RefreshTokenService refreshTokenService,
                                  IConfiguration configuration)
        {
            _context = context;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _configuration = configuration;
            _googleClientId = _configuration["Authentication:Google:ClientId"];
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserDTO userDTO)
        {
            if (_context.Users.Any(u => u.Email == userDTO.Email))
            {
                return BadRequest("User with this email already exists.");
            }

            var regUser = new User
            {
                UserName = userDTO.Username,
                NormalizedUserName = userDTO.Username.Normalize(),
                Email = userDTO.Email,
                NormalizedEmail = userDTO.Email.Normalize(),
                PasswordHash = HasherService.ComputeSHA256Hash(userDTO.Password),
                RoleId = "Client"
            };

            var accessToken = _jwtService.GenerateJwtToken(regUser.Email, regUser.Id, regUser.RoleId);
            var refreshToken = _refreshTokenService.GenerateRefreshToken();

            regUser.RefreshToken = refreshToken;
            regUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            _context.Users.Add(regUser);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterUserAdmin([FromBody] UserDTOAdmin userDTO)
        {
            if (_context.Users.Any(u => u.Email == userDTO.Email))
            {
                return BadRequest("User with this email already exists.");
            }

            var regUser = new User
            {
                UserName = userDTO.Username,
                NormalizedUserName = userDTO.Username.Normalize(),
                Email = userDTO.Email,
                NormalizedEmail = userDTO.Email.Normalize(),
                PasswordHash = HasherService.ComputeSHA256Hash(userDTO.Password),
                RoleId = userDTO.RoleId
            };

            var accessToken = _jwtService.GenerateJwtToken(regUser.Email, regUser.Id, regUser.RoleId);
            var refreshToken = _refreshTokenService.GenerateRefreshToken();

            regUser.RefreshToken = refreshToken;
            regUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            _context.Users.Add(regUser);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpGet("decode-token")]
        public IActionResult DecodeToken(string token)
        {
            var claimsPrincipal = _jwtService.DecodeJwtToken(token);

            if (claimsPrincipal == null)
            {
                return Unauthorized("Invalid token");
            }

            // Отримуємо клейми
            var claims = claimsPrincipal.Claims.Select(c => new { c.Type, c.Value });

            return Ok(claims);
        }

        [HttpGet("get-users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == loginDTO.Email);
            if (user == null || !HasherService.VerifyPassword(loginDTO.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }

            var accessToken = _jwtService.GenerateJwtToken(user.Email, user.Id, user.RoleId);
            var refreshToken = _refreshTokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("token/refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.RefreshToken == request.RefreshToken);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return Unauthorized("Invalid or expired refresh token");
            }

            var newAccessToken = _jwtService.GenerateJwtToken(user.Email, user.Id, user.RoleId);
            var newRefreshToken = _refreshTokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;
                await _context.SaveChangesAsync();
            }

            return Ok("Logged out successfully");
        }

        [HttpGet("get-user/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }


        [HttpPut("update-user/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDTO model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.UserName = model.Username ?? user.UserName;
            user.FirstName = model.FirstName ?? user.FirstName;
            user.LastName = model.LastName ?? user.LastName;
            user.Email = model.Email ?? user.Email;
            user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;
            user.Url = model.Url ?? user.Url;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPut("update-user-admin/{id}")]
        public async Task<IActionResult> UpdateUserAdmin(string id, [FromBody] UpdateUserDTOAdmin model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.UserName = model.Username ?? user.UserName;
            user.FirstName = model.FirstName ?? user.FirstName;
            user.LastName = model.LastName ?? user.LastName;
            user.Email = model.Email ?? user.Email;
            user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;
            user.Url = model.Url ?? user.Url;
            user.RoleId = model.RoleId ?? user.RoleId;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPut("change-password/{id}")]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordDTO model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.PasswordHash = HasherService.ComputeSHA256Hash(model.NewPassword);
            await _context.SaveChangesAsync();

            return Ok("Password changed successfully.");
        }

        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var uploadPath = Path.Combine("wwwroot", "uploads");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

            return Ok(new { ImageUrl = fileUrl });
        }


        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Користувача не знайдено");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok($"Користувач {user.UserName} видалений");
        }

        public class GoogleLoginRequest
        {
            public string Credential { get; set; }
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                // Валідація Google токена
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { _googleClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(request.Credential, settings);

                // Пошук користувача в базі даних
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

                if (user == null)
                {
                    // Створення нового користувача, якщо він не існує
                    user = new User
                    {
                        Email = payload.Email,
                        NormalizedEmail = payload.Email.Normalize(),
                        UserName = payload.Name,
                        NormalizedUserName = payload.Name.Normalize(),
                        Url = payload.Picture,
                        RoleId = "Client"
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                // Генерація JWT токенів
                var accessToken = _jwtService.GenerateJwtToken(user.Email, user.Id, user.RoleId);
                var refreshToken = _refreshTokenService.GenerateRefreshToken();

                // Зберігання refresh токена
                user.RefreshToken = refreshToken;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            catch (InvalidJwtException)
            {
                return BadRequest(new { message = "Недійсний Google токен" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутрішня помилка сервера", error = ex.Message });
            }
        }

    }
}
