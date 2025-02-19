using Logic.Entity;

namespace ChoiceofaNation_WebAPI.Logic.Entity
{
    public class Comment
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }
        public string TopicId { get; set; }
        public Topic? Topic { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
