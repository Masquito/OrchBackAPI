namespace Orch_back_API.Entities
{
    public class Notifications
    {
        public Guid Id { get; set; }
        public string? Content { get; set; }
        public DateTime? SendDate { get; set; }
        public Guid? DeliveryId { get; set; }
        public Users? Author { get; set; }
        public Guid? AuthorId { get; set; }
    }
}
