using ChoiceofaNation_WebAPI.Logic.DTO;
using ChoiceofaNation_WebAPI.Logic.Entity;
using Logic.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChoiceofaNation_WebAPI.Controllers
{
    [Route("/history")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly Data.DbContext _context;

        public HistoryController(Data.DbContext context)
        {
            _context = context;
        }

        [HttpGet("get-wiki")]
        public async Task<ActionResult<IEnumerable<HistoryWiki>>> GetWiki()
        {
            var wiki = await _context.HistoryWikis.ToListAsync();
            return Ok(wiki);
        }

        [HttpGet("get-wik/{id}")]
        public async Task<IActionResult> GetWik(int id)
        {
            var wik = await _context.HistoryWikis.FindAsync(id);
            if (wik == null)
            {
                return NotFound("Wiki not found");
            }

            return Ok(wik);
        }

        [HttpPost("create-wiki")]
        public async Task<IActionResult> CreateWiki([FromBody] WikiDTO wikiDTO)
        {
            var newWiki = new HistoryWiki
            {
                Title = wikiDTO.Title,
                Description = wikiDTO.Description,
                Url = wikiDTO.Url,
                CreatedDate = DateTime.UtcNow
            };

            _context.HistoryWikis.Add(newWiki);
            _context.SaveChanges();

            return Ok();
        }
    }
}
