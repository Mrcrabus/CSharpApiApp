using Microsoft.EntityFrameworkCore;
using WebApplication1.Helpers;
using WebApplication1.Model;

namespace WebApplication1.Services
{
    public class PostsService
    {
        private readonly DataContext _db;

        public PostsService(DataContext context)
        {
            _db = context;
        }

        public async Task<Post[]> AllPosts()
        {
            Post[] posts = await _db.Posts.Include(x => x.Author).ToArrayAsync();

            Post[] reversedPosts = posts.Clone() as Post[];
            Array.Reverse(reversedPosts);

            return reversedPosts;
        }

        public async Task<Post[]> UserPosts(string userId)
        {
            Post[] posts = await _db.Posts.Where(p => p.AuthorId == userId).Include(x => x.Author).ToArrayAsync();

            Post[] reversedPosts = posts.Clone() as Post[];
            Array.Reverse(reversedPosts);

            return reversedPosts;
        }

        public async Task AddPost(Post post)
        {
            await _db.Posts.AddAsync(post);

            await _db.SaveChangesAsync();
        }
        public async Task UpdatePost(Post post)
        {
            _db.Posts.Update(post);
            await _db.SaveChangesAsync();
        }

        public async Task<Post> GetPost(string id)
        {
            var post = await _db.Posts.Include(x => x.Author).SingleOrDefaultAsync(p => p.Id == id);

            return post;
        }

        public async Task<string> DeletePost(string postId)
        {
            var post = await GetPost(postId);

            if (post != null)
            {
                _db.Posts.Remove(post);
                await _db.SaveChangesAsync();
                return post.Id;
            }
            else
            {
                return null;
            }
        }

        public async Task ChangeImage()
        {
            await _db.SaveChangesAsync();
        }

        // public async Task<string> EditPost(string postId)
        // {
        //     var post = await GetPost(postId);

        //     if (post != null)
        //     {

        //     }
        // }



    }
}
