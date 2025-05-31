using ChoiceofaNation_WebAPI.Logic.Entity;
using Logic.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChoiceofaNation_WebAPI.Controllers
{
    [Route("/achivs")]
    [ApiController]
    public class AchivsController : ControllerBase
    {
        private readonly Data.DbContext _context;

        public AchivsController(Data.DbContext context)
        {
            _context = context;
        }

        [HttpGet("get-achivs")]
        public async Task<ActionResult<IEnumerable<Achivments>>> GetAchivs()
        {
            var achivs = await _context.Achivments.ToListAsync();
            return Ok(achivs);
        }

        [HttpDelete("delete-achiv/{id}")]
        public async Task<IActionResult> DeleteAchiv(int id)
        {
            var achiv = await _context.Achivments.FindAsync(id);
            if (achiv == null)
            {
                return NotFound("Досягнення не знайдено");
            }

            _context.Achivments.Remove(achiv);
            await _context.SaveChangesAsync();

            return Ok($"Досягнення {achiv.Name} видалений");
        }
    }
}
