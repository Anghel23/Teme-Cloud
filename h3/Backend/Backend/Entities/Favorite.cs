namespace Backend.Entities
{
    public class Favorite
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string RawgId { get; set; }
    }
}
