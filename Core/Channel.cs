using System.ComponentModel.DataAnnotations;


namespace Core
{
   public class Channel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ChannelId { get; set; }

        public ICollection<Video> Videos { get; set; }
    }
}
