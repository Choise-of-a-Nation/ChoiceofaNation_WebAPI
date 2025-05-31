using Logic.Entity;

namespace ChoiceofaNation_WebAPI.Logic.Entity
{
    public class Achivments
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameEng { get; set; }
        public string Description { get; set; }
        public string DescriptionEng { get; set; }
        public string IconUrl { get; set; }
        public bool isOk { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }
    }
}
