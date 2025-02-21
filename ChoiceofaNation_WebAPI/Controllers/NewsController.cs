using ChoiceofaNation_WebAPI.Logic.DTO;
using Logic.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChoiceofaNation_WebAPI.Controllers
{
    [Route("/news")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly Data.DbContext _context;

        public NewsController(Data.DbContext context)
        {
            _context = context;
        }

        [HttpGet("get-news")]
        public async Task<ActionResult<IEnumerable<News>>> GetNews()
        {
            var news = await _context.News.ToListAsync();
            return Ok(news);
        }

        [HttpGet("get-new/{id}")]
        public async Task<IActionResult> GetNew(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound("News not found");
            }

            return Ok(news);
        }

        [HttpPost("create-news")]
        public async Task<IActionResult> CreateNews([FromBody] NewsDTO newsDTO)
        {
            var newNews = new News
            {
                Name = newsDTO.Name,
                Description = newsDTO.Description,
                Url = newsDTO.Url,
                CreatedDate = DateTime.UtcNow
            };

            _context.News.Add(newNews);
            _context.SaveChanges();

            return Ok();
        }
    }
}
