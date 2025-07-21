namespace Core
{
    public class PostModel
    {
        public required string Id { get; set; }
        public required PostType PostType { get; set; }

        public PostModel(PostType postType)
        {
            PostType = postType;
        }

        public PostModel SetPostType(PostType postType)
        {
            PostType = postType;
            return this;
        }
    }
}
