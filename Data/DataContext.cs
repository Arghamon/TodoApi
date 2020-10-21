using System;
using Microsoft.EntityFrameworkCore;
using TodoApi.Domains;

namespace TodoApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
    }
}
