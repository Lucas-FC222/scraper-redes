namespace Core
{
    public class AppUser
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = "";
        public string Name { get; set; } = "";

        public ICollection<string> TopicPreferences { get; set; }
            = new List<string>();
    }

}
