using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TodoApi.Contracts.V1;
using TodoApi.Contracts.V1.Requests;
using TodoApi.Contracts.V1.Responses;
using TodoApi.Domains;
using TodoApi.Ectensions;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : Controller
    {

        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }


        // GET: api/posts
        [HttpGet(ApiRoutes.Post.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _postService.GetPostsAsync());
        }

        // GET: api/posts/{id}
        [HttpGet(ApiRoutes.Post.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);

            if (post == null)
                return NotFound();

            return Ok(post);
        }

        // PUT: api/posts/{id}
        [HttpPut(ApiRoutes.Post.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest request)
        {

            var userOwnsPost = await _postService.UserOwnsPost(postId, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new { Error = "You don't own this post" });
            }

            var post = await _postService.GetPostByIdAsync(postId);
            post.Name = request.Name;

            var updated = await _postService.UpdatePostAsync(post);

            if (updated)
                return Ok(post);

            return NotFound();
        }

        // POST: api/posts
        [HttpPost(ApiRoutes.Post.Create)]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            var post = new Post
            {
                Name = request.Name,
                UserId = HttpContext.GetUserId()
            };

            await _postService.CreatePostAsync(post);

            var response = new PostResponse { Id = post.Id, Name = post.Name };

            var baseUrl = $"{HttpContext.Request.Scheme}//{HttpContext.Request.Host.ToUriComponent()}";
            var locarionUri = baseUrl + "/" + ApiRoutes.Post.Get.Replace("{postId}", post.Id.ToString());

            return Created(locarionUri, response);
        }

        // DELETE: api/posts/{id}
        [HttpDelete(ApiRoutes.Post.Delete)]
        public async Task<IActionResult> Delete(Guid postId)
        {

            var userOwnsPost = await _postService.UserOwnsPost(postId, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new { Error = "You don't own this post" });
            }
             
            var deleted = await _postService.DeletePostAsync(postId);

            if (deleted)
                return NoContent();

            return NotFound();
        }
    }
}
