namespace m151_backend.Entities
{
    public class RunNote
    {
        public Guid Id { get; set; }
        public Guid RunId { get; set; }
        public string Note { get; set; }
    }
}
