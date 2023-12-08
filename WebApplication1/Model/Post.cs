namespace WebApplication1.Model
{
    public class Post
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; } = "/post-default.jpg";
        public string AuthorName { get; set; }
        public string AuthorId { get; set; }
        public User Author { get; set; } = null!;
    }

}
