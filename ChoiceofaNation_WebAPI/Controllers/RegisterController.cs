using ChoiceofaNation_WebAPI.Logic.DTO;
using ChoiceofaNation_WebAPI.Logic.Services;
using Logic.DTO;
using Logic.Entity;
using Logic.Services;
using Logic.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly BlobService _bloobService;

        public RegisterController(Data.DbContext context,
                                  JwtService jwtService,
                                  RefreshTokenService refreshTokenService,
                                  BlobService blobService)
        {
            _context = context;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _bloobService = blobService;
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

            var accessToken = _jwtService.GenerateJwtToken(regUser.Email, regUser.Id);
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

            var accessToken = _jwtService.GenerateJwtToken(user.Email, user.Id);
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

            var newAccessToken = _jwtService.GenerateJwtToken(user.Email, user.Id);
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

            using var stream = file.OpenReadStream();
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}"; 

            var url = await _bloobService.UploadFileAsync(stream, fileName); 

            return Ok(new { ImageUrl = url });
        }

    }
}
