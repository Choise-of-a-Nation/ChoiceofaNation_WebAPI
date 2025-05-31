namespace ChoiceofaNation_WebAPI.Logic.DTO
{
    public class UpdateAchivsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameEng { get; set; }
        public string Description { get; set; }
        public string DescriptionEng { get; set; }
        public string IconUrl { get; set; }
        public bool isOk { get; set; }
    }
}
