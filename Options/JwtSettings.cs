using System;
namespace TodoApi.Options
{
    public class JwtSettings
    {
        public string Secret { get; set;  }
        public TimeSpan LifeTime { get; set; }
    }
}
