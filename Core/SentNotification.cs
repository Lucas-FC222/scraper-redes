namespace Core
{
    public class SentNotification
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
        public string PostId { get; set; } = "";
        public DateTime SentAt { get; set; }
    }

}
