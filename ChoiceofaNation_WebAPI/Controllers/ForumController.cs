using ChoiceofaNation_WebAPI.Logic.DTO;
using ChoiceofaNation_WebAPI.Logic.Entity;
using Logic.Entity;
using Logic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChoiceofaNation_WebAPI.Controllers
{
    [Route("/forum")]
    [ApiController]
    public class ForumController : ControllerBase
    {
        private readonly Data.DbContext _context;

        public ForumController(Data.DbContext context)
        {
            _context = context;
        }

        [HttpGet("get-topics")]
        public async Task<ActionResult<IEnumerable<Topic>>> GetTopics()
        {
            var topics = await _context.Topics.ToListAsync();
            return Ok(topics);
        }

        [HttpGet("get-topic/{id}")]
        public async Task<IActionResult> GetTopic(string id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound("Topic not found");
            }

            return Ok(topic);
        }

        [HttpPost("create-topic")]
        public async Task<IActionResult> CreateTopic([FromBody] CreateTopicDTO createTopicDTO)
        {
            var newTopic = new Topic
            {
                Title = createTopicDTO.Title,
                UserId = createTopicDTO.UserId,
                Description = createTopicDTO.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Topics.Add(newTopic);
            await _context.SaveChangesAsync();

            return Ok(newTopic);
        }

        [HttpPost("add-comment")]
        public async Task<IActionResult> AddComment([FromBody] AddCommentDTO addCommentDTO)
        {
            var newCom = new Comment
            {
                UserId = addCommentDTO.UserId,
                TopicId = addCommentDTO.TopicId,
                Content = addCommentDTO.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(newCom);
            await _context.SaveChangesAsync();

            return Ok(newCom);
        }

        [HttpGet("get-comments/{id}")]
        public async Task<IActionResult> GetComments(string id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound("Topic not found");
            }

            var comments = await _context.Comments
                                         .Where(c => c.TopicId == id) 
                                         .OrderByDescending(c => c.CreatedAt) 
                                         .ToListAsync();

            return Ok(comments);
        }

        [HttpDelete("delete-topic/{id}")]
        public async Task<IActionResult> DeleteTopic(string id)
        {
            var topic = await _context.Topics.FirstOrDefaultAsync(t => t.Id == id);
            if (topic == null)
            {
                return NotFound("Тему не знайдено");
            }

            var comments = await _context.Comments
                             .Where(c => c.TopicId == topic.Id)
                             .ToListAsync();

            _context.Comments.RemoveRange(comments);

            _context.Topics.Remove(topic);

            await _context.SaveChangesAsync();

            return Ok("Тема та її коментарі успішно видалені");
        }

        [HttpDelete("delete-comm/{id}")]
        public async Task<IActionResult> DeleteComm(string id)
        {
            var topic = await _context.Comments.FirstOrDefaultAsync(t => t.Id == id);
            if (topic == null)
            {
                return NotFound("Коментар не знайдено");
            }

            _context.Comments.Remove(topic);

            await _context.SaveChangesAsync();

            return Ok("Коментар успішно видалені");
        }
    }
}
