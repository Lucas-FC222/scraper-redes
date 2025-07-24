namespace Core.Models
{
    public class AppUser
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = "";
        public string Name { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "User"; // "User" ou "Admin"

        public ICollection<string> TopicPreferences { get; set; }
            = new List<string>();
    }

}
