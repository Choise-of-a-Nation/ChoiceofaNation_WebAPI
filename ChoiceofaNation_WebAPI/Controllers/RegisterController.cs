using Logic.DTO;
using Logic.Entity;
using Logic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace ChoiceofaNation_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly Data.DbContext _context;
        private readonly JwtService _jwtService;

        public RegisterController(IConfiguration configuration, 
                                  Data.DbContext context,
                                  JwtService jwtService)
        {
            _configuration = configuration;
            _context = context;
            _jwtService = jwtService;
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
                PasswordHash = HasherService.ComputeSHA256Hash(userDTO.Password)
            };

            regUser.Token = _jwtService.GenerateJwtToken(userDTO.Email, regUser.Id);

            _context.Users.Add(regUser);
            await _context.SaveChangesAsync();

            return Ok(new { regUser.Token });
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

    }
}
