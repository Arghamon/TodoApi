using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Contracts.V1;
using TodoApi.Contracts.V1.Requests;
using TodoApi.Contracts.V1.Responses;
using TodoApi.Domains.V1;

namespace TodoApi.Controllers
{
    public class PostsController : Controller
    {

        private List<Post> _posts;

        public PostsController()
        {
            _posts = new List<Post>();
            for (var i = 0; i < 5; i++)
            {
                _posts.Add(new Post { Id = Guid.NewGuid().ToString() });
            }
        }

        [HttpGet(ApiRoutes.Post.GetAll)]
        public IActionResult Get()
        {
            return Ok(_posts);
        }

        [HttpPost(ApiRoutes.Post.Create)]
        public IActionResult CreatePost([FromBody] PostRequest postRequest)
        {
            var post = new Post { Id = postRequest.Id };


            if (post.Id == null)
                return BadRequest();

            var respnse = new PostResponse { Id = post.Id };

            var baseUrl = $"{HttpContext.Request.Scheme}//{HttpContext.Request.Host.ToUriComponent()}";
            var locarionUri = baseUrl + "/" + ApiRoutes.Post.Get.Replace("{postId}", post.Id);

            return Created(locarionUri, respnse);
        }
    }
}
