using System;
using System.ComponentModel.DataAnnotations;

namespace TodoApi.Domains
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
