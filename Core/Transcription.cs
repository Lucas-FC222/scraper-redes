namespace Core
{
   public class Transcription
    {
        public int Id { get; set; }

        public int VideoIdFk { get; set; }

        public Video Video { get; set; }

        public string Text { get; set; }

        public string Resume { get; set; }

    }
}
