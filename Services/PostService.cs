using System;
using System.Collections.Generic;
using TodoApi.Domains;
using TodoApi.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext context;

        public PostService(DataContext dataContext)
        {
            context = dataContext;
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            return await context.Posts.ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            return await context.Posts.SingleOrDefaultAsync(post => post.Id == postId);
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            await context.Posts.AddAsync(post);
            var created = await context.SaveChangesAsync();

            return created > 0;
        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            context.Posts.Update(postToUpdate);
            var updated = await context.SaveChangesAsync();

            return updated > 0;
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await GetPostByIdAsync(postId);
            if (post == null)
                return false;

            context.Posts.Remove(post);
            var deleted = await context.SaveChangesAsync();

            return deleted > 0;
        }

        public async Task<bool> UserOwnsPost(Guid postId, string UserId)
        {
            var post = await context.Posts.AsNoTracking().SingleOrDefaultAsync(post => post.Id == postId);

            if (post == null)
                return false;

            return post.UserId == UserId;
        }
    }
}
