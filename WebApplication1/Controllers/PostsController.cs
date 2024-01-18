using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Dto;
using WebApplication1.Model;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/posts")]
    public class PostsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly PostsService _postService;

        public PostsController(PostsService postsService, UserManager<User> userManager)
        {
            _postService = postsService;
            _userManager = userManager;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _postService.AllPosts();
            return Ok(posts);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> UserPosts([FromRoute] string userId)
        {
            var posts = await _postService.UserPosts(userId);

            return Ok(posts);
        }

        [HttpGet("post/{postId}")]
        public async Task<IActionResult> Post([FromRoute] string postId)
        {
            var post = await _postService.GetPost(postId);

            if (post == null) return BadRequest("Not found");

            return Ok(post);
        }

        [Authorize()]
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost([FromRoute] string postId)
        {
            var post = await _postService.DeletePost(postId);

            if (post == null) return BadRequest("Post not found");

            return Ok();
        }

        [Authorize()]
        [HttpPut()]
        public async Task<IActionResult> EditPost([FromBody] AddPostDto post)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userId = User.FindFirst("id")?.Value;
            var userName = User.FindFirst("userName")?.Value;
            var author = await _userManager.FindByEmailAsync(userEmail);

            var newPost = new Post
            {
                Id = Guid.NewGuid().ToString(),
                Author = author,
                Title = post.Title,
                Description = post.Description,
                AuthorId = userId,
                AuthorName = userName
            };

            await _postService.UpdatePost(newPost);

            return Ok(newPost);
        }

        [Authorize()]
        [HttpPost()]
        public async Task<IActionResult> CreatePost([FromBody] AddPostDto post)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userId = User.FindFirst("id")?.Value;
            var userName = User.FindFirst("userName")?.Value;
            var author = await _userManager.FindByEmailAsync(userEmail);


            var newPost = new Post
            {
                Id = Guid.NewGuid().ToString(),
                Author = author,
                Title = post.Title,
                Description = post.Description,
                AuthorId = userId,
                AuthorName = userName
            };

            await _postService.AddPost(newPost);

            return Ok(newPost);
        }


        [HttpPost("img/{postId}")]
        public async Task<IActionResult> UploadImageAsync(IFormFile file, [FromRoute] string postId)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is required");
            }

            var fileName = postId + Path.GetExtension(file.FileName);

            var filePath = Path.Combine("uploads/postsImages", fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(fileName);
        }


        [HttpGet("image/{imageName}")]
        public IActionResult GetImage(string imageName)
        {
            var imagePath = Path.Combine("uploads", imageName);

            if (!System.IO.File.Exists(imagePath)) return NotFound();

            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
            return File(imageBytes, "image/jpg");
        }
    }
}